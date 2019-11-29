using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Users.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.TeamFoundation.Common;
using Serilog.Events;

namespace Draeger.Dynamics365.Testautomation.Common.Helper
{
    public static class WorkItems
    {
        private static Dictionary<int, WorkItem> workItemDict = new Dictionary<int, WorkItem>();
        public const string azureDevOpsOrganizationUrl = "https://dev.azure.com/draeger";
        public const string azureDevOpsProjectName = "CRM";
        private static readonly object Lock = new object();
        private static readonly object workitemLock = new object();
        private const string pat = "6opssjduepken5jciswfhpsywohwyng3lfzcr5tbtti54vilfjya";

        private static VssConnection NewVssConnection()
        {
            lock (Lock)
            {
                // TODO:  Save creds in key vault???
                var creds = new VssCredentials(new VssBasicCredential("AutomatedClient", pat));
                //Prompt user for credential
                VssConnection connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), creds);
                //create http client and query for results
                WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
                return connection;
            }
        }

        public static WorkItem FetchWorkItemData(int id)
        {
            //create http client and query for results
            WorkItemTrackingHttpClient witClient = NewVssConnection().GetClient<WorkItemTrackingHttpClient>();
            //get work item for the id found in query
            var result = witClient.GetWorkItemAsync(id, expand: WorkItemExpand.All).Result;
            lock (workItemDict)
            {
                workItemDict[id] = result;
                return result;
            }

        }

        public static List<WorkItem> FetchWorkItemsData(List<int> ids)
        {
            //create http client and query for results
            WorkItemTrackingHttpClient witClient = NewVssConnection().GetClient<WorkItemTrackingHttpClient>();
            //get work items for the ids found in query
            var result = witClient.GetWorkItemsAsync(ids, expand: WorkItemExpand.All, errorPolicy: WorkItemErrorPolicy.Omit).Result;
            foreach (var res in result)
            {
                lock (workItemDict)
                {
                    if (res != null)
                        workItemDict[res.Id.Value] = res;
                }
            }
            return result;
        }

        public static List<WorkItem> GetWorkItems(List<int> ids)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            lock (workItemDict)
            {
                if (ids.Any()
                    && ids.All(key => workItemDict.ContainsKey(key)))
                    return workItemDict.Where(x => ids.Contains(x.Key)).Select(x => x.Value).ToList();
            }

            ids.RemoveAll(x => x == 0);

            for (int i = 0; i < ids.Count; i += 200)
            {
                if (ids.Count - i < 200)
                {
                    var modI = ids.Count % 200;
                    var tempList = ids.Skip(i).Take(modI).ToList();
                    FetchWorkItemsData(tempList);
                }
                else
                {
                    var tempList = ids.Skip(i).Take(200).ToList();
                    FetchWorkItemsData(tempList);
                }

            }


            lock (workItemDict)
            {
                return workItemDict.Where(x => ids.Contains(x.Key)).Select(x => x.Value).ToList();
            }
        }

        public static WorkItem GetWorkItem(int id)
        {
            lock (workItemDict)
            {
                return !workItemDict.ContainsKey(id) ? FetchWorkItemData(id) : workItemDict[id];
            }
        }

        public static void CreateOrUpdateBug(int testCaseId, List<ListSinkInfo> loggerSinkList, Exception exception, string screenshotPath = "")
        {
            lock (workitemLock)
            {

                var testCase = GetWorkItem(testCaseId);

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

                var vssConn = NewVssConnection();
                //create http client and query for results
                WorkItemTrackingHttpClient witClient = vssConn.GetClient<WorkItemTrackingHttpClient>();


                // TODO:  Save creds in key vault???
                var creds = new VssCredentials(new VssBasicCredential("AutomatedClient", pat));
                //Prompt user for credential
                var connection = new TeamHttpClient(new Uri(azureDevOpsOrganizationUrl), creds);

                var allTeams = connection.GetAllTeamsAsync().Result;
                var project = allTeams.First(x => x.ProjectName == azureDevOpsProjectName);

                var teamMembers = connection.GetTeamMembersWithExtendedPropertiesAsync(project.ProjectId.ToString(), project.Id.ToString()).Result;
                // TODO Fail to feature owner
                var bugResponsible = teamMembers.First(x => x.Identity.UniqueName == "malte.fries@draeger.com").Identity;


                var relationTypes = witClient.GetRelationTypesAsync().Result;

                var relationTestedBy = relationTypes.First(x => x.Name == "Tested By");

                var relationAttachedFile = relationTypes.First(x => x.Name == "Attached File");

                Wiql wiql = new Wiql()
                {
                    Query = "Select [Title] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        "And [System.TeamProject] = '" + azureDevOpsProjectName + "' " +
                        "Order By [Changed Date] Desc"
                };

                WorkItemQueryResult workItemQueryResult = witClient.QueryByWiqlAsync(wiql).Result;
                List<int> workItemsList = new List<int>();
                foreach (var item in workItemQueryResult.WorkItems)
                {
                    workItemsList.Add(item.Id);
                }


                var workItems = GetWorkItems(workItemsList);

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
                    UpdateBug(testCaseId, loggerSinkList, screenshotPath, testCase, witClient, project, bugResponsible, relationTestedBy, title, actionFailedIndex, existingBug);
                }
                else
                {
                    CreateBug(testCaseId, loggerSinkList, screenshotPath, testCase, witClient, project, bugResponsible, relationTestedBy, title, actionFailedIndex);
                }

            }


        }

        private static void CreateBug(int testCaseId, List<ListSinkInfo> loggerSinkList, string screenshotPath, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex)
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
                      Url = $"{azureDevOpsOrganizationUrl}/{project.ProjectId.ToString()}/_apis/wit/workItems/{testCaseId}"
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


            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Custom.FoundInRelease",
                    Operation = Operation.Replace,
                    Value = foundInRelease,
                });
            FileStream attStream = new FileStream(screenshotPath, FileMode.Open, FileAccess.Read);
            var screenshotFullPage = witClient.CreateAttachmentAsync(attStream).Result; // upload the file
            attStream.Dispose();
            jsonPatchDocument.Add(
                     new JsonPatchOperation()
                     {

                         Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                         Operation = Operation.Add,
                         Value = LoggerToReproStepsHTML(loggerSinkList, witClient, screenshotFullPage, actionFailedIndex, testCase)

                     }); ;



            var result = witClient.CreateWorkItemAsync(jsonPatchDocument, project.ProjectId, "Bug").Result;
        }

        private static void UpdateBug(int testCaseId, List<ListSinkInfo> loggerSinkList, string screenshotPath, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex, WorkItem existingBug)
        {


            // Full page screenshot
            FileStream attStream = new FileStream(screenshotPath, FileMode.Open, FileAccess.Read);
            var screenshotFullPage = witClient.CreateAttachmentAsync(attStream).Result; // upload file
            attStream.Dispose();
            var reproStepsHtml = LoggerToReproStepsHTML(loggerSinkList, witClient, screenshotFullPage, actionFailedIndex, testCase);
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
                    Path = "/fields/Custom.FoundInRelease",
                    Operation = Operation.Replace,
                    Value = foundInRelease,
                });
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
                    Path = "/fields/System.AreaPath",
                    Operation = Operation.Replace,
                    Value = testCase.Fields["System.AreaPath"],
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
                     Path = "/fields/System.AssignedTo",
                     Operation = Operation.Replace,
                     Value = bugResponsible,
                 });
            jsonPatchDocument.Add(
                 new JsonPatchOperation()
                 {

                     Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                     Operation = Operation.Add,
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
                            Url = $"{azureDevOpsOrganizationUrl}/{project.ProjectId.ToString()}/_apis/wit/workItems/{testCaseId}"
                        },
                    });
            }

            var result = witClient.UpdateWorkItemAsync(jsonPatchDocument, existingBug.Id.Value).Result;
            workItemDict[existingBug.Id.Value] = result;
        }

        private static string LoggerToReproStepsHTML(List<ListSinkInfo> loggerSinkList, WorkItemTrackingHttpClient witClient, AttachmentReference screenshotFullPage, int actionFailedIndex, WorkItem testCase)
        {
            String reproStepsHTML;
            var testcaseInfo = loggerSinkList[0].Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(DevOpsPropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(DevOpsPropertyKeys.Url)];
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

            foreach (var log in loggerSinkList)
            {
                var logmsgModified = log.Message.Replace("\r\n", "<br>");
                if (log.EntityUrl.IsNullOrEmpty())
                {
                    var htmlLink = new Regex(@"(http[^\s""']+)");
                    var linkValue = htmlLink.Match(logmsgModified).Value;
                    logmsgModified = htmlLink.Replace(logmsgModified, $"<a href=\"{linkValue}\">{linkValue}</a>");
                }

                AttachmentReference screenshot = null;

                if (!log.Screenshot.ToString().IsNullOrEmpty())
                {
                    FileStream attStream = new FileStream(log.Screenshot.ToString(), FileMode.Open, FileAccess.Read);
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



            }
            reproStepsHTML += "</tbody></table>" +
                              "<br/>" +
                              "<br/>" +
                              $"<img src=\"{screenshotFullPage.Url}\">" +
                              "</div>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>" +
                              "<br/>";

            return reproStepsHTML;
        }

        private static string SystemInfoHTML(List<ListSinkInfo> loggerSinkList, string oldSystemInfo = null)
        {
            var testcaseInfo = loggerSinkList[0].Properties["TestCaseInfo"] as DictionaryValue;
            var title = testcaseInfo.Elements[new ScalarValue(DevOpsPropertyKeys.Title)];
            var url = testcaseInfo.Elements[new ScalarValue(DevOpsPropertyKeys.Url)];
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



    }
}
