using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using Draeger.Dynamics365.Testautomation.Common.Helper;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public static class DevOpsPropertyKeys
    {
        public static string Area = "Area";
        public static string TeamProject = "TeamProject";
        public static string Sprint = "Sprint";
        public static string State = "State";
        public static string WorkItemType = "WorkItemType";
        public static string AssignedTo = "AssignedTo";
        public static string Id = "Id";
        public static string Title = "Title";
        public static string Priority = "Priority";
        public static string Url = "Url";
    }

    public static class TestSuiteIdentifier
    {
        public static Guid TestSuiteGuid = Guid.NewGuid();
        public static DateTime TestSuiteStartTime = DateTime.Now;
    }

    public class DevOpsConnector
    {
        
        private string UrlBuilder(string teamProject, string workItemId)
        {
            return $"{WorkItems.azureDevOpsOrganizationUrl}/{teamProject}/_workitems/edit/{workItemId}";
        }


        public Dictionary<ScalarValue, LogEventPropertyValue> GetTestCaseEnrichInfo(int id)
        {

            //get work item for the id found in query
            WorkItem workItem = WorkItems.FetchWorkItemData(id);

            // Add TC info to dict TODO: Tags / Category 
            var tcInfo = new Dictionary<ScalarValue, LogEventPropertyValue>()
            {
                { new ScalarValue("TestSuiteGuid"), new ScalarValue(TestSuiteIdentifier.TestSuiteGuid.ToString()) },
                { new ScalarValue("TestSuiteStartTime"), new ScalarValue(TestSuiteIdentifier.TestSuiteStartTime.ToString("dd/MM/yyyy HH:mm:ss.fff"))},
                { new ScalarValue("Area"), new ScalarValue(workItem.Fields["System.NodeName"].ToString()) },
                { new ScalarValue("TeamProject"), new ScalarValue(workItem.Fields["System.TeamProject"].ToString()) },
                { new ScalarValue("Sprint"), new ScalarValue(Regex.Match(workItem.Fields["System.IterationPath"].ToString(),@"\d+").Value) },
                { new ScalarValue("State"), new ScalarValue(workItem.Fields["System.State"].ToString()) },
                { new ScalarValue("WorkItemType"), new ScalarValue(workItem.Fields["System.WorkItemType"].ToString()) },
                { new ScalarValue("AssignedTo"), new ScalarValue(((IdentityRef)workItem.Fields["System.AssignedTo"]).DisplayName)},
                { new ScalarValue("Id"), new ScalarValue(id.ToString()) },
                { new ScalarValue("Title"), new ScalarValue(workItem.Fields["System.Title"].ToString()) },
                { new ScalarValue("Priority"), new ScalarValue(workItem.Fields["Microsoft.VSTS.Common.Priority"].ToString()) },
                { new ScalarValue("Url"), new ScalarValue(UrlBuilder(workItem.Fields["System.TeamProject"].ToString(),id.ToString())) },
            };

            // Get relations and add info to dict
            List<int> relationsList = new List<int>();
            var regex = new Regex(@"\d+$");
            if (workItem.Relations != null && workItem.Relations.Any())
            {
                foreach (var relation in workItem.Relations)
                {
                    if (regex.IsMatch(relation.Url))
                        relationsList.Add(int.Parse(regex.Match(relation.Url).Value));
                }

                var tempWorkItemDict = new Dictionary<ScalarValue, LogEventPropertyValue>();
                var relatedWorkitems = WorkItems.GetWorkItems(relationsList);
                foreach (var wI in relatedWorkitems)
                {
                    var tempDict = new Dictionary<ScalarValue, LogEventPropertyValue>() {
                    { new ScalarValue("Sprint"), new ScalarValue(Regex.Match(wI.Fields["System.IterationPath"].ToString(), @"\d+").Value) },
                    { new ScalarValue("WorkItemType"), new ScalarValue(wI.Fields["System.WorkItemType"].ToString()) },
                    { new ScalarValue("AssignedTo"), new ScalarValue(((IdentityRef)wI.Fields["System.AssignedTo"]).DisplayName)},
                    { new ScalarValue("Id"), new ScalarValue(wI.Id.Value) },
                    { new ScalarValue("Title"), new ScalarValue(wI.Fields["System.Title"].ToString()) },
                    { new ScalarValue("State"), new ScalarValue(wI.Fields["System.State"].ToString()) },
                    { new ScalarValue("Url"), new ScalarValue(UrlBuilder(wI.Fields["System.TeamProject"].ToString(),wI.Id.Value.ToString())) },
                };

                    // severity is exclusive for type bug, priority is not present for type feature
                    if (wI.Fields.ContainsKey("Microsoft.VSTS.Common.Severity"))
                        tempDict.Add(new ScalarValue("Severity"), new ScalarValue(wI.Fields["Microsoft.VSTS.Common.Severity"].ToString()));
                    if (wI.Fields.ContainsKey("Microsoft.VSTS.Common.Priority"))
                        tempDict.Add(new ScalarValue("Priority"), new ScalarValue(wI.Fields["Microsoft.VSTS.Common.Priority"].ToString()));

                    tempWorkItemDict.Add(new ScalarValue(wI.Id.Value), new DictionaryValue(tempDict));

                }
                if (tempWorkItemDict.Any())
                    tcInfo.Add(new ScalarValue("Relations"), new DictionaryValue(tempWorkItemDict));
            }
            return tcInfo;

        }

    }
}
