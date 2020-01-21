using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Draeger.Dynamics365.Testautomation.Common;
using HtmlAgilityPack;
using ImageMagick;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaADOLog.ADO.HTML;
using TaADOLog.Logger;
using TaADOLog.Logger.Sink;
using static TaADOLog.ADO.ADOManager;

namespace TaADOLog.ADO
{
    internal static class BugManager
    {
        private static readonly object workitemLock = new object();

        public static void CreateOrUpdateBug<T>(int testCaseId,
            List<ListSinkInfo<T>> loggerSinkList,
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

        private static void CreateBug<T>(int testCaseId, List<ListSinkInfo<T>> loggerSinkList, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex)
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



            var newSystemInfo = SystemInfo.GenerateHtml(loggerSinkList);
            jsonPatchDocument.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/Microsoft.VSTS.TCM.SystemInfo",
                    Operation = Operation.Replace,
                    Value = newSystemInfo,
                });


            var solutionVersion = GetSolutionVersion(loggerSinkList);
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
                    Value = ReproSteps.GenerateHtml(loggerSinkList, witClient, actionFailedIndex, testCase)

                }); ;



            var result = witClient.CreateWorkItemAsync(jsonPatchDocument, project.ProjectId, "Bug").Result;
            workItemDict[result.Id.Value] = result;
        }

        private static void UpdateBug<T>(int testCaseId, List<ListSinkInfo<T>> loggerSinkList, WorkItem testCase, WorkItemTrackingHttpClient witClient, WebApiTeam project, IdentityRef bugResponsible, WorkItemRelationType relationTestedBy, string title, int actionFailedIndex, WorkItem existingBug)
        {
            var reproStepsHtml = ReproSteps.GenerateHtml(loggerSinkList, witClient, actionFailedIndex, testCase);
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


            var solutionVersion = GetSolutionVersion(loggerSinkList);
            var foundInRelease = new Regex("(.+[1-9][0-9]*)").Match(solutionVersion.ToString()).Value;

            var oldSystemInfo = existingBug.Fields["Microsoft.VSTS.TCM.SystemInfo"].ToString();

            var newSystemInfo = SystemInfo.GenerateHtml(loggerSinkList, oldSystemInfo);


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
        
        internal static string GetSolutionVersion<T>(List<ListSinkInfo<T>> loggerSinkList)
        {
            string solutionVersion = null;
            if (loggerSinkList is List<ListSinkInfo<TestContext>> tc)
            {
                solutionVersion = tc.FirstOrDefault(logInfo => logInfo.TestContext.Properties.Contains("SolutionVersion"))?.TestContext
                    .Properties["SolutionVersion"].ToString();
            }

            if (loggerSinkList is List<ListSinkInfo<IDictionary<string, object>>> dict)

            {
                solutionVersion = dict.FirstOrDefault().TestContext["SolutionVersion"].ToString();

            }

            return solutionVersion;

        }
    }


}

