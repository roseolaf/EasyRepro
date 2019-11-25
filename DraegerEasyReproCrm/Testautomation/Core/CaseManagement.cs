using System;
using System.Collections.Generic;
using Draeger.Dynamics365.Testautomation.Common;
using Draeger.Dynamics365.Testautomation.Common.EntityManager;
using Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator;
using Draeger.Dynamics365.Testautomation.Common.Enums;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Draeger.Dynamics365.Testautomation.Common.PageObjects;
using Infoman.Xrm.Services;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using ExecutionScope = Microsoft.VisualStudio.TestTools.UnitTesting.ExecutionScope;
using System.Linq;
using Draeger.Dynamics365.Testautomation.Common.Helper;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using Draeger.Testautomation.CredentialsManagerCore.Attributes;
using OpenQA.Selenium;
using Microsoft.Crm.Sdk.Messages;
using Entity = Microsoft.Xrm.Sdk.Entity;
using System.Globalization;

namespace Draeger.Dynamics365.Testautomation.Core
{
    [TestClass]
    public class CaseManagement : TestBase
    {
        protected static Dictionary<string, Entity> testData = new Dictionary<string, Entity>();

        private const string UserAlias = "UserAlias";

        [TestInitialize]
        public void TestClassSetup()
        {


            var TestCaseId = TestContext.Properties["TestCaseId"];

            switch (TestCaseId)
            {
                case "33392":
                    testData.Add("33392Inquiry", new TestcaseNameDecorator(
                                                        new OptionSetDecorator("dw_org_key_2", (int)Org_key_2_GlobalOptionSet._,
                                                        new OptionSetDecorator("dw_org_key_3", (int)Org_key_2_GlobalOptionSet._,
                                                        new OptionSetDecorator("dw_org_key_4", (int)Org_key_2_GlobalOptionSet._,
                                                    new AllAttributesDecorator(
                                                        new BaseComponent(logger))))))
                                                    .CreateEntityRecord(this, new Inquirytype()));

                    testData.Add("33392Institution", new TestcaseNameDecorator(
                                                        new BaseComponent(logger)).CreateEntityRecord(this, new Account()));

                    break;
                case "33396":
                    testData.Add("33396Institution", new TestcaseNameDecorator(
                                                        new BaseComponent(logger)).CreateEntityRecord(this, new Account()));

                    testData.Add("33396Contact", new ReferenceToDecorator("parentcustomerid", testData["33396Institution"],
                                                    new TestcaseNameDecorator(
                                                        new AllAttributesDecorator(
                                                            new BaseComponent(logger)))).CreateEntityRecord(this, new Contact()));

                    break;


                case "33397":
                    testData.Add("33397Institution", new TestcaseNameDecorator(
                                                        new BaseComponent(logger)).CreateEntityRecord(this, new Account()));
                    break;

                case "33394":
                    testData.Add("33394Institution", new TestcaseNameDecorator(
                                                        new AllAttributesDecorator(
                                                            new BaseComponent(logger))).CreateEntityRecord(this, new Account()));
                    break;
                case "33391":
                    testData.Add("33391Case", new CloseIncidentDecorator(
                                                    new TestcaseNameDecorator(
                                                            new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                                new AllAttributesDecorator(
                                                                    new BaseComponent(logger))))).CreateEntityRecord(this, new Incident()));
                    break;
                    
            }
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            foreach (var tD in testData)
            {
                new BaseComponent().DeleteEntityRecord(tD.Value.LogicalName, tD.Value.Id);

            }

        }

        [TestProperty("TestCaseId", "33392")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseCreateAndResolve()
        {
            var institution = testData["33392Institution"] as Account;
            var inquiry = testData["33392Inquiry"];
            string expectedResult = "";


            try
            {
                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(
                    xrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                logger.Log<Action<string, string, bool>>(
                    xrmApp.CommandBar.ClickCommand, "New Case");

                expectedResult = "The Case Form opens";

                var newCasePage = new CaseEntity(xrmApp, XrmBrowser);
                var caseForm = logger.LogGetExpectedResultCheck(() =>
                            newCasePage.GetFormLabel, "Case: Case: New Case", expectedResult);
                Assert.IsTrue(caseForm, "case form");

                logger.NextStep();
                logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);
                logger.LogSet(() =>
                    newCasePage.Origin, "Phone Call");
                logger.LogSet(() =>
                    newCasePage.CaseTitle, "Test Case 33392");
                logger.LogSet(() =>
                    newCasePage.LastName, "Last Name Test Case 33392");
                logger.LogSet(() =>
                    newCasePage.InquiryType, inquiry.Attributes["dw_name"]);
                //logger.Log<Action<string, string>>(
                //    xrmApp.Entity.SelectTab, "Case Resolution");
                logger.LogSet(() =>
                    newCasePage.ReportedTopic, "Reported Topic");
                logger.Log<Action>(
                    xrmApp.Entity.Save);

                expectedResult = "The case is saved";
                var createdOn = logger.LogExpectedResult<Func<string, string>, string>(
                    newCasePage.GetHeaderControlItem, "Case is created", expectedResult, "Created On");
                DateTime resultTime;
                var isDateTime = DateTime.TryParseExact(createdOn,"g" ,new CultureInfo("en-US"),DateTimeStyles.None, out resultTime);
                Assert.IsTrue(isDateTime, "No date found for created on");

                logger.NextStep();
                logger.Log<Action<string, Field>>(
                    xrmApp.BusinessProcessFlow.NextStage, "Capture");
                logger.Log<Action<string>>(
                    xrmApp.BusinessProcessFlow.SelectStage, "Resolve");
                logger.Log<Action<string, string>>(
                    xrmApp.BusinessProcessFlow.SetValue, CaseLocators.ResultFix, "Result Fix Test Case 33392");
                logger.Log<Action>(
                    newCasePage.ClickBusinessProcessFlowFinish);

                xrmApp.ThinkTime(8000);
                expectedResult = "The case is closed automatically (may take two seconds)";
                var status = logger.LogExpectedResultCheck<Func<string>, string>(
                    xrmApp.Entity.GetFooterStatusValue, "Resolved", expectedResult);
                Assert.IsTrue(status, "status");
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }


        }



        [TestProperty("TestCaseId", "33393")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseQualityRelevantDeviceComplaint()
        {
            try
            {
                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(
                    xrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                logger.Log<Action<string, string, bool>>(
                    xrmApp.CommandBar.ClickCommand, "New Case");
                logger.Log<Action<string, string>>(
                    xrmApp.Entity.SelectTab, "Quality Relevant Device Complaint");

                var newCasePage = new CaseEntity(xrmApp, XrmBrowser);
                logger.LogSet(() => newCasePage.DeviceComplaint, true);
                xrmApp.ThinkTime(2000);


                string expectedResult = "The following fields should be marked as mandatory (red star):\n"
                                                          + "Customer Raised Complaint\n"
                                                          + "Patient Involvement\n"
                                                          + "Product Malfunction\n"
                                                          + "Device Tested\n"
                                                          + "Description of Event\n"
                                                          + "Date / Time of Event\n"
                                                          + "Initial Reporter Relationship\n"
                                                          + "Injury Reported\n"
                                                          + "Generated Alarms\n"
                                                          + "Material Availability\n"
                                                          + "Occurence of Event\n"
                                                          + "User Facility CA Report\n"
                                                          + "Patient / Person injured";

                List<Requirement.Status> assertList = new List<Requirement.Status>();
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.CustomerRaisedComplaint));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.PatientInvolvement));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.ProductMalfunction));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.DeviceTested));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.DescriptionOfEvent));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.DateTimeOfEvent));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.InitialReporterRelationship));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.InjuryReported));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.GeneratedAlarms));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.OccurenceOfEvent));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.MaterialAvailability));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.PatientPersionInjured));
                assertList.Add(logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.UserfacilityCareport));
                Assert.IsTrue(assertList.All(x =>
                            x == Requirement.Status.Required1 ||
                            x == Requirement.Status.Required2), expectedResult);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }


        }

        [TestProperty("TestCaseId", "33396")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseContactLookup()
        {
            string expectedResult = "The related contact information is shown under CONTACT INFORMATION section. The Information can be overridden by the user. ";
       
            var institution = testData["33396Institution"] as Account;
            var relatedContact = testData["33396Contact"] as Contact;

            try
            {
                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(
                    xrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                logger.Log<Action<string, string, bool>>(
                    xrmApp.CommandBar.ClickCommand, "New Case");


                var newCasePage = new CaseEntity(xrmApp, XrmBrowser);
                logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);
                logger.LogSet(() =>
                    newCasePage.ExistingContact, relatedContact.LastName);
                xrmApp.ThinkTime(2000);



                var FirstName = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FirstName, relatedContact.FirstName, expectedResult);

                var LastName = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.LastName, relatedContact.LastName, expectedResult);

                var BusinessPhone = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.BusinessPhone, relatedContact.Telephone1, expectedResult);

                var MobilePhone = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.MobilePhone, relatedContact.MobilePhone, expectedResult);

                var EmailAddress = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.EmailAddress, relatedContact.EMailAddress1, expectedResult);

                var FaxNumber = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FaxNumber, relatedContact.Fax, expectedResult);

                Assert.IsTrue(FirstName && LastName && BusinessPhone && MobilePhone && EmailAddress && FaxNumber, expectedResult);


                var FirstNameNew = logger.LogSet(() =>
                        newCasePage.FirstName, relatedContact.FirstName + "New");

                // lastName field will autofill its value after clearing it
                var LastNameNew = logger.LogSet(() =>
                    newCasePage.LastName, relatedContact.LastName + "New");

                var BusinessPhoneNew = logger.LogSet(() =>
                    newCasePage.BusinessPhone, relatedContact.Telephone1 + "New");

                var MobilePhoneNew = logger.LogSet(() =>
                    newCasePage.MobilePhone, relatedContact.MobilePhone + "New");

                var EmailAddressNew = logger.LogSet(() =>
                    newCasePage.EmailAddress, relatedContact.EMailAddress1 + "New");

                var FaxNumberNew = logger.LogSet(() =>
                    newCasePage.FaxNumber, relatedContact.Fax + "New");

                FirstName = logger.LogGetExpectedResultCheck(() =>
                        newCasePage.FirstName, FirstNameNew, expectedResult);

                LastName = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.LastName, LastNameNew, expectedResult);

                BusinessPhone = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.BusinessPhone, BusinessPhoneNew, expectedResult);

                MobilePhone = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.MobilePhone, MobilePhoneNew, expectedResult);

                EmailAddress = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.EmailAddress, EmailAddressNew, expectedResult);

                FaxNumber = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FaxNumber, FaxNumberNew, expectedResult);

                Assert.IsTrue(FirstName && LastName && BusinessPhone && MobilePhone && EmailAddress && FaxNumber, expectedResult);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "33397")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseAlreadyInsertedContactInformationIsNotDeletedWhenInstutionIsChoosen()
        {
            string expectedResult = " The already inserted CONTACT INFORMATION is not deleted or overriden.";
            var institution = testData["33397Institution"] as Account;

            try
            {
                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(
                    xrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                logger.Log<Action<string, string, bool>>(
                    xrmApp.CommandBar.ClickCommand, "New Case");


                var newCasePage = new CaseEntity(xrmApp, XrmBrowser);

                var FirstNameField = logger.LogSet(() =>
                        newCasePage.FirstName, "FirstName");
                var LastNameField = logger.LogSet(() =>
                        newCasePage.LastName, "LastName");

                var BusinessPhoneField = logger.LogSet(() =>
                    newCasePage.BusinessPhone, "BusinessPhone");

                var MobilePhoneField = logger.LogSet(() =>
                    newCasePage.MobilePhone, "MobilePhone");

                var EmailAddressField = logger.LogSet(() =>
                    newCasePage.EmailAddress, "EMailAddress");

                var FaxNumberField = logger.LogSet(() =>
                    newCasePage.FaxNumber, "Fax");

                logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);

                xrmApp.ThinkTime(2000);

                var FirstName = logger.LogGetExpectedResultCheck(() =>
                          newCasePage.FirstName, FirstNameField, expectedResult);

                var LastName = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.LastName, LastNameField, expectedResult);

                var BusinessPhone = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.BusinessPhone, BusinessPhoneField, expectedResult);

                var MobilePhone = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.MobilePhone, MobilePhoneField, expectedResult);

                var EmailAddress = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.EmailAddress, EmailAddressField, expectedResult);

                var FaxNumber = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FaxNumber, FaxNumberField, expectedResult);

                Assert.IsTrue(FirstName && LastName && BusinessPhone && MobilePhone && EmailAddress && FaxNumber, expectedResult);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
        }


        [TestProperty("TestCaseId", "33394")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseInstitutionLookup()
        {
            string expectedResult = " The related institution information is shown under INSTITUTION INFORMATION section.";
            var institution = testData["33394Institution"] as Account;

            try
            {
                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(
                    xrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                logger.Log<Action<string, string, bool>>(
                    xrmApp.CommandBar.ClickCommand, "New Case");


                var countryName = institution.Countryid.Name;
                var newCasePage = new CaseEntity(xrmApp, XrmBrowser);

                logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);

                xrmApp.ThinkTime(8000);


                var Name = logger.LogGetExpectedResultCheck(() =>
                          newCasePage.Name, institution.Name, expectedResult);

                var Street = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.Street, institution.Address1_Line1, expectedResult);

                var City = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.City, institution.Address1_City, expectedResult);

                var Postalcode = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.Postalcode, institution.Address1_PostalCode, expectedResult);

                var Country = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.Country, countryName, expectedResult);


                Assert.IsTrue(Name && Street && City && Postalcode && Country, expectedResult);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "33328")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseOneTimeInstitionWorks()
        {
            string expectedResult = "The OneTimeInstution configured is set under Existing Institution.";

            try
            {
                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(
                    xrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                logger.Log<Action<string, string, bool>>(
                    xrmApp.CommandBar.ClickCommand, "New Case");

                var newCasePage = new CaseEntity(xrmApp, XrmBrowser);

                var name = logger.LogSet(() =>
                    newCasePage.Name, "OneTimeInstitution");
                xrmApp.ThinkTime(5000);

                var Country = logger.LogGetExpectedResultCheck(() =>
                    newCasePage.ExistingInstitution, name, expectedResult);

                Assert.IsTrue(Country);



            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "33391")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [TestCategory("Regression Test")]
        [TestCategory("Smoke Test")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseReopenAClosedRecord()
        {
            string expectedResult = "The case is reopened again and can be edited.";
            
            var caseData = testData["33391Case"] as Incident;
            
            try
            {
                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string, string>>(
                    xrmApp.Navigation.OpenSubArea, "Sales", "Cases");

                logger.Log<Action<string, bool>>(
                    xrmApp.Grid.Search, caseData.Title);

                logger.Log<Action<int>>(
                    xrmApp.Grid.HighLightRecord, 0);

                logger.Log<Action<int>>(
                    xrmApp.Grid.OpenRecord, 0);

                logger.Log<Action<string,string,bool>>(
                    xrmApp.CommandBar.ClickCommand,"Reactivate Case");


                var confirmDialog = logger.LogExpectedResultCheck<Func<bool,bool>,bool>(
                    xrmApp.Dialogs.ConfirmationDialog, true, expectedResult, true);

                xrmApp.ThinkTime(2000);

                var status = logger.LogExpectedResultCheck<Func<string>, string>(
                    xrmApp.Entity.GetFooterStatusValue, "Active", expectedResult);

               
                Assert.IsTrue(confirmDialog && status, expectedResult);

            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }

        }

    }
}
