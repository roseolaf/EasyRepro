using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Draeger.Dynamics365.Testautomation.Common;
using HtmlAgilityPack;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;
using TaADOLog.Logger.Sink;
using static TaADOLog.ADO.ADOManager;

namespace TaADOLog.ADO
{
    internal static class BugManager
    {
        private static readonly object workitemLock = new object();

        public static void CreateOrUpdateBug(int testCaseId, 
            List<ListSinkInfo<TestContext>> loggerSinkList, 
            Exception exception)
        {
            lock (workitemLock)
            {
                var testCase = WorkItems.GetWorkItem(testCaseId);

                // Relations can also be attachments
                List<int> relationsList = new List<int>();
                foreach (var relation in testCase.Relations)
                {
                    if (!int.TryParse(Regex.Match(relation.Url, @"\d+$").Value, out var result))
                        continue;
                    relationsList.Add(result);
                }

                var relatedWorkItems = WorkItems.GetWorkItems(relationsList);
                var featureWorkItem = relatedWorkItems.FirstOrDefault(x => x.Fields["System.WorkItemType"].ToString() == "Feature");
                var featureOwner = featureWorkItem != null ? featureWorkItem.Fields["System.AssignedTo"] : "";

                var vssConn = VssConnection();
                //create http client and query for results
                WorkItemTrackingHttpClient witClient = vssConn.GetClient<WorkItemTrackingHttpClient>();


                // TODO:  Save creds in key vault???
                var creds = new VssCredentials(new VssBasicCredential("AutomatedClient", personalAccessToken));
                //Prompt user for credential
                var connection = new TeamHttpClient(new Uri(ADOOrganizationUrl), creds);

                var allTeams = connection.GetAllTeamsAsync().Result;
                var project = allTeams.First(x => x.ProjectName == ADOProjectName);

                var teamMembers = connection.GetTeamMembersWithExtendedPropertiesAsync(project.ProjectId.ToString(), project.Id.ToString()).Result;
                // TODO Fail to feature owner
                var bugResponsible = teamMembers.First(x => x.Identity.UniqueName == ADOResponsibleEMail).Identity;


                var relationTypes = witClient.GetRelationTypesAsync().Result;

                var relationTestedBy = relationTypes.First(x => x.Name == "Tested By");

                var relationAttachedFile = relationTypes.First(x => x.Name == "Attached File");

                Wiql wiql = new Wiql()
                {
                    Query = "Select [Title] " +
                            "From WorkItems " +
                            "Where [Work Item Type] = 'Bug' " +
                            "And [System.TeamProject] = '" + ADOProjectName+ "' " +
                            "Order By [Changed Date] Desc"
                };

                WorkItemQueryResult workItemQueryResult = witClient.QueryByWiqlAsync(wiql).Result;
                List<int> workItemsList = new List<int>();
                foreach (var item in workItemQueryResult.WorkItems)
                {
                    workItemsList.Add(item.Id);
                }


                var workItems = WorkItems.GetWorkItems(workItemsList);

                var failedLine = loggerSinkList.First(s => s.State.Contains("Failed") || s.State.Contains("Error"));
                var actionFailedIndex = loggerSinkList.IndexOf(failedLine) - 1;


                var exceptionType = exception.InnerException != null
                    ? exception.InnerException.GetType().Name
                    : exception.GetType().Name;

                exceptionType = exceptionType.Replace("Exception", "");
                exceptionType = Regex.Replace(exceptionType, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

                var title = $"{exceptionType}: {loggerSinkList[actionFailedIndex].Message}";
                title = title.Length > 255 ? title.Substring(0, 255) : title;

                var removeStep = new Regex(@"Step\d+");
                var existingBug = workItems.FirstOrDefault(x => removeStep.Replace(x.Fields["System.Title"].ToString(), "") == removeStep.Replace(title, ""));

                if (existingBug != null)
                {
                    UpdateBug(testCaseId, loggerSinkList, testCase, witClient, project, bugResponsible, relationTestedBy, title, actionFailedIndex, existingBug);
                }
                else
                {
                    CreateBug(testCaseId, loggerSinkList, testCase, witClient, project, bugResponsible, relationTestedBy, title, actionFailedIndex);
                }

            }


        }

        private static void CreateBug(int testCaseId, List<ListSinkInfo<TestContext>> loggerSinkList, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex)
        {
            var jsonPatchDocument = new JsonPatchDocument();
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.Title",
                    Operation = Operation.Add,
                    Value = title,
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.AreaPath",
                    Operation = Operation.Add,
                    Value = testCase.Fields["System.AreaPath"],
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.WorkItemType",
                    Operation = Operation.Add,
                    Value = "Bug",
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.State",
                    Operation = Operation.Add,
                    Value = "Active",
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.AssignedTo",
                    Operation = Operation.Add,
                    Value = bugResponsible,
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/relations/-",
                    Operation = Operation.Add,
                    Value = new
                    {
                        Attributes = new Dictionary<string, object> { { "isLocked", false }, { "name", "Tested By" } },
                        Rel = relationTestedBy.ReferenceName,
                        Url = $"{ADOOrganizationUrl}/{project.ProjectId.ToString()}/_apis/wit/workItems/{testCaseId}"
                    },
                });



            var newSystemInfo = SystemInfoHTML(loggerSinkList);
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Microsoft.VSTS.TCM.SystemInfo",
                    Operation = Operation.Replace,
                    Value = newSystemInfo,
                });


            var solutionVersion = loggerSinkList[0].TestContext.Properties["SolutionVersion"];
            var foundInRelease = new Regex("(.+[1-9][0-9]*)").Match(solutionVersion.ToString()).Value;


            //jsonPatchDocument.Add(
            //    new JsonPatchOperation()
            //    {
            //        Path = "/fields/Custom.FoundInRelease",
            //        Operation = Operation.Replace,
            //        Value = foundInRelease,
            //    });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {

                    Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                    Operation = Operation.Add,
                    Value = LoggerToReproStepsHTML(loggerSinkList, witClient, actionFailedIndex, testCase)

                }); ;



            var result = witClient.CreateWorkItemAsync(jsonPatchDocument, project.ProjectId, "Bug").Result;
        }

        private static void UpdateBug(int testCaseId, List<ListSinkInfo<TestContext>> loggerSinkList,WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex, WorkItem existingBug)
        {
            var reproStepsHtml = LoggerToReproStepsHTML(loggerSinkList, witClient, actionFailedIndex, testCase);
            String newReproStepsHtml;

            // Repro Steps to add
            var htmlReproDoc = new HtmlDocument();
            htmlReproDoc.LoadHtml(reproStepsHtml);
            var newTestCaseNode = htmlReproDoc.DocumentNode.SelectSingleNode($"//div[@id=\"{testCaseId}\"]");

            // already existing ReproSteps from bug
            var oldReproSteps = existingBug.Fields["Microsoft.VSTS.TCM.ReproSteps"];
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(oldReproSteps.ToString());
            var oldTestCaseNode = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id=\"{testCaseId}\"]");

            // old Repro Steps does not contain steps for testcase
            if (oldTestCaseNode != null)
            {
                htmlDoc.DocumentNode.ReplaceChild(newTestCaseNode, oldTestCaseNode);
                newReproStepsHtml = htmlDoc.DocumentNode.OuterHtml;
            }
            else
            {
                // in case the bug exists without repro steps, handle it
                var parentNode = htmlDoc.DocumentNode;
                if (parentNode != null)
                {
                    parentNode.AppendChild(newTestCaseNode);
                    newReproStepsHtml = htmlDoc.DocumentNode.OuterHtml;
                }
                else
                    newReproStepsHtml = htmlReproDoc.DocumentNode.OuterHtml;

            }


            var solutionVersion = loggerSinkList[0].TestContext.Properties["SolutionVersion"];
            var foundInRelease = new Regex("(.+[1-9][0-9]*)").Match(solutionVersion.ToString()).Value;

            var oldSystemInfo = existingBug.Fields["Microsoft.VSTS.TCM.SystemInfo"].ToString();

            var newSystemInfo = SystemInfoHTML(loggerSinkList, oldSystemInfo);


            var addRelation = existingBug.Relations.Any(x => !x.Url.Contains(testCaseId.ToString()));
            var jsonPatchDocument = new JsonPatchDocument();
            //jsonPatchDocument.Add(
            //    new JsonPatchOperation()
            //    {
            //        Path = "/fields/Custom.FoundInRelease",
            //        Operation = Operation.Replace,
            //        Value = foundInRelease,
            //    });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Microsoft.VSTS.TCM.SystemInfo",
                    Operation = Operation.Replace,
                    Value = newSystemInfo,
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.State",
                    Operation = Operation.Replace,
                    Value = "Active",
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                    Operation = Operation.Replace,
                    Value = newReproStepsHtml

                });


            if (addRelation)
            {
                jsonPatchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Path = "/relations/-",
                        Operation = Operation.Add,
                        Value = new
                        {
                            Attributes = new Dictionary<string, object> { { "isLocked", false }, { "name", "Tested By" } },
                            Rel = relationTestedBy.ReferenceName,
                            Url = $"{ADOOrganizationUrl}/{project.ProjectId.ToString()}/_apis/wit/workItems/{testCaseId}"
                        },
                    });
            }

            var result = witClient.UpdateWorkItemAsync(jsonPatchDocument, existingBug.Id.Value).Result;
            workItemDict[existingBug.Id.Value] = result;
        }

        private static string LoggerToReproStepsHTML(List<ListSinkInfo<TestContext>> loggerSinkList, WorkItemTrackingHttpClient witClient, int actionFailedIndex, WorkItem testCase)
        {
            String reproStepsHTML;
            var testcaseInfo = loggerSinkList[0].Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Url)];
            var testcaseId = loggerSinkList[0].TestContext.Properties["TestCaseId"];
            var solutionVersion = loggerSinkList[0].TestContext.Properties["SolutionVersion"];
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
                text = text.Replace("&amp;nbsp;", Environment.NewLine);

                stepDict.Add(i, text);
                i++;
            }

            reproStepsHTML = $"<div id='{testcaseId}'>" +
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
                             $"<th> Level </th>" +
                             $"<th> Step </th>" +
                             $"<th> Description </th>" +
                             $"<th> Url </th>" +
                             $"<th> Screenshot </th>" +
                             $"</tr></thead><tbody>";

            AttachmentReference screenshotError = null;

            foreach (var log in loggerSinkList)
            {
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
                    FileStream attStream = new FileStream(log.Multimedia.ToString(), FileMode.Open, FileAccess.Read);
                    screenshot = witClient.CreateAttachmentAsync(attStream).Result; // upload file
                    attStream.Dispose();
                }
                var stepNrRegEx = new Regex(@"\d+");

                int actualStep = -1;
                if (!log.Step.IsNullOrEmpty())
                    actualStep = int.Parse(stepNrRegEx.Match(log.Step).Value);


                reproStepsHTML +=
                    $"<tr style=\"padding:10px\" >" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid \">" +
                    $"<i>{log.DateTime:dd/MM/yyyy HH:mm:ss.fff}</i></td>" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                    $"{log.Level}</td>" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\"" + (!log.Step.IsNullOrEmpty() ? $"title=\"'{stepDict[actualStep]}'\">" : ">") +
                    $"{log.Step}</td>" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                    (loggerSinkList.IndexOf(log) == actionFailedIndex ? $"❌ {logmsgModified}</td>" : $"{logmsgModified}</td>") +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                    (log.Url.IsNullOrEmpty() ? "</td>" : $"<a href=\"{log.Url}\" target=\"_blank\">🌍</a></td>") +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                    (screenshot == null ? $"</td>" : $"<a href=\"{screenshot.Url}\" target=\"_blank\"><img src=\"{screenshot.Url}\"></a></td>") +
                    $"</tr>";

                if (loggerSinkList.IndexOf(log) == actionFailedIndex && screenshot != null)
                {
                    screenshotError = screenshot;
                }
                


            }
            reproStepsHTML += "</tbody></table>" +
                              "<br/>" +
                              "<br/>" +
                              $"<img src=\"{screenshotError?.Url}\">" +
                              "</div>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>";

            return reproStepsHTML;
        }

        private static string SystemInfoHTML(List<ListSinkInfo<TestContext>> loggerSinkList, string oldSystemInfo = null)
        {
            var testcaseInfo = loggerSinkList[0].Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Url)];
            var testcaseId = loggerSinkList[0].TestContext.Properties["TestCaseId"];
            var solutionVersion = loggerSinkList[0].TestContext.Properties["SolutionVersion"];
            var foundDateTime = loggerSinkList.First().DateTime;

            HtmlNode solutionNode = null;
            HtmlDocument systemHtmlDocument = null;

            if (!oldSystemInfo.IsNullOrEmpty())
            {
                systemHtmlDocument = new HtmlDocument();
                systemHtmlDocument.LoadHtml(oldSystemInfo);

                solutionNode = systemHtmlDocument.DocumentNode.SelectSingleNode($"//table[@id='{solutionVersion}']/tbody");
            }

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

                if (systemHtmlDocument != null)
                    systemHtmlDocument.DocumentNode.AppendChild(tempHtmlDoc.DocumentNode);
                else
                    systemHtmlDocument = tempHtmlDoc;

            }

            return systemHtmlDocument.DocumentNode.OuterHtml;
        }




        public static void CreateOrUpdateBug(int testCaseId,
            List<ListSinkInfo<IDictionary<string, object>>> loggerSinkList,
            Exception exception)
        {
            lock (workitemLock)
            {
                var testCase = WorkItems.GetWorkItem(testCaseId);

                // Relations can also be attachments
                List<int> relationsList = new List<int>();
                foreach (var relation in testCase.Relations)
                {
                    if (!int.TryParse(Regex.Match(relation.Url, @"\d+$").Value, out var result))
                        continue;
                    relationsList.Add(result);
                }

                var relatedWorkItems = WorkItems.GetWorkItems(relationsList);
                var featureWorkItem = relatedWorkItems.FirstOrDefault(x => x.Fields["System.WorkItemType"].ToString() == "Feature");
                var featureOwner = featureWorkItem != null ? featureWorkItem.Fields["System.AssignedTo"] : "";

                var vssConn = VssConnection();
                //create http client and query for results
                WorkItemTrackingHttpClient witClient = vssConn.GetClient<WorkItemTrackingHttpClient>();


                // TODO:  Save creds in key vault???
                var creds = new VssCredentials(new VssBasicCredential("AutomatedClient", personalAccessToken));
                //Prompt user for credential
                var connection = new TeamHttpClient(new Uri(ADOOrganizationUrl), creds);

                var allTeams = connection.GetAllTeamsAsync().Result;
                var project = allTeams.First(x => x.ProjectName == ADOProjectName);

                var teamMembers = connection.GetTeamMembersWithExtendedPropertiesAsync(project.ProjectId.ToString(), project.Id.ToString()).Result;
                // TODO Fail to feature owner
                var bugResponsible = teamMembers.First(x => x.Identity.UniqueName == ADOResponsibleEMail).Identity;


                var relationTypes = witClient.GetRelationTypesAsync().Result;

                var relationTestedBy = relationTypes.First(x => x.Name == "Tested By");

                var relationAttachedFile = relationTypes.First(x => x.Name == "Attached File");

                Wiql wiql = new Wiql()
                {
                    Query = "Select [Title] " +
                            "From WorkItems " +
                            "Where [Work Item Type] = 'Bug' " +
                            "And [System.TeamProject] = '" + ADOProjectName + "' " +
                            "Order By [Changed Date] Desc"
                };

                WorkItemQueryResult workItemQueryResult = witClient.QueryByWiqlAsync(wiql).Result;
                List<int> workItemsList = new List<int>();
                foreach (var item in workItemQueryResult.WorkItems)
                {
                    workItemsList.Add(item.Id);
                }


                var workItems = WorkItems.GetWorkItems(workItemsList);

                var failedLine = loggerSinkList.First(s => s.State.Contains("Failed") || s.State.Contains("Error"));
                var actionFailedIndex = loggerSinkList.IndexOf(failedLine) - 1;


                var exceptionType = exception.InnerException != null
                    ? exception.InnerException.GetType().Name
                    : exception.GetType().Name;

                exceptionType = exceptionType.Replace("Exception", "");
                exceptionType = Regex.Replace(exceptionType, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

                var title = $"{exceptionType}: {loggerSinkList[actionFailedIndex].Message}";
                title = title.Length > 255 ? title.Substring(0, 255) : title;

                var removeStep = new Regex(@"Step\d+");
                var existingBug = workItems.FirstOrDefault(x => removeStep.Replace(x.Fields["System.Title"].ToString(), "") == removeStep.Replace(title, ""));

                if (existingBug != null)
                {
                    UpdateBug(testCaseId, loggerSinkList, testCase, witClient, project, bugResponsible, relationTestedBy, title, actionFailedIndex, existingBug);
                }
                else
                {
                    CreateBug(testCaseId, loggerSinkList, testCase, witClient, project, bugResponsible, relationTestedBy, title, actionFailedIndex);
                }

            }


        }

        private static void CreateBug(int testCaseId, List<ListSinkInfo<IDictionary<string, object>>> loggerSinkList, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex)
        {
            var jsonPatchDocument = new JsonPatchDocument();
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.Title",
                    Operation = Operation.Add,
                    Value = title,
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.AreaPath",
                    Operation = Operation.Add,
                    Value = testCase.Fields["System.AreaPath"],
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.WorkItemType",
                    Operation = Operation.Add,
                    Value = "Bug",
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.State",
                    Operation = Operation.Add,
                    Value = "Active",
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.AssignedTo",
                    Operation = Operation.Add,
                    Value = bugResponsible,
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/relations/-",
                    Operation = Operation.Add,
                    Value = new
                    {
                        Attributes = new Dictionary<string, object> { { "isLocked", false }, { "name", "Tested By" } },
                        Rel = relationTestedBy.ReferenceName,
                        Url = $"{ADOOrganizationUrl}/{project.ProjectId.ToString()}/_apis/wit/workItems/{testCaseId}"
                    },
                });



            var newSystemInfo = SystemInfoHTML(loggerSinkList);
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Microsoft.VSTS.TCM.SystemInfo",
                    Operation = Operation.Replace,
                    Value = newSystemInfo,
                });


            var solutionVersion = loggerSinkList[0].TestContext["SolutionVersion"];
            var foundInRelease = new Regex("(.+[1-9][0-9]*)").Match(solutionVersion.ToString()).Value;


            //jsonPatchDocument.Add(
            //    new JsonPatchOperation()
            //    {
            //        Path = "/fields/Custom.FoundInRelease",
            //        Operation = Operation.Replace,
            //        Value = foundInRelease,
            //    });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {

                    Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                    Operation = Operation.Add,
                    Value = LoggerToReproStepsHTML(loggerSinkList, witClient, actionFailedIndex, testCase)

                }); ;



            var result = witClient.CreateWorkItemAsync(jsonPatchDocument, project.ProjectId, "Bug").Result;
        }

        private static void UpdateBug(int testCaseId, List<ListSinkInfo<IDictionary<string, object>>> loggerSinkList, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex, WorkItem existingBug)
        {
            var reproStepsHtml = LoggerToReproStepsHTML(loggerSinkList, witClient, actionFailedIndex, testCase);
            String newReproStepsHtml;

            // Repro Steps to add
            var htmlReproDoc = new HtmlDocument();
            htmlReproDoc.LoadHtml(reproStepsHtml);
            var newTestCaseNode = htmlReproDoc.DocumentNode.SelectSingleNode($"//div[@id=\"{testCaseId}\"]");

            // already existing ReproSteps from bug
            var oldReproSteps = existingBug.Fields["Microsoft.VSTS.TCM.ReproSteps"];
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(oldReproSteps.ToString());
            var oldTestCaseNode = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id=\"{testCaseId}\"]");

            // old Repro Steps does not contain steps for testcase
            if (oldTestCaseNode != null)
            {
                htmlDoc.DocumentNode.ReplaceChild(newTestCaseNode, oldTestCaseNode);
                newReproStepsHtml = htmlDoc.DocumentNode.OuterHtml;
            }
            else
            {
                // in case the bug exists without repro steps, handle it
                var parentNode = htmlDoc.DocumentNode;
                if (parentNode != null)
                {
                    parentNode.AppendChild(newTestCaseNode);
                    newReproStepsHtml = htmlDoc.DocumentNode.OuterHtml;
                }
                else
                    newReproStepsHtml = htmlReproDoc.DocumentNode.OuterHtml;

            }


            var solutionVersion = loggerSinkList[0].TestContext["SolutionVersion"];
            var foundInRelease = new Regex("(.+[1-9][0-9]*)").Match(solutionVersion.ToString()).Value;

            var oldSystemInfo = existingBug.Fields["Microsoft.VSTS.TCM.SystemInfo"].ToString();

            var newSystemInfo = SystemInfoHTML(loggerSinkList, oldSystemInfo);


            var addRelation = existingBug.Relations.Any(x => !x.Url.Contains(testCaseId.ToString()));
            var jsonPatchDocument = new JsonPatchDocument();
            //jsonPatchDocument.Add(
            //    new JsonPatchOperation()
            //    {
            //        Path = "/fields/Custom.FoundInRelease",
            //        Operation = Operation.Replace,
            //        Value = foundInRelease,
            //    });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Microsoft.VSTS.TCM.SystemInfo",
                    Operation = Operation.Replace,
                    Value = newSystemInfo,
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.State",
                    Operation = Operation.Replace,
                    Value = "Active",
                });
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                    Operation = Operation.Replace,
                    Value = newReproStepsHtml

                });


            if (addRelation)
            {
                jsonPatchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Path = "/relations/-",
                        Operation = Operation.Add,
                        Value = new
                        {
                            Attributes = new Dictionary<string, object> { { "isLocked", false }, { "name", "Tested By" } },
                            Rel = relationTestedBy.ReferenceName,
                            Url = $"{ADOOrganizationUrl}/{project.ProjectId.ToString()}/_apis/wit/workItems/{testCaseId}"
                        },
                    });
            }

            var result = witClient.UpdateWorkItemAsync(jsonPatchDocument, existingBug.Id.Value).Result;
            workItemDict[existingBug.Id.Value] = result;
        }

        private static string LoggerToReproStepsHTML(List<ListSinkInfo<IDictionary<string, object>>> loggerSinkList, WorkItemTrackingHttpClient witClient, int actionFailedIndex, WorkItem testCase)
        {
            String reproStepsHTML;
            var testcaseInfo = loggerSinkList[0].Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Url)];
            var testcaseId = loggerSinkList[0].TestContext["TestCaseId"];
            var solutionVersion = loggerSinkList[0].TestContext["SolutionVersion"];
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
                text = text.Replace("&amp;nbsp;", Environment.NewLine);

                stepDict.Add(i, text);
                i++;
            }

            reproStepsHTML = $"<div id='{testcaseId}'>" +
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
                             $"<th> Level </th>" +
                             $"<th> Step </th>" +
                             $"<th> Description </th>" +
                             $"<th> Url </th>" +
                             $"<th> Screenshot </th>" +
                             $"</tr></thead><tbody>";

            List<AttachmentReference> screenshotError = new List<AttachmentReference>();

            foreach (var log in loggerSinkList)
            {

                AttachmentReference screenshotBeforeInvoke = null;
                if (log.LogEvent.Level == LogEventLevel.Verbose && log.Message.Equals("Screenshot before invoke"))
                {

                    if (!log.Multimedia.ToString().IsNullOrEmpty())
                    {
                        FileStream attStream = new FileStream(log.Multimedia.ToString(), FileMode.Open, FileAccess.Read);
                        screenshotBeforeInvoke = witClient.CreateAttachmentAsync(attStream).Result; // upload file
                        attStream.Dispose();
                    }

                }

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
                    FileStream attStream = new FileStream(log.Multimedia.ToString(), FileMode.Open, FileAccess.Read);
                    screenshot = witClient.CreateAttachmentAsync(attStream).Result; // upload file
                    attStream.Dispose();
                }
                var stepNrRegEx = new Regex(@"\d+");

                int actualStep = -1;
                if (!log.Step.IsNullOrEmpty())
                    actualStep = int.Parse(stepNrRegEx.Match(log.Step).Value);


                reproStepsHTML +=
                    $"<tr style=\"padding:10px\" >" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid \">" +
                    $"<i>{log.DateTime:dd/MM/yyyy HH:mm:ss.fff}</i></td>" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                    $"{log.Level}</td>" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\"" +
                    (!log.Step.IsNullOrEmpty() ? $"title=\"'{stepDict[actualStep]}'\">" : ">") +
                    $"{log.Step}</td>" +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                    ((log.Level == LogEventLevel.Error.ToString() || log.Level == LogEventLevel.Fatal.ToString())
                        ? $"❌ {logmsgModified}</td>"
                        : $"{logmsgModified}</td>") +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">" +
                    (log.Url.IsNullOrEmpty() ? "</td>" : $"<a href=\"{log.Url}\" target=\"_blank\">🌍</a></td>") +
                    $"<td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">";
                if (screenshotBeforeInvoke != null)
                {
                    reproStepsHTML +=
                        $"<a href=\"{screenshotBeforeInvoke.Url}\" target=\"_blank\"><img src=\"{screenshotBeforeInvoke.Url}\"></a><br>";
                }

                if (screenshot != null)
                {
                    reproStepsHTML +=
                        $"<a href=\"{screenshot.Url}\" target=\"_blank\"><img src=\"{screenshot.Url}\"></a>";
                }
                reproStepsHTML += "</td>"+
                                $"</tr>";

                if ((log.Level == LogEventLevel.Error.ToString() || log.Level == LogEventLevel.Fatal.ToString()) && screenshot != null)
                {
                    screenshotError.Add(screenshot);
                }



            }

            reproStepsHTML += "</tbody></table>" +
                              "<br/>" +
                              "<br/>;";

            foreach (var sE in screenshotError)
            {
                reproStepsHTML += $"<img src=\"{sE?.Url}\">";
            }
            reproStepsHTML += "</div>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>";

            return reproStepsHTML;
        }

        private static string SystemInfoHTML(List<ListSinkInfo<IDictionary<string, object>>> loggerSinkList, string oldSystemInfo = null)
        {
            var testcaseInfo = loggerSinkList[0].Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(PropertyKeys.Url)];
            var testcaseId = loggerSinkList[0].TestContext["TestCaseId"];
            var solutionVersion = loggerSinkList[0].TestContext["SolutionVersion"];
            var foundDateTime = loggerSinkList.First().DateTime;

            HtmlNode solutionNode = null;
            HtmlDocument systemHtmlDocument = null;

            if (!oldSystemInfo.IsNullOrEmpty())
            {
                systemHtmlDocument = new HtmlDocument();
                systemHtmlDocument.LoadHtml(oldSystemInfo);

                solutionNode = systemHtmlDocument.DocumentNode.SelectSingleNode($"//table[@id='{solutionVersion}']/tbody");
            }

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

                if (systemHtmlDocument != null)
                    systemHtmlDocument.DocumentNode.AppendChild(tempHtmlDoc.DocumentNode);
                else
                    systemHtmlDocument = tempHtmlDoc;

            }

            return systemHtmlDocument.DocumentNode.OuterHtml;
        }
    }
}