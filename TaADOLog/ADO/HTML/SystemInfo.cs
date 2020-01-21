using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.TeamFoundation.Common;
using Serilog.Events;
using TaADOLog.Logger.Sink;

namespace TaADOLog.ADO.HTML
{
    internal static class SystemInfo
    {
        public static string GenerateHtml<T>(List<ListSinkInfo<T>> loggerSinkList, string oldSystemInfo = null)
        {
            var testcaseInfo =
                loggerSinkList.FirstOrDefault(logInfo => logInfo.Properties.ContainsKey("TestCaseInfo"))?
                    .Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Url)];
            var testcaseId = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Id)];
            var solutionVersion = BugManager.GetSolutionVersion(loggerSinkList);
            var foundDateTime = loggerSinkList.First().DateTime;

            HtmlNode solutionNode = null;
            HtmlDocument systemHtmlDocument = null;

            // if its an existing bug, get the solution node to append new testcases
            if (!oldSystemInfo.IsNullOrEmpty())
            {
                systemHtmlDocument = new HtmlDocument();
                systemHtmlDocument.LoadHtml(oldSystemInfo);

                solutionNode = systemHtmlDocument.DocumentNode.SelectSingleNode($"//table[@id='{solutionVersion}']/tbody");
            }

            // Testcase row
            var tCHtml = $"<tr id='{testcaseId}' style=\"padding:10px\" >" +
                         $"<td  style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid \">" +
                         $"<a href=\"{url}\">{testcaseId}-{title}</a></td>" +
                         $"<td  style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid \">" +
                         $"{foundDateTime.DateTime:dd/MM/yyyy HH:mm}</td>" +
                         $"</tr>";

            if (solutionNode != null)
            {
                var testCaseNode = solutionNode.SelectSingleNode($"//tr[@id='{testcaseId}']");
                var tempDocument = new HtmlDocument();
                tempDocument.LoadHtml(tCHtml);
                if (testCaseNode == null)
                    solutionNode.AppendChild(tempDocument.DocumentNode);

            }
            else
            {
                var systemInfoHTML = $"<table id='{solutionVersion}' width:100%  style='border-collapse:collapse; padding:3px; border-color:#c0c0c0; border-width: 1px; border-style:solid'>" +
                                     $"<thead style='background-color: #c0c0c0; padding:3px'>" +
                                     $"<tr>" +
                                     $"<th> {solutionVersion} </th>" +
                                     $"<th> First Seen </th>" +
                                     $"</tr></thead><tbody>" +
                                     tCHtml +
                                     "</tbody></table>" +
                                     "<br/>" +
                                     "<br/>";
                var tempHtmlDoc = new HtmlDocument();
                tempHtmlDoc.LoadHtml(systemInfoHTML);

                // Prepend to keep the new testcases on top
                if (systemHtmlDocument != null)
                    systemHtmlDocument.DocumentNode.PrependChild(tempHtmlDoc.DocumentNode);
                else
                    systemHtmlDocument = tempHtmlDoc;

            }

            return systemHtmlDocument.DocumentNode.OuterHtml;
        }
    }
}