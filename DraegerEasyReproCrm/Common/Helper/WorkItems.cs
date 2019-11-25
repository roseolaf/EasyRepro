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
using System.Threading.Tasks;

namespace Draeger.Dynamics365.Testautomation.Common.Helper
{
    public static class WorkItems
    {
        private static Dictionary<int, WorkItem> workItemDict = new Dictionary<int, WorkItem>();
        public const string azureDevOpsOrganizationUrl = "https://dev.azure.com/draeger";
        public const string azureDevOpsProjectName = "CRM";
        private static readonly object Lock = new object();
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

        public static void CreateOrUpdateBug(int testCaseId, List<string> loggerSinkList, Exception exception, string screenshotPath = "")
        {
            var testCase = GetWorkItem(testCaseId);

            List<int> relationsList = new List<int>();
            foreach (var relation in testCase.Relations)
            {
                relationsList.Add(int.Parse(Regex.Match(relation.Url, @"\d+$").Value));
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

            // TODO ProjectName as const?
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

            var rxTime = new Regex(regxTimeAndLogType);
            var rxSteps = new Regex(regxStepsAndMessage);
            var rxMessage = new Regex(regxTimeAndLogTypeAndMessage);

            var failedLine = loggerSinkList.Find(s => s.Contains("Failed"));
            var failedLineIndex = loggerSinkList.IndexOf(failedLine);
            var step = "";
            var msg = "";
            var time = "";
            var logType = "";
            var logmsgModified = loggerSinkList[failedLineIndex - 1].Replace("\r\n", " ");
            if (rxMessage.IsMatch(logmsgModified))
            {
                var match = rxMessage.Match(logmsgModified);
                time = match.Groups[1].Value;
                logType = match.Groups[2].Value;
                msg = match.Groups[3].Value;

            }
            else
            {
                var match = rxTime.Match(logmsgModified);
                time = match.Groups[1].Value;
                logType = match.Groups[2].Value;
            }

            if (rxSteps.IsMatch(logmsgModified))
            {
                var match = rxSteps.Match(logmsgModified);
                step = match.Groups[1].Value;
                msg = match.Groups[2].Value;
            }


            var exceptionType = exception.InnerException != null
                ? exception.InnerException.GetType().Name
                : exception.GetType().Name;

            exceptionType = exceptionType.Replace("Exception", "");
            exceptionType = Regex.Replace(exceptionType, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

            var title = $"{exceptionType}: {step} - {msg}";
            title = title.Length > 255 ? title.Substring(0, 255) : title;
            var existingBug = workItems.FirstOrDefault(x => x.Fields["System.Title"].ToString() == title);

            if (existingBug != null)
            {
                UpdateBug(testCaseId, loggerSinkList, screenshotPath, testCase, witClient, project, bugResponsible, relationTestedBy, existingBug);
            }
            else
            {
                CreateBug(testCaseId, loggerSinkList, screenshotPath, testCase, witClient, project, bugResponsible, relationTestedBy, title);
            }



        }

        private static void CreateBug(int testCaseId, List<string> loggerSinkList, string screenshotPath, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title)
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
            if (screenshotPath != "")
            {
                using (FileStream attStream = new FileStream(screenshotPath, FileMode.Open, FileAccess.Read))
                {
                    var att = witClient.CreateAttachmentAsync(attStream).Result; // upload the file
                    jsonPatchDocument.Add(
                     new JsonPatchOperation()
                     {

                         Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                         Operation = Operation.Add,
                         Value = LoggerToReproStepsHTML(loggerSinkList, att)

                     }); ;
                }
            }
            else
            {
                jsonPatchDocument.Add(
               new JsonPatchOperation()
               {
                   Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                   Operation = Operation.Add,
                   Value = LoggerToReproStepsHTML(loggerSinkList),
               });
            }

            var result = witClient.CreateWorkItemAsync(jsonPatchDocument, project.ProjectId, "Bug").Result;
        }

        private static void UpdateBug(int testCaseId, List<string> loggerSinkList, string screenshotPath, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, WorkItem existingBug)
        {
            var addRelation = existingBug.Relations.Any(x => !x.Url.Contains(testCaseId.ToString()));
            var jsonPatchDocument = new JsonPatchDocument();
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
            if (screenshotPath != "")
            {
                using (FileStream attStream = new FileStream(screenshotPath, FileMode.Open, FileAccess.Read))
                {
                    var att = witClient.CreateAttachmentAsync(attStream).Result; // upload the file
                    jsonPatchDocument.Add(
                     new JsonPatchOperation()
                     {

                         Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                         Operation = Operation.Add,
                         Value = LoggerToReproStepsHTML(loggerSinkList, att)

                     }); ;
                }
            }
            else
            {
                jsonPatchDocument.Add(
               new JsonPatchOperation()
               {
                   Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                   Operation = Operation.Add,
                   Value = LoggerToReproStepsHTML(loggerSinkList),
               });
            }


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
        }

        private const string regxTimeAndLogType = @"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}.+\d\s)(?=\[)\[(\w+)";
        private const string regxStepsAndMessage = @"(Step\d+):(.+)";
        private const string regxTimeAndLogTypeAndMessage = @"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}.+\d\s)(?=\[)\[(\w+)\](.+)";
        private static string LoggerToReproStepsHTML(List<string> loggerSinkList)
        {


            String reproStepsHTML = "<table width:100%  style='border-collapse:collapse; padding:3px; border-color:#c0c0c0; border-width: 1px; border-style:solid'><thead style='background-color: #c0c0c0; padding:3px'><tr><th> Time </th><th> LogType </th><th> Step </th><th> Message </th></tr></thead><tbody>";

            var rxTime = new Regex(regxTimeAndLogType);
            var rxSteps = new Regex(regxStepsAndMessage);
            var rxMessage = new Regex(regxTimeAndLogTypeAndMessage);


            foreach (var logmsg in loggerSinkList)
            {
                var step = "";
                var msg = "";
                var time = "";
                var logType = "";
                var logmsgModified = logmsg.Replace("\r\n", "<br>");



                if (rxMessage.IsMatch(logmsgModified))
                {
                    var match = rxMessage.Match(logmsgModified);
                    time = match.Groups[1].Value;
                    logType = match.Groups[2].Value;
                    msg = match.Groups[3].Value;

                }
                else
                {
                    var match = rxTime.Match(logmsgModified);
                    time = match.Groups[1].Value;
                    logType = match.Groups[2].Value;
                }
                if (rxSteps.IsMatch(logmsgModified))
                {
                    var match = rxSteps.Match(logmsgModified);
                    step = match.Groups[1].Value;
                    msg = match.Groups[2].Value;
                }

                reproStepsHTML += $"<tr style=\"padding:10px\" ><td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid \"><i>{time}</i></td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\"><td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">{logType}</td><td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">{step}</td><td style=\"padding:10px;border-color:#c0c0c0; border-width: 1px; border-style:solid\">{msg}</td></tr>";
            }

            reproStepsHTML += "</tbody></table>";


            return reproStepsHTML;
        }

        private static string LoggerToReproStepsHTML(List<string> loggerSinkList, AttachmentReference screenshotUrl)
        {
            return LoggerToReproStepsHTML(loggerSinkList) + $"<br><br><img src=\"{screenshotUrl.Url}\">";
        }
    }
}
