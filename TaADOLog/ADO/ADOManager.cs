using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaADOLog.Logger.Sink;

namespace TaADOLog.ADO
{
    public static class ADOManager
    {
        public static string ADOProjectName { get; set; }// "CRM";
        public static string ADOOrganizationUrl { get; set; }// "https://dev.azure.com/draeger";
        public static string personalAccessToken { get; set; }//"6opssjduepken5jciswfhpsywohwyng3lfzcr5tbtti54vilfjya";
        public static string ADOResponsibleEMail { get; set; }// "malte.fries@draeger.com";

        internal static Dictionary<int, WorkItem> workItemDict = new Dictionary<int, WorkItem>();
        
        private static readonly object Lock = new object();

        internal static VssConnection VssConnection()
        {
            lock (Lock)
            {
                var creds = new VssCredentials(new VssBasicCredential("AutomatedClient", personalAccessToken));
                //Prompt user for credential
                VssConnection connection = new VssConnection(new Uri(ADOOrganizationUrl), creds);
                //create http client and query for results
                WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
                return connection;
            }
        }

        public static WorkItem GetWorkItem(int id) => WorkItems.GetWorkItem(id);
        public static List<WorkItem> GetWorkItems(List <int> ids) => WorkItems.GetWorkItems(ids);
        public static void CreateOrUpdateBug(int testCaseId, List<ListSinkInfo<TestContext>> loggerSinkList,Exception exception) => BugManager.CreateOrUpdateBug(testCaseId, loggerSinkList,exception);


    }
}