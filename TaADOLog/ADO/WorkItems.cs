using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using static TaADOLog.ADO.ADOManager;

namespace TaADOLog.ADO
{
    internal static class WorkItems
    {

        private static WorkItem FetchWorkItemData(int id)
        {
            //create http client and query for results
            WorkItemTrackingHttpClient witClient = VssConnection().GetClient<WorkItemTrackingHttpClient>();
            //get work item for the id found in query
            var result = witClient.GetWorkItemAsync(id, expand: WorkItemExpand.All).Result;
            lock (workItemDict)
            {
                workItemDict[id] = result;
                return result;
            }

        }

        private static List<WorkItem> FetchWorkItemsData(List<int> ids)
        {
            //create http client and query for results
            WorkItemTrackingHttpClient witClient = VssConnection().GetClient<WorkItemTrackingHttpClient>();
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
    }
}
