using System;
using Draeger.Dynamics365.Testautomation.Attributes;
using Draeger.Dynamics365.Testautomation.Common;
using Draeger.Dynamics365.Testautomation.Common.EntityManager;
using Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator;
using Infoman.Xrm.Services;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Draeger.Dynamics365.Testautomation.Common.Enums;
using Draeger.Dynamics365.Testautomation.Common.Helper;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Draeger.Dynamics365.Testautomation.Common.PageObjects;
using Draeger.Dynamics365.Testautomation.ExtensionMethods;
using Draeger.Testautomation.CredentialsManagerCore;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using System.Linq;
using Draeger.Testautomation.CredentialsManagerCore.Attributes;
using Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements;
using Microsoft.TeamFoundation.Framework.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TaADOLog.Logger;
using StringExtensions = Draeger.Dynamics365.Testautomation.ExtensionMethods.StringExtensions;
using Entity = Microsoft.Xrm.Sdk.Entity;

//using Infoman.Xrm.Services;
namespace Draeger.Dynamics365.Testautomation.Sales.OpportunityManagement
{
    [TestClass]
    public class OpportunitiesAreClosed : TestBase
    {
        private TestAutomationContext _context;
        protected static Dictionary<string, Entity> testData = new Dictionary<string, Entity>();
        private const string UserAlias = "UserAlias";

        [TestInitialize]
        public void InitializeTest()
        {
            var TestCaseId = TestContext.Properties["TestCaseId"];

            switch (TestCaseId)
            {
                case "33318":
                    testData.Add("33318Institution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                            new SoldToDecorator(
                                new TestcaseNameDecorator(
                                    new BaseComponent(Logger))))
                        .CreateEntityRecord(this, new Account()));
                    testData.Add("33318Competitor1",
                        new TestcaseNameDecorator(
                                new BaseComponent(Logger))
                            .CreateEntityRecord(this, new Competitor()));
                    testData.Add("33318Competitor2",
                        new TestcaseNameDecorator(
                                new BaseComponent(Logger))
                            .CreateEntityRecord(this, new Competitor()));
                    testData.Add("33318Competitor3",
                        new TestcaseNameDecorator(
                                new BaseComponent(Logger))
                            .CreateEntityRecord(this, new Competitor()));

                    break;
                case "38264":
                    testData.Add("38264Institution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                        new SoldToDecorator(
                            new TestcaseNameDecorator(
                                new BaseComponent(Logger))))
                        .CreateEntityRecord(this, new Account()));
                    break;

                case "33383":
                    testData.Add("33383Institution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                        new SoldToDecorator(
                            new TestcaseNameDecorator(
                                new BaseComponent(Logger))))
                        .CreateEntityRecord(this, new Account()));

                    testData.Add("33383Opportunity",
                            new TestcaseNameDecorator(
                                new BaseComponent(Logger))
                        .CreateEntityRecord(this, new Opportunity()));
                    break;

                case "33326":
                    testData.Add("33326Institution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                        new SoldToDecorator(
                            new TestcaseNameDecorator(
                                new BaseComponent(Logger))))
                        .CreateEntityRecord(this, new Account()));
                    break;
                case "63479":
                    testData.Add("63479Institution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                                        new SoldToDecorator(
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(Logger))))
                                                        .CreateEntityRecord(this, new Account()));
                    testData.Add("63479CustomerSolution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                                            new ReferenceToDecorator("dw_operatinginstitutionid", testData["63479Institution"],
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(Logger))))
                                                        .CreateEntityRecord(this, new Customersolution()));
                    break;
                case "63419":
                    testData.Add("63419Institution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                                        new SoldToDecorator(
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(Logger))))
                                                        .CreateEntityRecord(this, new Account()));
                    testData.Add("63419CustomerSolution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                                            new ReferenceToDecorator("dw_operatinginstitutionid", testData["63419Institution"],
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(Logger))))
                                                        .CreateEntityRecord(this, new Customersolution()));
                    break;
                case "63526":
                    testData.Add("63526Institution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                                        new SoldToDecorator(
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(Logger))))
                                                        .CreateEntityRecord(this, new Account()));
                    testData.Add("63526CustomerSolution", new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                                            new ReferenceToDecorator("dw_operatinginstitutionid", testData["63526Institution"],
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(Logger))))
                                                        .CreateEntityRecord(this, new Customersolution()));
                    testData.Add("63526Competitor",
                                        new TestcaseNameDecorator(
                                            new BaseComponent(Logger))
                                    .CreateEntityRecord(this, new Competitor()));
                    break;
            }

            _context = new TestAutomationContext();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            foreach (var tD in testData)
            {
                new BaseComponent().DeleteEntityRecord(tD.Value.LogicalName, tD.Value.Id);
            }
        }

        [TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [TestProperty("TestCaseId", "38264")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwOpportunityManagement, UserGroup =
            UserGroup.Sales)]
        [Priority(2)]
        public void AnOpportunityThatIsFunnelRelevantIsClosedAsWon()
        {
            var institution = testData["38264Institution"] as Account;


            try
            {
                Logger.NextStep();
                Login(XrmApp, UserAlias);

                Logger.NextStep();
                Logger.Log<Action<string>>(XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(XrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "New");

                Logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(XrmApp, XrmBrowser);
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                        "In the opportunity record, button 'Close as Won' is not available",
                        OpportunityLocators.CloseAsWon));

                var selectedOpportunityProbability = OpportunityConstants.Probability.FewChances;

                Logger.LogSet(() => newOpportunityPage.Topic, "Test Case 38264 Topic");
                Logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Attributes["name"]);
                Logger.LogSet(() => newOpportunityPage.Value, "500");
                Logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                Logger.LogSet(() => newOpportunityPage.Probability, selectedOpportunityProbability);
                Logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                Logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                Logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");


                Assert.IsTrue(
                    Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "'Is Funnel Relevant' is set to 'Yes' (default value)"), "funnel relevant is false");

                Logger.NextStep();
                Logger.Log<Action>(XrmApp.Entity.Save);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                        "Button 'Close as Won' occurs to the right of button 'Refresh'.",
                        OpportunityLocators.CloseAsWon), "Button 'Close as Won' should occur to the right of button 'Refresh'.");

                Logger.NextStep();
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Summary");

                Logger.LogSet(() => newOpportunityPage.AdditionalInformation, "38264 Additional Information");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                var endTime = DateTime.Now.AddSeconds(30);
                var success = false;
                do
                    if (newOpportunityPage.GetUnsavedChangesFooterVisible())
                    {
                        Logger.Log<Action<string, string>>(newOpportunityPage.ClickCommand, "Close as Won");
                        success = true;
                    }
                    else
                    {
                        newOpportunityPage.AdditionalInformation = "";
                        XrmApp.Entity.Save();
                        newOpportunityPage.AdditionalInformation = "38264 Additional Information";
                        XrmApp.ThinkTime(100);
                    }
                while (!success && DateTime.Now < endTime);

                Assert.IsTrue(success, "Didn't get 'unsaved changes' footer");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, true,
                    "A notification occurs: 'You have unsaved changes. Please save/refresh the opportunity to close it.'"), "No notification");

                Logger.NextStep();
                Logger.Log<Action>(XrmApp.Entity.Save);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, false,
                    "The notification disappears"), "Notification still visible");

                Logger.NextStep();
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Close as Won");

                var closingDialog = new OpportunityClosingDialog(XrmApp, XrmBrowser);

      

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true, "The closing dialog occurs.", true));

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select a reason for closing as won:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.ReasonForClosingField), "Field not marked as mandatory");


                Logger.NextStep();
                Logger.Log<Action>(closingDialog.Finish);

                var validationFailedDialog = new OpportunityClosingValidationErrorDialog(XrmApp, XrmBrowser);

           
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                Logger.NextStep();
                Logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, false,
                        "The notification is closed.", false),
                    "The notification is shown.");
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                    "The closing dialog occurs.", true), "Closing dialog not shown");
                var longText = StringExtensions.Random(2002);

                Logger.LogSet(() => closingDialog.CommentWonOrCanceled, longText);


                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Maximum 2000 characters can be filled in. All further filled in characters are discarded without any notification. " +
                    "The field shows 4 rows of the text.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                Logger.NextStep();
                Logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs again: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                Logger.NextStep();
                Logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Insert a comment for closing as won:'' is still filled in.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                var picklist = closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog.ReasonForClosingField).ToList();

                Logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                Logger.NextStep();
                Logger.Log<Action>(closingDialog.Cancel);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false, "The dialog is closed.", false));


                Assert.IsTrue(Logger.LogGetExpectedResultCheck(() => newOpportunityPage.Probability,
                    selectedOpportunityProbability, "The opportunity is not changed."), "Opportunity probability changed");

                Logger.NextStep();

                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Close as Won");

                Assert.IsTrue(closingDialog.DialogIsOpen(true),
                    "Closing dialog not shown after 5 seconds");


                Assert.IsTrue(Logger.LogGetExpectedResultCheck(() => closingDialog.CommentWonOrCanceled, "",
                    "The dialog opens up with the fields not filled in."));

                Logger.NextStep();

                Logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);
                Logger.LogSet(() => closingDialog.CommentWonOrCanceled, "Comment for Won test case 38264");

                Logger.NextStep();
                Logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(
                    Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false,
                        "The closing dialog is closed.", false), "closing dialog not closed");
                Assert.IsTrue(Logger.LogGetExpectedResultCheck(() => newOpportunityPage.Probability,
                    OpportunityConstants.Probability.OrderReceived, "Field 'Probability (%)' is set to '100% - Order Received'."),
                    "Opportunity probability changed");
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                Assert.IsTrue(Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "On tab 'Details', -field 'Is Funnel Relevant' is still set to 'Yes'"), "funnel relevant is false");
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Administration");
                Func<string> getOpportunityStatus = () => newOpportunityPage.Status;
                Assert.IsTrue(
                    Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatus, "Won",
                        "On tab 'Administration', - Field 'Status' equals 'Won'."), "status is not won");
                Func<string> getOpportunityStatusReason = () => newOpportunityPage.StatusReason;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatusReason, picklist[picklist.Count - 1],
                        "On tab 'Administration', - Field 'Status Reason' is set to the value that has been selected in the closing dialog."),
                    "status reason differs from input");
                Func<string> getOpportunityClosingComment = () => newOpportunityPage.ClosingComment;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityClosingComment, "Comment for Won test case 38264",
                        "On tab 'Administration', - Field 'Closing Comment' is filled in with the content of dialog field 'Insert a comment for closing as won:' correctly."),
                    "Closing comment differs from input");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetReadOnlyNotificationVisible, true,
                    "The opportunity is read-only."), "Read only Notification not found");

                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Funnel");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                Func<string> getLatestOpportunityTimelineContent =
                    () => newOpportunityPage.TimelineControl[0].Content;
                if (getLatestOpportunityTimelineContent() == "Won Opportunity")
                    getLatestOpportunityTimelineContent =
                        () => newOpportunityPage.TimelineControl[1].Content;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getLatestOpportunityTimelineContent, $"won Opportunity for Institution {institution.Name}",
                    "A new Auto Post is generated saying that the Opportunity is won for the Institution recorded in field 'Sold-To Party'."));

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Won' is not available",
                    OpportunityLocators.CloseAsWon));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Cancelled' is not available",
                    OpportunityLocators.CloseAsCanceled));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                    "In the opportunity record, button 'Reopen Opportunity' is available.",
                    OpportunityLocators.ReopenOpportunity));

            }
            catch (Exception e)
            {
                Exception = e;

                throw;
            }
        }

        [TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [TestProperty("TestCaseId", "33318")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwOpportunityManagement, UserGroup =
            UserGroup.Sales)]
        [Priority(2)]
        public void AnOpportunityThatIsFunnelRelevantIsClosedAsLost()
        {
            var institution = testData["33318Institution"];
            var competitor1 = testData["33318Competitor1"];
            var competitor2 = testData["33318Competitor2"];
            var competitor3 = testData["33318Competitor3"];


            try
            {
                Logger.NextStep(); //Step 2
                Login(XrmApp, UserAlias);

                Logger.NextStep(); //Step 3
                Logger.Log<Action<string>>(XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(XrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "New");

                Logger.NextStep(); //Step 4
                var newOpportunityPage = new OpportunityEntity(XrmApp, XrmBrowser);
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));

                var selectedOpportunityProbability = OpportunityConstants.Probability.FewChances;

                Logger.LogSet(() => newOpportunityPage.Topic, "Test Case 33318 Topic");
                Logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Attributes["name"]);
                Logger.LogSet(() => newOpportunityPage.Value, "500");
                Logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                Logger.LogSet(() => newOpportunityPage.Probability, selectedOpportunityProbability);
                Logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                Logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                Logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");


                Assert.IsTrue(
                    Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "'Is Funnel Relevant' is set to 'Yes' (default value)"), "funnel relevant is false");

                Logger.NextStep(); //Step 5
                Logger.Log<Action>(XrmApp.Entity.Save);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                        newOpportunityPage.GetCommandBarButtonAvailability, true,
                        "Button 'Close as Lost' occurs to the right of button 'Close as Won'.",
                        OpportunityLocators.CloseAsLost),
                    "Button 'Close as Lost' should occur to the right of button 'Close as Won'.");

                Logger.NextStep(); //Step 6
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                Logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                XrmApp.ThinkTime(200);

                Logger.LogSet(() => newOpportunityPage.SelectRecord, competitor1.Attributes["name"]);
                Logger.Log<Action>(newOpportunityPage.ClickAddRecord);
                XrmBrowser.Browser.Driver.WaitForTransaction();

                Logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                XrmApp.ThinkTime(200);

                Logger.LogSet(() => newOpportunityPage.SelectRecord, competitor2.Attributes["name"]);
                Logger.Log<Action>(newOpportunityPage.ClickAddRecord);
                XrmBrowser.Browser.Driver.WaitForTransaction();

                Logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                XrmApp.ThinkTime(200);

                Logger.LogSet(() => newOpportunityPage.SelectRecord, competitor3.Attributes["name"]);
                Logger.Log<Action>(newOpportunityPage.ClickAddRecord);
                XrmBrowser.Browser.Driver.WaitForTransaction();

                //TODO: check if competitors in subgrid

                var competitorNames = new List<string>
                {
                    competitor1.Attributes["name"].ToString(),
                    competitor2.Attributes["name"].ToString(),
                    competitor3.Attributes["name"].ToString(),
                };
                var grid = new GridElement(XrmApp, XrmBrowser);
                var cellTitles =
                    Logger.LogExpectedResult<Func<string, int, List<string>>, List<string>>(grid.GetCellTitles,
                        competitorNames,
                        "The selected competitors are available in subgrid \"Competitors\" correctly.",
                        "dataSetRoot_Competitors", 2);

                Assert.IsTrue(cellTitles.All(competitorNames.Contains), "The selected competitors are available in subgrid \"Competitors\" correctly.");


                Logger.NextStep(); //Step 7
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Summary");

                Logger.LogSet(() => newOpportunityPage.AdditionalInformation, "33318 Additional Information");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                var endTime = DateTime.Now.AddSeconds(30);
                var success = false;
                do
                    if (newOpportunityPage.GetUnsavedChangesFooterVisible())
                    {
                        Logger.Log<Action<string, string>>(newOpportunityPage.ClickCommand, "Close as Lost");
                        success = true;
                    }
                    else
                    {
                        newOpportunityPage.AdditionalInformation = "";
                        XrmApp.Entity.Save();
                        newOpportunityPage.AdditionalInformation = "33318 Additional Information";
                        XrmApp.ThinkTime(100);
                    }
                while (!success && DateTime.Now < endTime);

                Assert.IsTrue(success, "Didn't get 'unsaved changes' footer");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, true,
                    "A notification occurs: 'You have unsaved changes. Please save/refresh the opportunity to close it.'"), "No notification");

                Logger.NextStep(); //Step 8
                Logger.Log<Action>(XrmApp.Entity.Save);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, false,
                    "The notification disappears"), "Notification still visible");

                Logger.NextStep(); //Step 9
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Close as Lost");

                var closingDialog = new OpportunityClosingDialog(XrmApp, XrmBrowser);

           

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                        "The closing dialog occurs.", true), "Closing dialog not shown");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select a reason for closing as lost:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.ReasonForClosingField), "Reason field not marked as mandatory");
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select the main competitor that won the deal:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.MainCompetitorThatWonTheDealField), "Competitor field not marked as mandatory");

                Logger.NextStep(); //Step 10
                Logger.Log<Action>(closingDialog.Finish);

                var validationFailedDialog = new OpportunityClosingValidationErrorDialog(XrmApp, XrmBrowser);

         
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                Logger.NextStep(); //Step 11
                Logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, false,
                        "The notification is closed.", false),
                    "The notification is shown.");


                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                    "The closing dialog occurs.", true), "Closing dialog not shown");

                Logger.NextStep(); //Step 12
                var picklist = closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog.ReasonForClosingField).ToList();

                Logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                Logger.NextStep(); //Step 13

                Logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                Logger.NextStep(); //Step 14
                Logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Insert a comment for closing as won:'' is still filled in.", picklist[picklist.Count - 1], closingDialog.ReasonForClosing));

                Logger.NextStep(); //Step 15

                Logger.Log<Action>(closingDialog.Cancel);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false, "The dialog is closed.", false));


                Assert.IsTrue(Logger.LogGetExpectedResultCheck(()=>newOpportunityPage.Probability,
                    selectedOpportunityProbability, "The opportunity is not changed."), "Opportunity probability changed");

                Logger.NextStep(); //Step 16
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Close as Lost");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true, "The closing dialog opens up again.", true));


                Logger.NextStep(); //Step 17
                var competitorList =
                    closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog
                        .MainCompetitorThatWonTheDealField).ToList();
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<IEnumerable<string>, string, bool>, bool>(Enumerable.Contains, true, "The picklist shows exactly the competitors that have been associated to the opportunity.", competitorList, competitor1.Attributes["name"]), "Competitor1 mising");
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<IEnumerable<string>, string, bool>, bool>(Enumerable.Contains, true, "The picklist shows exactly the competitors that have been associated to the opportunity.", competitorList, competitor2.Attributes["name"]), "Competitor2 mising");
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<IEnumerable<string>, string, bool>, bool>(Enumerable.Contains, true, "The picklist shows exactly the competitors that have been associated to the opportunity.", competitorList, competitor3.Attributes["name"]), "Competitor3 mising");
                Logger.LogSet(() => closingDialog.MainCompetitorThatWonTheDeal, competitorList[0]);

                Logger.NextStep(); //Step 18

                Logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs again: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                Logger.NextStep(); //Step 19
                Logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Select a main competitor that won the deal' is still filled in.", competitorList[0], closingDialog.MainCompetitorThatWonTheDeal));

                Logger.NextStep(); //Step 20
                Logger.LogSet(() => closingDialog.SalesAbandoned, true);
                var commentLost = "Comment for Lost 33318";
                Logger.LogSet(() => closingDialog.CommentLost, commentLost);
                Logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                Logger.NextStep();
                Logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(
                    Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false,
                        "The closing dialog is closed.", false), "closing dialog not closed");
                Assert.IsTrue(Logger.LogGetExpectedResultCheck(()=>newOpportunityPage.Probability,
                    OpportunityConstants.Probability.OrderLost, "Field 'Probability(%)' is set to '0 % -Order Lost'."), "Opportunity probability not 0%");

                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                Assert.IsTrue(Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                    "On tab 'Details', -field 'Is Funnel Relevant' is still set to 'Yes'"), "funnel relevant is false");

                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Administration");
                Assert.IsTrue(
                    Logger.LogGetExpectedResultCheck(()=>newOpportunityPage.Status, "Lost",
                        "On tab 'Administration', - Field 'Status' equals 'Lost'."), "status is not lost");
                Func<string> getOpportunityStatusReason = () => newOpportunityPage.StatusReason;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatusReason, picklist[picklist.Count - 1],
                        "On tab 'Administration', - Field 'Status Reason' is set to the value that has been selected in the closing dialog."),
                    "status reason differs from input");
                Func<string> getOpportunityClosingComment = () => newOpportunityPage.ClosingComment;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityClosingComment, commentLost,
                        "On tab 'Administration', - Field 'Closing Comment' is filled in with the content of dialog field 'Insert a comment for closing as lost:' correctly."),
                    "Closing comment differs from input");
                Func<bool> getOpportunitySalesAbandoned = () => newOpportunityPage.SalesAbandoned;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(getOpportunitySalesAbandoned, true,
                        "On tab 'Administration', - Field 'Sales Abandoned' is set to'Yes'"),
                    "Sales abandoned not set");
                Func<string> getOpportunityCompetitorThatWon = () => newOpportunityPage.MainCompetitor;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityCompetitorThatWon,
                        competitorList[0], "On tab 'Administration', - Field 'Main Competitor' is filled with the value selected in the dialog"),
                    "Main competitor differs from selection");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetReadOnlyNotificationVisible, true,
                    "The opportunity is read-only."), "Read only Notification not found");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Won' is not available",
                    OpportunityLocators.CloseAsWon));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Cancelled' is not available",
                    OpportunityLocators.CloseAsCanceled));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                    "In the opportunity record, button 'Reopen Opportunity' is available.",
                    OpportunityLocators.ReopenOpportunity));


            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [Priority(1)]
        [CrmUser(UserAlias,
            SecurityRole.DwBasicCrmAccess,
            SecurityRole.DwOpportunityManagement,
            SecurityRole.DwOpportunityExtendedReadBu,
            UserGroup = UserGroup.Sales)]
        [TestProperty("TestCaseId", "33383")]
        public void AUserCannotCloseAnOpportunityHeSheHasOnlyReadAccessTo()
        {
            var institution = testData["33383Institution"];
            var opportunity = testData["33383Opportunity"] as Opportunity;


            try
            {
                Logger.NextStep();
                Login(XrmApp, UserAlias);
                Logger.NextStep();

                Logger.Log<Action<string, Guid>>(
                    XrmApp.Entity.OpenEntity, opportunity.LogicalName, opportunity.Id);

                var commandValues = Logger.Log<Func<bool, int, BrowserCommandResult<List<string>>>, BrowserCommandResult<List<string>>>(
                    XrmApp.CommandBar.GetCommandValues);


                List<string> expectedResultList = new List<string>
                {
                    OpportunityLocators.CloseAsWon,
                    OpportunityLocators.CloseAsLost,
                    OpportunityLocators.CloseAsCanceled
                };

                var expectedResult = "The buttons \"Close as Won\", \"Close as Lost\" and \"Close as Cancel\" are not available.";
                var result = Logger.LogGetExpectedResult(() =>
                    commandValues.Value, expectedResultList, expectedResult);

                Assert.IsFalse(result.Any(x => expectedResultList.Contains(x)), expectedResult);

            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [Priority(1)]
        [CrmUser(UserAlias,
            SecurityRole.DwBasicCrmAccess,
            SecurityRole.DwOpportunityManagement,
            UserGroup = UserGroup.Sales)]
        [TestProperty("TestCaseId", "33326")]
        public void AnOpportunityThatIsFunnelRelevantIsClosedAsCanceled()
        {
            var institution = testData["33326Institution"] as Account;


            try
            {
                Logger.NextStep(); // 2
                Login(XrmApp, UserAlias);

                Logger.NextStep();// 3
                Logger.Log<Action<string>>(XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(XrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");

                Logger.NextStep();// 4
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "New");

                var newOpportunityPage = new OpportunityEntity(XrmApp, XrmBrowser);
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                        "In the opportunity record, button 'Close as Canceled' is not available",
                        OpportunityLocators.CloseAsCanceled));

                var selectedOpportunityProbability = OpportunityConstants.Probability.FewChances;

                Logger.LogSet(() => newOpportunityPage.Topic, "Test Case 33326 Topic");
                Logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Attributes["name"]);
                Logger.LogSet(() => newOpportunityPage.Value, "500");
                Logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                Logger.LogSet(() => newOpportunityPage.Probability, selectedOpportunityProbability);
                Logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                Logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                Logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");


                Assert.IsTrue(
                    Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "'Is Funnel Relevant' is set to 'Yes' (default value)"), "funnel relevant is false");

                Logger.NextStep();// 5
                Logger.Log<Action>(XrmApp.Entity.Save);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                        "Button 'Close as Canceled' occurs to the right of button 'Refresh'.",
                        OpportunityLocators.CloseAsCanceled), "Button 'Close as Canceled' should occur to the right of button 'Refresh'.");

                Logger.NextStep();// 6
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Summary");

                Logger.LogSet(() => newOpportunityPage.AdditionalInformation, "33326 Additional Information");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                var endTime = DateTime.Now.AddSeconds(30);
                var success = false;
                do
                    if (newOpportunityPage.GetUnsavedChangesFooterVisible())
                    {
                        Logger.Log<Action<string, string>>(newOpportunityPage.ClickCommand, "Close as Canceled");
                        success = true;
                    }
                    else
                    {
                        newOpportunityPage.AdditionalInformation = "";
                        XrmApp.Entity.Save();
                        newOpportunityPage.AdditionalInformation = "33326 Additional Information";
                        XrmApp.ThinkTime(100);
                    }
                while (!success && DateTime.Now < endTime);

                Assert.IsTrue(success, "Didn't get 'unsaved changes' footer");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, true,
                    "A notification occurs: 'You have unsaved changes. Please save/refresh the opportunity to close it.'"), "No notification");

                Logger.NextStep();// 7
                Logger.Log<Action>(XrmApp.Entity.Save);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, false,
                    "The notification disappears"), "Notification still visible");

                Logger.NextStep();// 8
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Close as Canceled");

                var closingDialog = new OpportunityClosingDialog(XrmApp, XrmBrowser);

     

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true, "A closing dialog occurs.", true));

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select a reason for closing as canceled:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.ReasonForClosingField), "Field not marked as mandatory");


                Logger.NextStep();// 9
                Logger.Log<Action>(closingDialog.Finish);

                var validationFailedDialog = new OpportunityClosingValidationErrorDialog(XrmApp, XrmBrowser);

             

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                Logger.NextStep();// 10
                Logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, false,
                        "The notification is closed.", false),
                    "The notification is shown.");
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                    "The closing dialog occurs.", true), "Closing dialog not shown");

                Logger.NextStep(); // 11
                var longText = StringExtensions.Random(2002);

                Logger.LogSet(() => closingDialog.CommentWonOrCanceled, longText);


                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Maximum 2000 characters can be filled in. All further filled in characters are discarded without any notification. " +
                    "The field shows 4 rows of the text.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                Logger.NextStep();// 12
                Logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs again: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                Logger.NextStep();// 13
                Logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Insert a comment for closing as canceled:'' is still filled in.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                var picklist = closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog.ReasonForClosingField).ToList();
                Logger.NextStep(); // 14
                Logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                Logger.NextStep();// 15
                Logger.Log<Action>(closingDialog.Cancel);

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false, "The dialog is closed.", false));

                Func<string> getOpportunityProbability = () => newOpportunityPage.Probability;

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityProbability,
                    selectedOpportunityProbability, "The opportunity is not changed."), "Opportunity probability changed");

                Logger.NextStep();// 16

                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Close as Canceled");

                Assert.IsTrue(closingDialog.DialogIsOpen(true),
                    "Closing dialog not shown after 5 seconds");

                Func<string> getCloseAsWonDialogComment = () => closingDialog.CommentWonOrCanceled;

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getCloseAsWonDialogComment, "",
                    "The dialog opens up with the fields not filled in."));

                Logger.NextStep();// 17

                Logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);
                Logger.LogSet(() => closingDialog.CommentWonOrCanceled, "Comment for Canceled test case 33326");

                Logger.NextStep();// 18
                Logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(
                    Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false,
                        "The closing dialog is closed.", false), "closing dialog not closed");
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityProbability,
                    OpportunityConstants.Probability.OrderLost, "Field 'Probability (%)' is set to '0% - Order Lost'."),
                    "Opportunity probability changed");
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                Assert.IsTrue(Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "On tab 'Details', -field 'Is Funnel Relevant' is still set to 'Yes'"), "funnel relevant is false");
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Administration");
                Func<string> getOpportunityStatus = () => newOpportunityPage.Status;
                Assert.IsTrue(
                    Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatus, "Lost",
                        "On tab 'Administration', - Field 'Status' equals 'Lost'."), "status is not lost");
                Func<string> getOpportunityStatusReason = () => newOpportunityPage.StatusReason;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatusReason, picklist[picklist.Count - 1],
                        "On tab 'Administration', - Field 'Status Reason' is set to the value that has been selected in the closing dialog."),
                    "status reason differs from input");
                Func<string> getOpportunityClosingComment = () => newOpportunityPage.ClosingComment;
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityClosingComment, "Comment for Lost test case 33326",
                        "On tab 'Administration', - Field 'Closing Comment' is filled in with the content of dialog field 'Insert a comment for closing as won:' correctly."),
                    "Closing comment differs from input");

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetReadOnlyNotificationVisible, true,
                    "The opportunity is read-only."), "Read only Notification not found");

                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Funnel");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                //Func<string> getLatestOpportunityTimelineContent =
                //    () => newOpportunityPage.TimelineControl[0].Content;
                //if (getLatestOpportunityTimelineContent() == "Won Opportunity")
                //    getLatestOpportunityTimelineContent =
                //        () => newOpportunityPage.TimelineControl[1].Content;
                //Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getLatestOpportunityTimelineContent, $"won Opportunity for Institution {institution.Name}",
                //    "A new Auto Post is generated saying that the Opportunity is won for the Institution recorded in field 'Sold-To Party'."));

                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Won' is not available",
                    OpportunityLocators.CloseAsWon));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Cancelled' is not available",
                    OpportunityLocators.CloseAsCanceled));
                Assert.IsTrue(Logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                    "In the opportunity record, button 'Reopen Opportunity' is available.",
                    OpportunityLocators.ReopenOpportunity));
                Logger.NextStep();


            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [Priority(1)]
        [CrmUser(UserAlias,
    SecurityRole.DwBasicCrmAccess,
    SecurityRole.DwOpportunityManagement,
    UserGroup = UserGroup.Sales)]
        [TestProperty("TestCaseId", "63479")]
        public void AnOpportunityThatIsNotFunnelRelevantIsClosesAsCanceled()
        {

            var institution = testData["63479Institution"] as Account;
            var customerSolution = testData["63479CustomerSolution"];
            var expectedResult = "";

            try
            {
                Logger.NextStep();
                Login(XrmApp, UserAlias);

                Logger.NextStep();
                Logger.Log<Action<string>>(XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(XrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "New");

                Logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(XrmApp, XrmBrowser);


                Logger.LogSet(() => newOpportunityPage.Topic, "Test Case 63479 Topic");
                Logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Name);
                Logger.LogSet(() => newOpportunityPage.Value, "500");
                Logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                Logger.LogSet(() => newOpportunityPage.Probability, OpportunityConstants.Probability.GoodChance);
                Logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                Logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                Logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                Logger.LogSet(() => newOpportunityPage.CustomerSolution, customerSolution.Attributes["dw_name"]);
                var isFunnelRelevant = Logger.LogSet(() => newOpportunityPage.IsFunnelRelevant, false);


                Logger.NextStep();
                Logger.Log<Action>(XrmApp.Entity.Save);

                Logger.NextStep();
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, OpportunityLocators.CloseAsCanceled);

                var oppClosingDialog = new OpportunityClosingDialog(XrmApp, XrmBrowser);

                XrmApp.ThinkTime(500);
                Logger.LogSet(() => oppClosingDialog.CommentWonOrCanceled, "Test 63479 Comment!");
                Logger.LogSet(() => oppClosingDialog.ReasonForClosing, OpportunityConstants.ClosingReasonsCanceled[0]);
                Logger.Log<Action>(oppClosingDialog.Finish);

                XrmApp.ThinkTime(500);
                expectedResult = "The closing dialog is closed." +
                    "The opportunity is shown again." +
                    "On tab \"Details\", field \"Is Funnel Relevant\" is set to \"No\".";
                var check1 = Logger.LogExpectedResultCheck<Func<bool,bool>,bool>(oppClosingDialog.DialogIsOpen, false, expectedResult,false);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                var check2 = Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, false, expectedResult);

                Assert.IsTrue(check1 && check2, expectedResult);

            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [Priority(2)]
        [CrmUser(UserAlias,
SecurityRole.DwBasicCrmAccess,
SecurityRole.DwOpportunityManagement,
UserGroup = UserGroup.Sales)]
        [TestProperty("TestCaseId", "63526")]
        public void AnOpportunityThatIsNotFunnelRelevantIsClosedAsLost()
        {

            var institution = testData["63526Institution"] as Account;
            var customerSolution = testData["63526CustomerSolution"];
            var competitor = testData["63526Competitor"] as Competitor;
            var expectedResult = "";

            try
            {
                Logger.NextStep();
                Login(XrmApp, UserAlias);

                Logger.NextStep();
                Logger.Log<Action<string>>(XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(XrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "New");

                Logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(XrmApp, XrmBrowser);


                Logger.LogSet(() => newOpportunityPage.Topic, "Test Case 63526 Topic");
                Logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Name);
                Logger.LogSet(() => newOpportunityPage.Value, "500");
                Logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                Logger.LogSet(() => newOpportunityPage.Probability, OpportunityConstants.Probability.GoodChance);
                Logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                Logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                Logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                Logger.LogSet(() => newOpportunityPage.CustomerSolution, customerSolution.Attributes["dw_name"]);
                var isFunnelRelevant = Logger.LogSet(() => newOpportunityPage.IsFunnelRelevant, false);
                Logger.NextStep();
                Logger.Log<Action>(XrmApp.Entity.Save);

                Logger.NextStep();
                Logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                XrmApp.ThinkTime(200);

                Logger.LogSet(() => newOpportunityPage.SelectRecord, competitor.Name);
                Logger.Log<Action>(newOpportunityPage.ClickAddRecord);



                Logger.NextStep();
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, OpportunityLocators.CloseAsLost);

                Logger.NextStep();
                var oppClosingDialog = new OpportunityClosingDialog(XrmApp, XrmBrowser);

                XrmApp.ThinkTime(500);
                Logger.LogSet(() => oppClosingDialog.CommentLost, "Test 63526 Comment!");
                Logger.LogSet(() => oppClosingDialog.ReasonForClosing, OpportunityConstants.ClosingReasonsLost[5]);
                Logger.LogSet(() => oppClosingDialog.MainCompetitorThatWonTheDeal, competitor.Name);
                Logger.Log<Action>(oppClosingDialog.Finish);

                XrmApp.ThinkTime(500);
                expectedResult = "The closing dialog is closed." +
                    "The opportunity is shown again." +
                    "On tab 'Details', field 'Is Funnel Relevant' is set to 'Yes'." +
                    "On tab Administration.field 'Sales Abandoned' is set to 'No'.";
                var check1 = Logger.LogExpectedResultCheck<Func<bool,bool>,bool>(oppClosingDialog.DialogIsOpen, false, expectedResult,false);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                var check2 = Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true, expectedResult);
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Administration");
                var check3 = Logger.LogGetExpectedResultCheck(() => newOpportunityPage.SalesAbandoned, false, expectedResult);

                Assert.IsTrue(check1 && check2 && check3, expectedResult);
            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("Sales")]
        [TestCategory("Opportunity Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [Priority(2)]
        [CrmUser(UserAlias,
SecurityRole.DwBasicCrmAccess,
SecurityRole.DwOpportunityManagement,
UserGroup = UserGroup.Sales)]
        [TestProperty("TestCaseId", "63419")]
        public void AnOpportunityThatIsNotFunnelRelevantIsClosesAsWon()
        {

            var institution = testData["63419Institution"] as Account;
            var customerSolution = testData["63419CustomerSolution"];
            var expectedResult = "";

            try
            {
                Logger.NextStep();
                Login(XrmApp, UserAlias);

                Logger.NextStep();
                Logger.Log<Action<string>>(XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(XrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "New");

                Logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(XrmApp, XrmBrowser);


                Logger.LogSet(() => newOpportunityPage.Topic, "Test Case 63419 Topic");
                Logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Name);
                Logger.LogSet(() => newOpportunityPage.Value, "500");
                Logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                Logger.LogSet(() => newOpportunityPage.Probability, OpportunityConstants.Probability.GoodChance);
                Logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                Logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                Logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                Logger.LogSet(() => newOpportunityPage.CustomerSolution, customerSolution.Attributes["dw_name"]);
                var isFunnelRelevant = Logger.LogSet(() => newOpportunityPage.IsFunnelRelevant, false);


                Logger.NextStep();
                Logger.Log<Action>(XrmApp.Entity.Save);

                Logger.NextStep();
                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, OpportunityLocators.CloseAsWon);

                var oppClosingDialog = new OpportunityClosingDialog(XrmApp, XrmBrowser);

                XrmApp.ThinkTime(500);
                Logger.LogSet(() => oppClosingDialog.CommentWonOrCanceled, "Test 63419 Comment!");
                Logger.LogSet(() => oppClosingDialog.ReasonForClosing, OpportunityConstants.ClosingReasonsWon[5]);
                Logger.Log<Action>(oppClosingDialog.Finish);

                XrmApp.ThinkTime(500);
                expectedResult = "The closing dialog is closed." +
                    "The opportunity is shown again." +
                    "On tab \"Details\", field \"Is Funnel Relevant\" is set to \"Yes\".";
                var check1 = Logger.LogExpectedResultCheck<Func<bool,bool>,bool>(oppClosingDialog.DialogIsOpen, false, expectedResult,false);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                var check2 = Logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true, expectedResult);

                Assert.IsTrue(check1 && check2, expectedResult);

            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }

        }


    }
}
