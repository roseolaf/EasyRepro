using Draeger.Dynamics365.Testautomation.Attributes;
using Draeger.Dynamics365.Testautomation.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Draeger.Dynamics365.Testautomation.Sales.OpportunityManagement
{
    /// <summary>
    /// Summary description for CopyOpportunity
    /// </summary>
    [TestClass]
    public class CopyOpportunityWithAllEditableFields : TestBase
    {
        //[TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression")]
        [TestCategory("Testautomation")]
        [TestCase(WorkItem = 50774, Title = "The user copies an own open opportunity with all editable fields filled and a lot of associations")]
        public void TheUserCopiesAnOwnOpenOpportunityWithAllEditableFieldsFilledAndALotOfAssociations()
        {
            //SetSecurityRoles();

            //using (XrmBrowser = new Browser(TestOptionsManager.GetDefaultOptions()))
            //{
            //    ExecuteDev(XrmBrowser);
            //    //ExecuteProd(xrmBrowser);
            //}
        }

        //private void ExecuteDev(IXrmApplication xrmBrowser)
        //{
        //    Login("user");
        //    //OpenFirstOpportunity();
        //    xrmBrowser.ThinkTime(2000);

        //    //xrmBrowser.OpportunityEntity.SoldToParty = "Dräger Austria";

        //    xrmBrowser.GetCustomPage<OpportunityEntity>().AddSalesTeamUser("jennifer");

        //    //AssociateEntities(xrmBrowser);

        //    //ShareOpportunity(xrmBrowser);
        //}

        //private void ExecuteProd(IXrmApplication xrmBrowser)
        //{
        //    var driver = xrmBrowser.Driver;

        //    // 1. Login as user having roles "DW Basic CRM Access" and "DW Opportunity Management"
        //    xrmBrowser.LoginPage.Login(XrmUri, Username, Password);
        //    // Wait for page loaded
        //    driver.WaitUntilClickable(By.Id("TabUserInfoId")); // wait for user info icon

        //    xrmBrowser.GuidedHelp.CloseGuidedHelp();
        //    xrmBrowser.ThinkTime(500);

        //    xrmBrowser.Navigation.OpenSubArea("Sales", "Opportunities");
        //    xrmBrowser.ThinkTime(200);

        //    // 2. Generate a new opportunity.
        //    xrmBrowser.CommandBar.ClickCommand("New");
        //    xrmBrowser.ThinkTime(2000);

        //    // 2. Fill in all editable fields on the opportunity form.
        //    // 3. Make sure that the fields "Sold-To Party", "Ship-To Party" and "End User Party" are filled with different institution records.  
        //    // 4. Make sure that field "Probability (%)" does not equal "10% - Very Unlikely"
        //    // 5. Make sure that field "Is Funnel Relevant" is set to "No"
        //    // 6. Change the default values of all fields that are already filled in by default
        //    SetOpportunityFields(xrmBrowser);

        //    // 7. Save the Opportunity.
        //    xrmBrowser.Entity.Save(5000);

        //    xrmBrowser.ThinkTime(2000);
        //    ShareOpportunity(xrmBrowser);

        //    // 8. Change the business process flow stage so that is does not equal "1 Need" 
        //    //xrmBrowser.BusinessProcessFlow.NextStage(3000); // switch to '2 Consideration'

        //    // 9. Change all business process flow fields in all stages that are not marked
        //    //    with green check mark.
        //    xrmBrowser.ThinkTime(2000);
        //    SetBusinessProcessFlowFields(xrmBrowser);

        //    // 10. Make sure that you follow the Opportunity.
        //    xrmBrowser.CommandBar.ClickCommand("Follow");

        //    // 11. Make sure that the opportunity is shared with at least one user
        //    ShareOpportunity(xrmBrowser);

        //    // 12. Make sure that at least one record of the following entities is associated
        //    //     to the opportunity:
        //    //      - Activities
        //    //      - Notes
        //    //      - Posts
        //    //      - User(Sales Team)
        //    //      - Contact(Opportunity Stakeholder)
        //    //      - Competitor
        //    AssociateEntities(xrmBrowser); // Todo: Complete associated Entities

        //    // 13. Click on button "Copy Opportunity" in the ribbon.
        //    //  => The notification "Copying in progress" occurs. 
        //    //     As long as the notification is shown no user actions in CRM are possible.
        //    xrmBrowser.CommandBar.ClickCommand("COPY OPPORTUNITY");
        //    TheNotificationCopyingInProgressOccurs(xrmBrowser);

        //    // 14. n/a
        //    //  => After finishing the copy process the notification "Copying in progress" disappears.
        //    //     Notification
        //    //     „Copy successfully created. -
        //    //     Products that are no longer sellable were not copied.
        //    //     To guarantee a correct funnel volume adjust at least all funnel relevant fields of the opportunity copy and all associated opportunity products.“ 
        //    //     occurs.
        //    //     The window does no longer show the original opportunity record but it shows the opportunity copy


        //    // 15. Check the status of the opportunity copy
        //    //  => The opportunity is in status "Open".

        //    // 16. Check the business process flow
        //    //  => Business process flow "Opportunity Process" is shown and set to phase "Need"

        //    // 17. Go to "Posts"
        //    //  => The post/s entered before to the original Opportunity manually are not copied.

        //    // 18. Go to "Activities"
        //    //  => There are no activities available 

        //    // 19. Go to "Notes"
        //    //  => There are no notes available

        //    // 20. Go to "Sales Team"
        //    //  => The same entries as in the original opportunity are available.

        //    // 21. Go to "Stakeholders"
        //    //  => The same entries as in the original opportunity are available.

        //    // 22. Go to "Competitors"
        //    //  => The same entries as in the original opportunity are available.

        //    // 23. Check the menu
        //    //  => "Follow" is available in the menu

        //    // 24. Click on "Share" in the menu
        //    //  => There are no entries available in the sharing dialog

        //    // 25. Compare original opportunity and the copy
        //    //  => The fields of the opportunity copy and its business process flow
        //    //     are filled in as defined in the attached White List
        //}

        //private void SetOpportunityFields(IXrmApplication xrmBrowser)
        //{
        //    var opportunity = xrmBrowser.GetCustomPage<OpportunityEntity>();
        //    // Mandatory Fields
        //    opportunity.Topic = $"Auto generated Opportunity {DateTime.Now.ToString("g")}";
        //    opportunity.SoldToParty = "Dräger";
        //    opportunity.ShipToParty = "dev partner";
        //    opportunity.EndUserParty = "robert bosch gmbh 1";
        //    opportunity.Value = "1000";
        //    opportunity.Currency = "Euro";
        //    opportunity.Probability = OpportunityConstants.Probability.GoodChance;
        //    opportunity.OrderEntryDate = DateTime.Today.ToString("MM/dd/yyyy");
        //    opportunity.CustomerDeliveryDate = DateTime.Today.AddDays(10).ToString("MM/dd/yyyy");
        //    opportunity.TimingAccuracy = OpportunityConstants.TimingAccuracy.Good;

        //    opportunity.CustomerSolution = "Heringsschwarm";
        //}

        //private void SetBusinessProcessFlowFields(IXrmApplication xrmBrowser)
        //{
        //    // 1. Need
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_basicinfoprovided", Value = "Completed" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_primarycontactidentified", Value = "Yes" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_specificsolutionrequested", Value = "Yes" });
        //    xrmBrowser.ThinkTime(2000);

        //    // 2. Consideration (Active for 1 day, 2 hours)
        //    xrmBrowser.BusinessProcessFlow.NextStage();
        //    xrmBrowser.ThinkTime(2000);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_internalproject", Value = "Yes" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_decisionmakeridentified", Value = "Completed" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_requiredproductsknown", Value = "Completed" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_quotepresented", Value = "Complete" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_negotiationrequested", Value = "Yes" });
        //    xrmBrowser.ThinkTime(50);

        //    // 3. Purchase
        //    xrmBrowser.BusinessProcessFlow.NextStage();
        //    xrmBrowser.ThinkTime(2000);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_orderdateverified", Value = "Completed" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_deliverydateverified", Value = "Completed" });
        //    xrmBrowser.ThinkTime(500);
        //    xrmBrowser.BusinessProcessFlow.SetValue(new TwoOption { Name = "dw_finalquotepresented", Value = "Completed" });
        //    xrmBrowser.ThinkTime(500);
        //}

        //private void ShareOpportunity(IXrmApplication xrmBrowser)
        //{
        //    xrmBrowser.CommandBar.ClickCommand("Share");

        //    var shareDialog = xrmBrowser.GetCustomPage<ShareDialog>();
        //    shareDialog.AddUser("jennifer");
        //    shareDialog.ClickShare();
        //}

        //private void AssociateEntities(IXrmApplication xrmBrowser)
        //{
        //    //      - User(Sales Team)
        //    //      - Contact(Opportunity Stakeholder)
        //    //      - Competitor
        //    // Activity
        //    xrmBrowser.ActivityFeed.SelectTab(ActivityFeed.Tab.Activities);
        //    xrmBrowser.ActivityFeed.AddTask($"Task from {DateTime.Now.ToString()}", "Task Description", DateTime.Now.AddDays(2),
        //        new OptionSet { Name = "prioritycode", Value = "Normal" });
        //    xrmBrowser.ThinkTime(2000);

        //    // Post
        //    xrmBrowser.ActivityFeed.SelectTab(ActivityFeed.Tab.Posts);
        //    xrmBrowser.ActivityFeed.AddPost($"Post from {DateTime.Now.ToString()}");
        //    xrmBrowser.ThinkTime(2000);

        //    // Note
        //    xrmBrowser.ActivityFeed.SelectTab(ActivityFeed.Tab.Notes);
        //    xrmBrowser.ActivityFeed.AddNote($"Note from {DateTime.Now.ToString()}");
        //    xrmBrowser.ThinkTime(2000);

        //    // User(Sales Team)
        //    xrmBrowser.GetCustomPage<OpportunityEntity>().AddSalesTeamUser("jennifer");

        //    // Contact(Opportunity Stakeholder)
        //    xrmBrowser.GetCustomPage<OpportunityEntity>().AddStakeholder("");

        //    // Competitor
        //    xrmBrowser.GetCustomPage<OpportunityEntity>().AddCompetitor("");
        //}

        //[AcceptanceCriteria(WorkItem = 50774, Sort = 1, Description = "The Notification 'Copying in progress' occurs")]
        //private bool TheNotificationCopyingInProgressOccurs(IXrmApplication xrmBrowser)
        //{
        //    string message = GetAcceptanceCriteriaMessage(MethodBase.GetCurrentMethod());

        //    var notifications = xrmBrowser.Notifications.FormNotifications.Value;

        //    foreach (var notification in notifications)
        //    {
        //        if (notification.Message.ToUpper().StartsWith("COPYING IN PROGRESS"))
        //        {
        //            Console.WriteLine(message);
        //            return true;
        //        }
        //    }

        //    Console.WriteLine(message);
        //    throw new AssertFailedException(message);
        //}

        //private void WaitForNotificationDisappears(Browser xrmBrowser, string notification)
        //{
        //    WebDriverWait wait = new WebDriverWait(xrmBrowser.Driver, TimeSpan.FromSeconds(60));
        //    //wait.Until(ExpectedConditions.ElementIsVisible(By.));
        //}
    }
}
