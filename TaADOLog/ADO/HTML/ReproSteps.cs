using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using ImageMagick;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Serilog.Events;
using TaADOLog.Logger;
using TaADOLog.Logger.Sink;

namespace TaADOLog.ADO.HTML
{
    internal static class ReproSteps
    {
        private static string OpenTable(object testcaseId, LogEventPropertyValue url, LogEventPropertyValue title,
            TimeSpan duration, object solutionVersion)
        {
            string returnValue = $"<div id='{testcaseId}'>" +
                             $"<span><b>TestCase:</b> <a href=\"{url}\"> {testcaseId} - {title}</a></span>" +
                             $"<br/>" +
                             $"<span><b>Duration:</b> {duration}</span>" +
                             $"<br/>" +
                             $"<span><b>Solution Version:</b> {solutionVersion}</span>" +
                             $"<br/>" +
                             $"<br/>" +
                             $"<table width:100%  style='border-collapse:collapse; padding:3px; border-color:#c0c0c0; border-width: 1px; border-style:solid'>" +
                             $"<thead style='background-color: #c0c0c0; padding:3px'>" +
                             $"<tr>" +
                             $"<th> Time </th>" +
                             $"<th> Severity </th>" +
                             $"<th> Step </th>" +
                             $"<th> Description </th>" +
                             $"<th> Url </th>" +
                             $"<th> Screenshot </th>" +
                             $"</tr></thead><tbody>";
            return returnValue;
        }

        private static AttachmentReference GetSecondScreenshot<T>(WorkItemTrackingHttpClient witClient, ListSinkInfo<T> log,
            string screenshotLastStep)
        {
            AttachmentReference tempScreenshot = null;
            if (log.Level.ToString() == LogEventLevel.Verbose.ToString() &&
                log.Message.Contains(SerilogExtensions.VerboseScreenshot))
            {
                bool addAttachment = true;

                if (!log.Multimedia.ToString().IsNullOrEmpty())
                {
                    if (!screenshotLastStep.IsNullOrEmpty())
                    {
                        using (var img1 = new MagickImage(screenshotLastStep))
                        {
                            img1.ColorFuzz = new Percentage(5);

                            using (var img2 = new MagickImage(log.Multimedia.ToString()))
                            {
                                using (var imgSimilarity = new MagickImage())
                                {
                                    double diff = img1.Compare(img2, ErrorMetric.Fuzz, imgSimilarity);
                                    if (diff < 0.02)
                                        addAttachment = false;
                                }
                            }
                        }
                    }

                    if (addAttachment)
                    {
                        FileStream attStream = new FileStream(log.Multimedia.ToString(), FileMode.Open,
                            FileAccess.Read);
                        tempScreenshot =
                            witClient.CreateAttachmentAsync(attStream).Result; // upload file
                        attStream.Dispose();
                    }
                }
            }

            return tempScreenshot;
        }

        private static string CloseTable(AttachmentReference screenshotError)
        {
            string returnValue = "</tbody></table>" +
                              "<br/>" +
                              "<br/>" +
                              $"<img src=\"{screenshotError?.Url}\">" +
                              "</div>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>";
            return returnValue;
        }

        private static string AddRow<T>(List<ListSinkInfo<T>> loggerSinkList, int actionFailedIndex, ListSinkInfo<T> log,
            Dictionary<int, string> stepDict, int actualStep, string logmsgModified, AttachmentReference screenshot,
            ref AttachmentReference screenshotBeforeInvoke)
        {
            string returnValue =
                $"<tr style=\"padding:10px\" >" +
                $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid \">" +
                $"<i>{log.DateTime:dd/MM/yyyy HH:mm:ss.fff}</i></td>" +
                $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                $"{((log.Properties.ContainsKey("MessageType") && !log.Properties["MessageType"].ToString().Replace("\"", "").Equals("Step")) ? log.Properties["MessageType"].ToString().Replace("\"", "") : log.Level)}</td>" +
                $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\"" +
                (!log.Step.IsNullOrEmpty() ? $"title=\"'{stepDict[actualStep]}'\">" : ">") +
                $"{log.Step}</td>" +
                $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                (loggerSinkList.IndexOf(log) == actionFailedIndex
                    ? $"❌ {logmsgModified}</td>"
                    : $"{logmsgModified}</td>") +
                $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                (log.Url.IsNullOrEmpty() ? "</td>" : $"<a href=\"{log.Url}\" target=\"_blank\">🌍</a></td>") +
                $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">";
            if (screenshotBeforeInvoke != null)
            {
                returnValue +=
                    $"<a href=\"{screenshotBeforeInvoke.Url}\" target=\"_blank\"><img src=\"{screenshotBeforeInvoke.Url}\"></a><br>";
            }

            if (screenshot != null)
            {
                returnValue +=
                    $"<a href=\"{screenshot.Url}\" target=\"_blank\"><img src=\"{screenshot.Url}\"></a>";
                // reset
                screenshotBeforeInvoke = null;
            }

            returnValue += "</td>" +
                              $"</tr>";
            return returnValue;
        }


        public static string GenerateHtml<T>(List<ListSinkInfo<T>> loggerSinkList, WorkItemTrackingHttpClient witClient, int actionFailedIndex, WorkItem testCase)
        {

            String reproStepsHTML;
            var testcaseInfo = loggerSinkList.FirstOrDefault(logInfo => logInfo.Properties.ContainsKey("TestCaseInfo"))?.Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Url)];
            var testcaseId = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Id)];
            var solutionVersion = BugManager.GetSolutionVersion(loggerSinkList);
            var duration = loggerSinkList.Last().DateTime - loggerSinkList.First().DateTime;


            var steps = testCase.Fields["Microsoft.VSTS.TCM.Steps"].ToString();
            var stepDict = new Dictionary<int, string>();

            var htmlstepsDoc = new HtmlDocument();
            htmlstepsDoc.LoadHtml(steps);
            var stepsNode = htmlstepsDoc.DocumentNode.SelectNodes("//step");

            var i = 1;
            foreach (var step in stepsNode)
            {
                var text = step.InnerText;
                text = text.Replace("&lt;", "<");
                text = text.Replace("&gt;", ">");
                text = text.Replace("\"", "'");
                var tmpDoc = new HtmlDocument();
                tmpDoc.LoadHtml(text);
                text = tmpDoc.DocumentNode.InnerText;
                text = text.Replace("&amp;nbsp;", "");

                stepDict.Add(i, text);
                i++;
            }

            reproStepsHTML = OpenTable(testcaseId, url, title, duration, solutionVersion);

            string screenshotLastStep = null;
            AttachmentReference screenshotError = null;
            AttachmentReference secondScreenshot = null;
            foreach (var log in loggerSinkList)
            {

                secondScreenshot = GetSecondScreenshot(witClient, log, screenshotLastStep);

                if (log.LogEvent.Level < LogEventLevel.Information)
                    continue;

                var logmsgModified = log.Message.Replace("\r\n", "<br>");
                if (log.EntityUrl.IsNullOrEmpty())
                {
                    var htmlLink = new Regex(@"(http[^\s""']+)");
                    var linkValue = htmlLink.Match(logmsgModified).Value;
                    logmsgModified = htmlLink.Replace(logmsgModified, $"<a href=\"{linkValue}\">{linkValue}</a>");
                }

                AttachmentReference screenshot = null;

                if (!log.Multimedia.ToString().IsNullOrEmpty())
                {
                    screenshotLastStep = log.Multimedia.ToString();
                    FileStream attStream =
                        new FileStream(log.Multimedia.ToString(), FileMode.Open, FileAccess.Read);
                    screenshot = witClient.CreateAttachmentAsync(attStream).Result; // upload file
                    attStream.Dispose();
                }

                int actualStep = -1;
                if (!log.Step.IsNullOrEmpty())
                    actualStep = Int32.Parse(log.Step);


                reproStepsHTML += AddRow(loggerSinkList, actionFailedIndex, log, stepDict, actualStep, logmsgModified, screenshot, ref secondScreenshot);

                if (loggerSinkList.IndexOf(log) == actionFailedIndex && screenshot != null)
                {
                    screenshotError = screenshot;
                }

            }

            reproStepsHTML += CloseTable(screenshotError);

            return reproStepsHTML;
        }

    }
}