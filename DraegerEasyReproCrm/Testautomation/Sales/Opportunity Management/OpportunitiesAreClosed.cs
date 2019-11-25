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
                    testData.Add("33318Institution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                            new SoldToDecorator(
                                new TestcaseNameDecorator(
                                    new BaseComponent(logger))))
                        .CreateEntityRecord(this, new Account()));
                    testData.Add("33318Competitor1",
                        new TestcaseNameDecorator(
                                new BaseComponent(logger))
                            .CreateEntityRecord(this, new Competitor()));
                    testData.Add("33318Competitor2",
                        new TestcaseNameDecorator(
                                new BaseComponent(logger))
                            .CreateEntityRecord(this, new Competitor()));
                    testData.Add("33318Competitor3",
                        new TestcaseNameDecorator(
                                new BaseComponent(logger))
                            .CreateEntityRecord(this, new Competitor()));

                    break;
                case "38264":
                    testData.Add("38264Institution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                        new SoldToDecorator(
                            new TestcaseNameDecorator(
                                new BaseComponent(logger))))
                        .CreateEntityRecord(this, new Account()));
                    break;

                case "33383":
                    testData.Add("33383Institution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                        new SoldToDecorator(
                            new TestcaseNameDecorator(
                                new BaseComponent(logger))))
                        .CreateEntityRecord(this, new Account()));

                    testData.Add("33383Opportunity",
                            new TestcaseNameDecorator(
                                new BaseComponent(logger))
                        .CreateEntityRecord(this, new Opportunity()));
                    break;

                case "33326":
                    testData.Add("33326Institution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                        new SoldToDecorator(
                            new TestcaseNameDecorator(
                                new BaseComponent(logger))))
                        .CreateEntityRecord(this, new Account()));
                    break;
                case "63479":
                    testData.Add("63479Institution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                        new SoldToDecorator(
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(logger))))
                                                        .CreateEntityRecord(this, new Account()));
                    testData.Add("63479CustomerSolution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                            new ReferenceToDecorator("dw_operatinginstitutionid", testData["63479Institution"],
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(logger))))
                                                        .CreateEntityRecord(this, new Customersolution()));
                    break;
                case "63419":
                    testData.Add("63419Institution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                        new SoldToDecorator(
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(logger))))
                                                        .CreateEntityRecord(this, new Account()));
                    testData.Add("63419CustomerSolution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                            new ReferenceToDecorator("dw_operatinginstitutionid", testData["63419Institution"],
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(logger))))
                                                        .CreateEntityRecord(this, new Customersolution()));
                    break;
                case "63526":
                    testData.Add("63526Institution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                        new SoldToDecorator(
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(logger))))
                                                        .CreateEntityRecord(this, new Account()));
                    testData.Add("63526CustomerSolution", new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                            new ReferenceToDecorator("dw_operatinginstitutionid", testData["63526Institution"],
                                                            new TestcaseNameDecorator(
                                                                new BaseComponent(logger))))
                                                        .CreateEntityRecord(this, new Customersolution()));
                    testData.Add("63526Competitor",
                                        new TestcaseNameDecorator(
                                            new BaseComponent(logger))
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
                logger.NextStep();
                Login(xrmApp, UserAlias);

                logger.NextStep();
                logger.Log<Action<string>>(xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(xrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "New");

                logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(xrmApp, XrmBrowser);
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                        "In the opportunity record, button 'Close as Won' is not available",
                        OpportunityLocators.CloseAsWon));

                var selectedOpportunityProbability = OpportunityConstants.Probability.FewChances;

                logger.LogSet(() => newOpportunityPage.Topic, "Test Case 38264 Topic");
                logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Attributes["name"]);
                logger.LogSet(() => newOpportunityPage.Value, "500");
                logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                logger.LogSet(() => newOpportunityPage.Probability, selectedOpportunityProbability);
                logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");


                Assert.IsTrue(
                    logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "'Is Funnel Relevant' is set to 'Yes' (default value)"), "funnel relevant is false");

                logger.NextStep();
                logger.Log<Action>(xrmApp.Entity.Save);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                        "Button 'Close as Won' occurs to the right of button 'Refresh'.",
                        OpportunityLocators.CloseAsWon), "Button 'Close as Won' should occur to the right of button 'Refresh'.");

                logger.NextStep();
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Summary");

                logger.LogSet(() => newOpportunityPage.AdditionalInformation, "38264 Additional Information");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                var endTime = DateTime.Now.AddSeconds(30);
                var success = false;
                do
                    if (newOpportunityPage.GetUnsavedChangesFooterVisible())
                    {
                        logger.Log<Action<string, string>>(newOpportunityPage.ClickCommand, "Close as Won");
                        success = true;
                    }
                    else
                    {
                        newOpportunityPage.AdditionalInformation = "";
                        xrmApp.Entity.Save();
                        newOpportunityPage.AdditionalInformation = "38264 Additional Information";
                        xrmApp.ThinkTime(100);
                    }
                while (!success && DateTime.Now < endTime);

                Assert.IsTrue(success, "Didn't get 'unsaved changes' footer");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, true,
                    "A notification occurs: 'You have unsaved changes. Please save/refresh the opportunity to close it.'"), "No notification");

                logger.NextStep();
                logger.Log<Action>(xrmApp.Entity.Save);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, false,
                    "The notification disappears"), "Notification still visible");

                logger.NextStep();
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "Close as Won");

                var closingDialog = new OpportunityClosingDialog(xrmApp, XrmBrowser);

      

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true, "The closing dialog occurs.", true));

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select a reason for closing as won:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.ReasonForClosingField), "Field not marked as mandatory");


                logger.NextStep();
                logger.Log<Action>(closingDialog.Finish);

                var validationFailedDialog = new OpportunityClosingValidationErrorDialog(xrmApp, XrmBrowser);

           
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                logger.NextStep();
                logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, false,
                        "The notification is closed.", false),
                    "The notification is shown.");
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                    "The closing dialog occurs.", true), "Closing dialog not shown");
                var longText = StringExtensions.Random(2002);

                logger.LogSet(() => closingDialog.CommentWonOrCanceled, longText);


                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Maximum 2000 characters can be filled in. All further filled in characters are discarded without any notification. " +
                    "The field shows 4 rows of the text.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                logger.NextStep();
                logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs again: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                logger.NextStep();
                logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Insert a comment for closing as won:'' is still filled in.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                var picklist = closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog.ReasonForClosingField).ToList();

                logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                logger.NextStep();
                logger.Log<Action>(closingDialog.Cancel);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false, "The dialog is closed.", false));


                Assert.IsTrue(logger.LogGetExpectedResultCheck(() => newOpportunityPage.Probability,
                    selectedOpportunityProbability, "The opportunity is not changed."), "Opportunity probability changed");

                logger.NextStep();

                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "Close as Won");

                Assert.IsTrue(closingDialog.DialogIsOpen(true),
                    "Closing dialog not shown after 5 seconds");


                Assert.IsTrue(logger.LogGetExpectedResultCheck(() => closingDialog.CommentWonOrCanceled, "",
                    "The dialog opens up with the fields not filled in."));

                logger.NextStep();

                logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);
                logger.LogSet(() => closingDialog.CommentWonOrCanceled, "Comment for Won test case 38264");

                logger.NextStep();
                logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(
                    logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false,
                        "The closing dialog is closed.", false), "closing dialog not closed");
                Assert.IsTrue(logger.LogGetExpectedResultCheck(() => newOpportunityPage.Probability,
                    OpportunityConstants.Probability.OrderReceived, "Field 'Probability (%)' is set to '100% - Order Received'."),
                    "Opportunity probability changed");
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                Assert.IsTrue(logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "On tab 'Details', -field 'Is Funnel Relevant' is still set to 'Yes'"), "funnel relevant is false");
                logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Administration");
                Func<string> getOpportunityStatus = () => newOpportunityPage.Status;
                Assert.IsTrue(
                    logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatus, "Won",
                        "On tab 'Administration', - Field 'Status' equals 'Won'."), "status is not won");
                Func<string> getOpportunityStatusReason = () => newOpportunityPage.StatusReason;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatusReason, picklist[picklist.Count - 1],
                        "On tab 'Administration', - Field 'Status Reason' is set to the value that has been selected in the closing dialog."),
                    "status reason differs from input");
                Func<string> getOpportunityClosingComment = () => newOpportunityPage.ClosingComment;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityClosingComment, "Comment for Won test case 38264",
                        "On tab 'Administration', - Field 'Closing Comment' is filled in with the content of dialog field 'Insert a comment for closing as won:' correctly."),
                    "Closing comment differs from input");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetReadOnlyNotificationVisible, true,
                    "The opportunity is read-only."), "Read only Notification not found");

                logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Funnel");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                Func<string> getLatestOpportunityTimelineContent =
                    () => newOpportunityPage.TimelineControl[0].Content;
                if (getLatestOpportunityTimelineContent() == "Won Opportunity")
                    getLatestOpportunityTimelineContent =
                        () => newOpportunityPage.TimelineControl[1].Content;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getLatestOpportunityTimelineContent, $"won Opportunity for Institution {institution.Name}",
                    "A new Auto Post is generated saying that the Opportunity is won for the Institution recorded in field 'Sold-To Party'."));

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Won' is not available",
                    OpportunityLocators.CloseAsWon));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Cancelled' is not available",
                    OpportunityLocators.CloseAsCanceled));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                    "In the opportunity record, button 'Reopen Opportunity' is available.",
                    OpportunityLocators.ReopenOpportunity));

            }
            catch (Exception e)
            {
                exception = e;

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
                logger.NextStep(); //Step 2
                Login(xrmApp, UserAlias);

                logger.NextStep(); //Step 3
                logger.Log<Action<string>>(xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(xrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "New");

                logger.NextStep(); //Step 4
                var newOpportunityPage = new OpportunityEntity(xrmApp, XrmBrowser);
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));

                var selectedOpportunityProbability = OpportunityConstants.Probability.FewChances;

                logger.LogSet(() => newOpportunityPage.Topic, "Test Case 33318 Topic");
                logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Attributes["name"]);
                logger.LogSet(() => newOpportunityPage.Value, "500");
                logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                logger.LogSet(() => newOpportunityPage.Probability, selectedOpportunityProbability);
                logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");


                Assert.IsTrue(
                    logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "'Is Funnel Relevant' is set to 'Yes' (default value)"), "funnel relevant is false");

                logger.NextStep(); //Step 5
                logger.Log<Action>(xrmApp.Entity.Save);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                        newOpportunityPage.GetCommandBarButtonAvailability, true,
                        "Button 'Close as Lost' occurs to the right of button 'Close as Won'.",
                        OpportunityLocators.CloseAsLost),
                    "Button 'Close as Lost' should occur to the right of button 'Close as Won'.");

                logger.NextStep(); //Step 6
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                xrmApp.ThinkTime(200);

                logger.LogSet(() => newOpportunityPage.SelectRecord, competitor1.Attributes["name"]);
                logger.Log<Action>(newOpportunityPage.ClickAddRecord);
                XrmBrowser.Browser.Driver.WaitForTransaction();

                logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                xrmApp.ThinkTime(200);

                logger.LogSet(() => newOpportunityPage.SelectRecord, competitor2.Attributes["name"]);
                logger.Log<Action>(newOpportunityPage.ClickAddRecord);
                XrmBrowser.Browser.Driver.WaitForTransaction();

                logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                xrmApp.ThinkTime(200);

                logger.LogSet(() => newOpportunityPage.SelectRecord, competitor3.Attributes["name"]);
                logger.Log<Action>(newOpportunityPage.ClickAddRecord);
                XrmBrowser.Browser.Driver.WaitForTransaction();

                //TODO: check if competitors in subgrid

                var competitorNames = new List<string>
                {
                    competitor1.Attributes["name"].ToString(),
                    competitor2.Attributes["name"].ToString(),
                    competitor3.Attributes["name"].ToString(),
                };
                var grid = new GridElement(xrmApp, XrmBrowser);
                var cellTitles =
                    logger.LogExpectedResult<Func<string, int, List<string>>, List<string>>(grid.GetCellTitles,
                        competitorNames,
                        "The selected competitors are available in subgrid \"Competitors\" correctly.",
                        "dataSetRoot_Competitors", 2);

                Assert.IsTrue(cellTitles.All(competitorNames.Contains), "The selected competitors are available in subgrid \"Competitors\" correctly.");


                logger.NextStep(); //Step 7
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Summary");

                logger.LogSet(() => newOpportunityPage.AdditionalInformation, "33318 Additional Information");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                var endTime = DateTime.Now.AddSeconds(30);
                var success = false;
                do
                    if (newOpportunityPage.GetUnsavedChangesFooterVisible())
                    {
                        logger.Log<Action<string, string>>(newOpportunityPage.ClickCommand, "Close as Lost");
                        success = true;
                    }
                    else
                    {
                        newOpportunityPage.AdditionalInformation = "";
                        xrmApp.Entity.Save();
                        newOpportunityPage.AdditionalInformation = "33318 Additional Information";
                        xrmApp.ThinkTime(100);
                    }
                while (!success && DateTime.Now < endTime);

                Assert.IsTrue(success, "Didn't get 'unsaved changes' footer");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, true,
                    "A notification occurs: 'You have unsaved changes. Please save/refresh the opportunity to close it.'"), "No notification");

                logger.NextStep(); //Step 8
                logger.Log<Action>(xrmApp.Entity.Save);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, false,
                    "The notification disappears"), "Notification still visible");

                logger.NextStep(); //Step 9
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "Close as Lost");

                var closingDialog = new OpportunityClosingDialog(xrmApp, XrmBrowser);

           

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                        "The closing dialog occurs.", true), "Closing dialog not shown");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select a reason for closing as lost:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.ReasonForClosingField), "Reason field not marked as mandatory");
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select the main competitor that won the deal:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.MainCompetitorThatWonTheDealField), "Competitor field not marked as mandatory");

                logger.NextStep(); //Step 10
                logger.Log<Action>(closingDialog.Finish);

                var validationFailedDialog = new OpportunityClosingValidationErrorDialog(xrmApp, XrmBrowser);

         
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                logger.NextStep(); //Step 11
                logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, false,
                        "The notification is closed.", false),
                    "The notification is shown.");


                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                    "The closing dialog occurs.", true), "Closing dialog not shown");

                logger.NextStep(); //Step 12
                var picklist = closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog.ReasonForClosingField).ToList();

                logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                logger.NextStep(); //Step 13

                logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                logger.NextStep(); //Step 14
                logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Insert a comment for closing as won:'' is still filled in.", picklist[picklist.Count - 1], closingDialog.ReasonForClosing));

                logger.NextStep(); //Step 15

                logger.Log<Action>(closingDialog.Cancel);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false, "The dialog is closed.", false));


                Assert.IsTrue(logger.LogGetExpectedResultCheck(()=>newOpportunityPage.Probability,
                    selectedOpportunityProbability, "The opportunity is not changed."), "Opportunity probability changed");

                logger.NextStep(); //Step 16
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "Close as Lost");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true, "The closing dialog opens up again.", true));


                logger.NextStep(); //Step 17
                var competitorList =
                    closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog
                        .MainCompetitorThatWonTheDealField).ToList();
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<IEnumerable<string>, string, bool>, bool>(Enumerable.Contains, true, "The picklist shows exactly the competitors that have been associated to the opportunity.", competitorList, competitor1.Attributes["name"]), "Competitor1 mising");
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<IEnumerable<string>, string, bool>, bool>(Enumerable.Contains, true, "The picklist shows exactly the competitors that have been associated to the opportunity.", competitorList, competitor2.Attributes["name"]), "Competitor2 mising");
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<IEnumerable<string>, string, bool>, bool>(Enumerable.Contains, true, "The picklist shows exactly the competitors that have been associated to the opportunity.", competitorList, competitor3.Attributes["name"]), "Competitor3 mising");
                logger.LogSet(() => closingDialog.MainCompetitorThatWonTheDeal, competitorList[0]);

                logger.NextStep(); //Step 18

                logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs again: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                logger.NextStep(); //Step 19
                logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Select a main competitor that won the deal' is still filled in.", competitorList[0], closingDialog.MainCompetitorThatWonTheDeal));

                logger.NextStep(); //Step 20
                logger.LogSet(() => closingDialog.SalesAbandoned, true);
                var commentLost = "Comment for Lost 33318";
                logger.LogSet(() => closingDialog.CommentLost, commentLost);
                logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                logger.NextStep();
                logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(
                    logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false,
                        "The closing dialog is closed.", false), "closing dialog not closed");
                Assert.IsTrue(logger.LogGetExpectedResultCheck(()=>newOpportunityPage.Probability,
                    OpportunityConstants.Probability.OrderLost, "Field 'Probability(%)' is set to '0 % -Order Lost'."), "Opportunity probability not 0%");

                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                Assert.IsTrue(logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                    "On tab 'Details', -field 'Is Funnel Relevant' is still set to 'Yes'"), "funnel relevant is false");

                logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Administration");
                Assert.IsTrue(
                    logger.LogGetExpectedResultCheck(()=>newOpportunityPage.Status, "Lost",
                        "On tab 'Administration', - Field 'Status' equals 'Lost'."), "status is not lost");
                Func<string> getOpportunityStatusReason = () => newOpportunityPage.StatusReason;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatusReason, picklist[picklist.Count - 1],
                        "On tab 'Administration', - Field 'Status Reason' is set to the value that has been selected in the closing dialog."),
                    "status reason differs from input");
                Func<string> getOpportunityClosingComment = () => newOpportunityPage.ClosingComment;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityClosingComment, commentLost,
                        "On tab 'Administration', - Field 'Closing Comment' is filled in with the content of dialog field 'Insert a comment for closing as lost:' correctly."),
                    "Closing comment differs from input");
                Func<bool> getOpportunitySalesAbandoned = () => newOpportunityPage.SalesAbandoned;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(getOpportunitySalesAbandoned, true,
                        "On tab 'Administration', - Field 'Sales Abandoned' is set to'Yes'"),
                    "Sales abandoned not set");
                Func<string> getOpportunityCompetitorThatWon = () => newOpportunityPage.MainCompetitor;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityCompetitorThatWon,
                        competitorList[0], "On tab 'Administration', - Field 'Main Competitor' is filled with the value selected in the dialog"),
                    "Main competitor differs from selection");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetReadOnlyNotificationVisible, true,
                    "The opportunity is read-only."), "Read only Notification not found");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Won' is not available",
                    OpportunityLocators.CloseAsWon));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Cancelled' is not available",
                    OpportunityLocators.CloseAsCanceled));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                    "In the opportunity record, button 'Reopen Opportunity' is available.",
                    OpportunityLocators.ReopenOpportunity));


            }
            catch (Exception e)
            {
                exception = e;
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
                logger.NextStep();
                Login(xrmApp, UserAlias);
                logger.NextStep();

                logger.Log<Action<string, Guid>>(
                    xrmApp.Entity.OpenEntity, opportunity.LogicalName, opportunity.Id);

                var commandValues = logger.Log<Func<bool, int, BrowserCommandResult<List<string>>>, BrowserCommandResult<List<string>>>(
                    xrmApp.CommandBar.GetCommandValues);


                List<string> expectedResultList = new List<string>
                {
                    OpportunityLocators.CloseAsWon,
                    OpportunityLocators.CloseAsLost,
                    OpportunityLocators.CloseAsCanceled
                };

                var expectedResult = "The buttons \"Close as Won\", \"Close as Lost\" and \"Close as Cancel\" are not available.";
                var result = logger.LogGetExpectedResult(() =>
                    commandValues.Value, expectedResultList, expectedResult);

                Assert.IsFalse(result.Any(x => expectedResultList.Contains(x)), expectedResult);

            }
            catch (Exception e)
            {
                exception = e;
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
                logger.NextStep(); // 2
                Login(xrmApp, UserAlias);

                logger.NextStep();// 3
                logger.Log<Action<string>>(xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(xrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");

                logger.NextStep();// 4
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "New");

                var newOpportunityPage = new OpportunityEntity(xrmApp, XrmBrowser);
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                        "In the opportunity record, button 'Close as Canceled' is not available",
                        OpportunityLocators.CloseAsCanceled));

                var selectedOpportunityProbability = OpportunityConstants.Probability.FewChances;

                logger.LogSet(() => newOpportunityPage.Topic, "Test Case 33326 Topic");
                logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Attributes["name"]);
                logger.LogSet(() => newOpportunityPage.Value, "500");
                logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                logger.LogSet(() => newOpportunityPage.Probability, selectedOpportunityProbability);
                logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");


                Assert.IsTrue(
                    logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "'Is Funnel Relevant' is set to 'Yes' (default value)"), "funnel relevant is false");

                logger.NextStep();// 5
                logger.Log<Action>(xrmApp.Entity.Save);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                        "Button 'Close as Canceled' occurs to the right of button 'Refresh'.",
                        OpportunityLocators.CloseAsCanceled), "Button 'Close as Canceled' should occur to the right of button 'Refresh'.");

                logger.NextStep();// 6
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Summary");

                logger.LogSet(() => newOpportunityPage.AdditionalInformation, "33326 Additional Information");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                var endTime = DateTime.Now.AddSeconds(30);
                var success = false;
                do
                    if (newOpportunityPage.GetUnsavedChangesFooterVisible())
                    {
                        logger.Log<Action<string, string>>(newOpportunityPage.ClickCommand, "Close as Canceled");
                        success = true;
                    }
                    else
                    {
                        newOpportunityPage.AdditionalInformation = "";
                        xrmApp.Entity.Save();
                        newOpportunityPage.AdditionalInformation = "33326 Additional Information";
                        xrmApp.ThinkTime(100);
                    }
                while (!success && DateTime.Now < endTime);

                Assert.IsTrue(success, "Didn't get 'unsaved changes' footer");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, true,
                    "A notification occurs: 'You have unsaved changes. Please save/refresh the opportunity to close it.'"), "No notification");

                logger.NextStep();// 7
                logger.Log<Action>(xrmApp.Entity.Save);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetUnsavedChangesNotificationVisible, false,
                    "The notification disappears"), "Notification still visible");

                logger.NextStep();// 8
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "Close as Canceled");

                var closingDialog = new OpportunityClosingDialog(xrmApp, XrmBrowser);

     

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true, "A closing dialog occurs.", true));

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<By, bool>, bool>(closingDialog.GetIsMandatory, true,
                    "Field 'Select a reason for closing as canceled:' is mandatory = marked with a red border.",
                    OpportunityLocators.ClosingDialog.ReasonForClosingField), "Field not marked as mandatory");


                logger.NextStep();// 9
                logger.Log<Action>(closingDialog.Finish);

                var validationFailedDialog = new OpportunityClosingValidationErrorDialog(xrmApp, XrmBrowser);

             

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                logger.NextStep();// 10
                logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, false,
                        "The notification is closed.", false),
                    "The notification is shown.");
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, true,
                    "The closing dialog occurs.", true), "Closing dialog not shown");

                logger.NextStep(); // 11
                var longText = StringExtensions.Random(2002);

                logger.LogSet(() => closingDialog.CommentWonOrCanceled, longText);


                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Maximum 2000 characters can be filled in. All further filled in characters are discarded without any notification. " +
                    "The field shows 4 rows of the text.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                logger.NextStep();// 12
                logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(validationFailedDialog.DialogIsOpen, true,
                        "A notification window occurs again: 'Validation error Please fill in all mandatory fields to close the Opportunity'", true),
                    "Validation error dialog not shown after 5 seconds");

                logger.NextStep();// 13
                logger.Log<Func<bool>>(validationFailedDialog.PressOk);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, string, int>, int>(string.Compare, 0,
                    "Field 'Insert a comment for closing as canceled:'' is still filled in.", longText.Substring(0, 2000), closingDialog.CommentWonOrCanceled));

                var picklist = closingDialog.GetOptionsForMultiOptionField(OpportunityLocators.ClosingDialog.ReasonForClosingField).ToList();
                logger.NextStep(); // 14
                logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);

                logger.NextStep();// 15
                logger.Log<Action>(closingDialog.Cancel);

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false, "The dialog is closed.", false));

                Func<string> getOpportunityProbability = () => newOpportunityPage.Probability;

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityProbability,
                    selectedOpportunityProbability, "The opportunity is not changed."), "Opportunity probability changed");

                logger.NextStep();// 16

                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "Close as Canceled");

                Assert.IsTrue(closingDialog.DialogIsOpen(true),
                    "Closing dialog not shown after 5 seconds");

                Func<string> getCloseAsWonDialogComment = () => closingDialog.CommentWonOrCanceled;

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getCloseAsWonDialogComment, "",
                    "The dialog opens up with the fields not filled in."));

                logger.NextStep();// 17

                logger.LogSet(() => closingDialog.ReasonForClosing, picklist[picklist.Count - 1]);
                logger.LogSet(() => closingDialog.CommentWonOrCanceled, "Comment for Canceled test case 33326");

                logger.NextStep();// 18
                logger.Log<Action>(closingDialog.Finish);

                Assert.IsTrue(
                    logger.LogExpectedResultCheck<Func<bool, bool>, bool>(closingDialog.DialogIsOpen, false,
                        "The closing dialog is closed.", false), "closing dialog not closed");
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityProbability,
                    OpportunityConstants.Probability.OrderLost, "Field 'Probability (%)' is set to '0% - Order Lost'."),
                    "Opportunity probability changed");
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                Assert.IsTrue(logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true,
                        "On tab 'Details', -field 'Is Funnel Relevant' is still set to 'Yes'"), "funnel relevant is false");
                logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Administration");
                Func<string> getOpportunityStatus = () => newOpportunityPage.Status;
                Assert.IsTrue(
                    logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatus, "Lost",
                        "On tab 'Administration', - Field 'Status' equals 'Lost'."), "status is not lost");
                Func<string> getOpportunityStatusReason = () => newOpportunityPage.StatusReason;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityStatusReason, picklist[picklist.Count - 1],
                        "On tab 'Administration', - Field 'Status Reason' is set to the value that has been selected in the closing dialog."),
                    "status reason differs from input");
                Func<string> getOpportunityClosingComment = () => newOpportunityPage.ClosingComment;
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getOpportunityClosingComment, "Comment for Lost test case 33326",
                        "On tab 'Administration', - Field 'Closing Comment' is filled in with the content of dialog field 'Insert a comment for closing as won:' correctly."),
                    "Closing comment differs from input");

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<bool>, bool>(newOpportunityPage.GetReadOnlyNotificationVisible, true,
                    "The opportunity is read-only."), "Read only Notification not found");

                logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Funnel");
                XrmBrowser.Browser.Driver.WaitForTransaction();

                //Func<string> getLatestOpportunityTimelineContent =
                //    () => newOpportunityPage.TimelineControl[0].Content;
                //if (getLatestOpportunityTimelineContent() == "Won Opportunity")
                //    getLatestOpportunityTimelineContent =
                //        () => newOpportunityPage.TimelineControl[1].Content;
                //Assert.IsTrue(logger.LogExpectedResultCheck<Func<string>, string>(getLatestOpportunityTimelineContent, $"won Opportunity for Institution {institution.Name}",
                //    "A new Auto Post is generated saying that the Opportunity is won for the Institution recorded in field 'Sold-To Party'."));

                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Won' is not available",
                    OpportunityLocators.CloseAsWon));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Lost' is not available",
                    OpportunityLocators.CloseAsLost));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, false,
                    "In the opportunity record, button 'Close as Cancelled' is not available",
                    OpportunityLocators.CloseAsCanceled));
                Assert.IsTrue(logger.LogExpectedResultCheck<Func<string, bool>, bool>(
                    newOpportunityPage.GetCommandBarButtonAvailability, true,
                    "In the opportunity record, button 'Reopen Opportunity' is available.",
                    OpportunityLocators.ReopenOpportunity));
                logger.NextStep();


            }
            catch (Exception e)
            {
                exception = e;
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
                logger.NextStep();
                Login(xrmApp, UserAlias);

                logger.NextStep();
                logger.Log<Action<string>>(xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(xrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "New");

                logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(xrmApp, XrmBrowser);


                logger.LogSet(() => newOpportunityPage.Topic, "Test Case 63479 Topic");
                logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Name);
                logger.LogSet(() => newOpportunityPage.Value, "500");
                logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                logger.LogSet(() => newOpportunityPage.Probability, OpportunityConstants.Probability.GoodChance);
                logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                logger.LogSet(() => newOpportunityPage.CustomerSolution, customerSolution.Attributes["dw_name"]);
                var isFunnelRelevant = logger.LogSet(() => newOpportunityPage.IsFunnelRelevant, false);


                logger.NextStep();
                logger.Log<Action>(xrmApp.Entity.Save);

                logger.NextStep();
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, OpportunityLocators.CloseAsCanceled);

                var oppClosingDialog = new OpportunityClosingDialog(xrmApp, XrmBrowser);

                xrmApp.ThinkTime(500);
                logger.LogSet(() => oppClosingDialog.CommentWonOrCanceled, "Test 63479 Comment!");
                logger.LogSet(() => oppClosingDialog.ReasonForClosing, OpportunityConstants.ClosingReasonsCanceled[0]);
                logger.Log<Action>(oppClosingDialog.Finish);

                xrmApp.ThinkTime(500);
                expectedResult = "The closing dialog is closed." +
                    "The opportunity is shown again." +
                    "On tab \"Details\", field \"Is Funnel Relevant\" is set to \"No\".";
                var check1 = logger.LogExpectedResultCheck<Func<bool,bool>,bool>(oppClosingDialog.DialogIsOpen, false, expectedResult,false);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                var check2 = logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, false, expectedResult);

                Assert.IsTrue(check1 && check2, expectedResult);

            }
            catch (Exception e)
            {
                exception = e;
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
                logger.NextStep();
                Login(xrmApp, UserAlias);

                logger.NextStep();
                logger.Log<Action<string>>(xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(xrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "New");

                logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(xrmApp, XrmBrowser);


                logger.LogSet(() => newOpportunityPage.Topic, "Test Case 63526 Topic");
                logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Name);
                logger.LogSet(() => newOpportunityPage.Value, "500");
                logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                logger.LogSet(() => newOpportunityPage.Probability, OpportunityConstants.Probability.GoodChance);
                logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                logger.LogSet(() => newOpportunityPage.CustomerSolution, customerSolution.Attributes["dw_name"]);
                var isFunnelRelevant = logger.LogSet(() => newOpportunityPage.IsFunnelRelevant, false);
                logger.NextStep();
                logger.Log<Action>(xrmApp.Entity.Save);

                logger.NextStep();
                logger.Log<Action<string, string>>(newOpportunityPage.SectionMoreCommands, "Competitors", "Add Existing Competitor");

                xrmApp.ThinkTime(200);

                logger.LogSet(() => newOpportunityPage.SelectRecord, competitor.Name);
                logger.Log<Action>(newOpportunityPage.ClickAddRecord);



                logger.NextStep();
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, OpportunityLocators.CloseAsLost);

                logger.NextStep();
                var oppClosingDialog = new OpportunityClosingDialog(xrmApp, XrmBrowser);

                xrmApp.ThinkTime(500);
                logger.LogSet(() => oppClosingDialog.CommentLost, "Test 63526 Comment!");
                logger.LogSet(() => oppClosingDialog.ReasonForClosing, OpportunityConstants.ClosingReasonsLost[5]);
                logger.LogSet(() => oppClosingDialog.MainCompetitorThatWonTheDeal, competitor.Name);
                logger.Log<Action>(oppClosingDialog.Finish);

                xrmApp.ThinkTime(500);
                expectedResult = "The closing dialog is closed." +
                    "The opportunity is shown again." +
                    "On tab 'Details', field 'Is Funnel Relevant' is set to 'Yes'." +
                    "On tab Administration.field 'Sales Abandoned' is set to 'No'.";
                var check1 = logger.LogExpectedResultCheck<Func<bool,bool>,bool>(oppClosingDialog.DialogIsOpen, false, expectedResult,false);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                var check2 = logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true, expectedResult);
                logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Administration");
                var check3 = logger.LogGetExpectedResultCheck(() => newOpportunityPage.SalesAbandoned, false, expectedResult);

                Assert.IsTrue(check1 && check2 && check3, expectedResult);
            }
            catch (Exception e)
            {
                exception = e;
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
                logger.NextStep();
                Login(xrmApp, UserAlias);

                logger.NextStep();
                logger.Log<Action<string>>(xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(xrmApp.Navigation.OpenSubArea, "Sales", "Opportunities");
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "New");

                logger.NextStep();
                var newOpportunityPage = new OpportunityEntity(xrmApp, XrmBrowser);


                logger.LogSet(() => newOpportunityPage.Topic, "Test Case 63419 Topic");
                logger.LogSet(() => newOpportunityPage.SoldToParty, institution.Name);
                logger.LogSet(() => newOpportunityPage.Value, "500");
                logger.LogSet(() => newOpportunityPage.Currency, "Euro");
                logger.LogSet(() => newOpportunityPage.Probability, OpportunityConstants.Probability.GoodChance);
                logger.LogSet(() => newOpportunityPage.OrderEntryDate, DateTime.Today.AddDays(14));
                logger.LogSet(() => newOpportunityPage.CustomerDeliveryDate, DateTime.Today.AddDays(30));
                logger.LogSet(() => newOpportunityPage.TimingAccuracy, OpportunityConstants.TimingAccuracy.Poor);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");

                logger.LogSet(() => newOpportunityPage.CustomerSolution, customerSolution.Attributes["dw_name"]);
                var isFunnelRelevant = logger.LogSet(() => newOpportunityPage.IsFunnelRelevant, false);


                logger.NextStep();
                logger.Log<Action>(xrmApp.Entity.Save);

                logger.NextStep();
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, OpportunityLocators.CloseAsWon);

                var oppClosingDialog = new OpportunityClosingDialog(xrmApp, XrmBrowser);

                xrmApp.ThinkTime(500);
                logger.LogSet(() => oppClosingDialog.CommentWonOrCanceled, "Test 63419 Comment!");
                logger.LogSet(() => oppClosingDialog.ReasonForClosing, OpportunityConstants.ClosingReasonsWon[5]);
                logger.Log<Action>(oppClosingDialog.Finish);

                xrmApp.ThinkTime(500);
                expectedResult = "The closing dialog is closed." +
                    "The opportunity is shown again." +
                    "On tab \"Details\", field \"Is Funnel Relevant\" is set to \"Yes\".";
                var check1 = logger.LogExpectedResultCheck<Func<bool,bool>,bool>(oppClosingDialog.DialogIsOpen, false, expectedResult,false);
                //logger.Log<Action<string, string>>(xrmApp.Entity.SelectTab, "Details");
                var check2 = logger.LogGetExpectedResultCheck(() => newOpportunityPage.IsFunnelRelevant, true, expectedResult);

                Assert.IsTrue(check1 && check2, expectedResult);

            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }

        }


    }
}
