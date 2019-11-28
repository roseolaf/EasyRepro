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
                                                        new BaseComponent(Logger))))))
                                                    .CreateEntityRecord(this, new Inquirytype()));

                    testData.Add("33392Institution", new TestcaseNameDecorator(
                                                        new BaseComponent(Logger)).CreateEntityRecord(this, new Account()));

                    break;
                case "33396":
                    testData.Add("33396Institution", new TestcaseNameDecorator(
                                                        new BaseComponent(Logger)).CreateEntityRecord(this, new Account()));

                    testData.Add("33396Contact", new ReferenceToDecorator("parentcustomerid", testData["33396Institution"],
                                                    new TestcaseNameDecorator(
                                                        new AllAttributesDecorator(
                                                            new BaseComponent(Logger)))).CreateEntityRecord(this, new Contact()));

                    break;


                case "33397":
                    testData.Add("33397Institution", new TestcaseNameDecorator(
                                                        new BaseComponent(Logger)).CreateEntityRecord(this, new Account()));
                    break;

                case "33394":
                    testData.Add("33394Institution", new TestcaseNameDecorator(
                                                        new AllAttributesDecorator(
                                                            new BaseComponent(Logger))).CreateEntityRecord(this, new Account()));
                    break;
                case "33391":
                    testData.Add("33391Case", new CloseIncidentDecorator(
                                                    new TestcaseNameDecorator(
                                                            new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                                                new AllAttributesDecorator(
                                                                    new BaseComponent(Logger))))).CreateEntityRecord(this, new Incident()));
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
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "New Case");

                expectedResult = "The Case Form opens";

                var newCasePage = new CaseEntity(XrmApp, XrmBrowser);
                var caseForm = Logger.LogGetExpectedResultCheck(() =>
                            newCasePage.GetFormLabel, "Case: Case: New Case", expectedResult);
                Assert.IsTrue(caseForm, "case form");

                Logger.NextStep();
                Logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);
                Logger.LogSet(() =>
                    newCasePage.Origin, "Phone Call");
                Logger.LogSet(() =>
                    newCasePage.CaseTitle, "Test Case 33392");
                Logger.LogSet(() =>
                    newCasePage.LastName, "Last Name Test Case 33392");
                Logger.LogSet(() =>
                    newCasePage.InquiryType, inquiry.Attributes["dw_name"]);
                //logger.Log<Action<string, string>>(
                //    xrmApp.Entity.SelectTab, "Case Resolution");
                Logger.LogSet(() =>
                    newCasePage.ReportedTopic, "Reported Topic");
                Logger.Log<Action>(
                    XrmApp.Entity.Save);

                expectedResult = "The case is saved";
                var createdOn = Logger.LogExpectedResult<Func<string, string>, string>(
                    newCasePage.GetHeaderControlItem, "Case is created", expectedResult, "Created On");
                DateTime resultTime;
                var isDateTime = DateTime.TryParseExact(createdOn,"g" ,new CultureInfo("en-US"),DateTimeStyles.None, out resultTime);
                Assert.IsTrue(isDateTime, "No date found for created on");

                Logger.NextStep();
                Logger.Log<Action<string, Field>>(
                    XrmApp.BusinessProcessFlow.NextStage, "Capture");
                Logger.Log<Action<string>>(
                    XrmApp.BusinessProcessFlow.SelectStage, "Resolve");
                Logger.Log<Action<string, string>>(
                    XrmApp.BusinessProcessFlow.SetValue, CaseLocators.ResultFix, "Result Fix Test Case 33392");
                Logger.Log<Action>(
                    newCasePage.ClickBusinessProcessFlowFinish);

                XrmApp.ThinkTime(8000);
                expectedResult = "The case is closed automatically (may take two seconds)";
                var status = Logger.LogExpectedResultCheck<Func<string>, string>(
                    XrmApp.Entity.GetFooterStatusValue, "Resolved", expectedResult);
                Assert.IsTrue(status, "status");
            }
            catch (Exception e)
            {
                Exception = e;
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
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "New Case");
                Logger.Log<Action<string, string>>(
                    XrmApp.Entity.SelectTab, "Quality Relevant Device Complaint");

                var newCasePage = new CaseEntity(XrmApp, XrmBrowser);
                Logger.LogSet(() => newCasePage.DeviceComplaint, true);
                XrmApp.ThinkTime(2000);


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
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.CustomerRaisedComplaint));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.PatientInvolvement));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.ProductMalfunction));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.DeviceTested));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.DescriptionOfEvent));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.DateTimeOfEvent));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.InitialReporterRelationship));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.InjuryReported));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.GeneratedAlarms));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.OccurenceOfEvent));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.MaterialAvailability));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.PatientPersionInjured));
                assertList.Add(Logger.LogExpectedResult<Func<string, Requirement.Status>, Requirement.Status>(
                    newCasePage.GetFieldRequirement, "required", expectedResult, CaseLocators.UserfacilityCareport));
                Assert.IsTrue(assertList.All(x =>
                            x == Requirement.Status.Required1 ||
                            x == Requirement.Status.Required2), expectedResult);
            }
            catch (Exception e)
            {
                Exception = e;
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
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "New Case");


                var newCasePage = new CaseEntity(XrmApp, XrmBrowser);
                Logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);
                Logger.LogSet(() =>
                    newCasePage.ExistingContact, relatedContact.LastName);
                XrmApp.ThinkTime(2000);



                var FirstName = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FirstName, relatedContact.FirstName, expectedResult);

                var LastName = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.LastName, relatedContact.LastName, expectedResult);

                var BusinessPhone = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.BusinessPhone, relatedContact.Telephone1, expectedResult);

                var MobilePhone = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.MobilePhone, relatedContact.MobilePhone, expectedResult);

                var EmailAddress = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.EmailAddress, relatedContact.EMailAddress1, expectedResult);

                var FaxNumber = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FaxNumber, relatedContact.Fax, expectedResult);

                Assert.IsTrue(FirstName && LastName && BusinessPhone && MobilePhone && EmailAddress && FaxNumber, expectedResult);


                var FirstNameNew = Logger.LogSet(() =>
                        newCasePage.FirstName, relatedContact.FirstName + "New");

                // lastName field will autofill its value after clearing it
                var LastNameNew = Logger.LogSet(() =>
                    newCasePage.LastName, relatedContact.LastName + "New");

                var BusinessPhoneNew = Logger.LogSet(() =>
                    newCasePage.BusinessPhone, relatedContact.Telephone1 + "New");

                var MobilePhoneNew = Logger.LogSet(() =>
                    newCasePage.MobilePhone, relatedContact.MobilePhone + "New");

                var EmailAddressNew = Logger.LogSet(() =>
                    newCasePage.EmailAddress, relatedContact.EMailAddress1 + "New");

                var FaxNumberNew = Logger.LogSet(() =>
                    newCasePage.FaxNumber, relatedContact.Fax + "New");

                FirstName = Logger.LogGetExpectedResultCheck(() =>
                        newCasePage.FirstName, FirstNameNew, expectedResult);

                LastName = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.LastName, LastNameNew, expectedResult);

                BusinessPhone = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.BusinessPhone, BusinessPhoneNew, expectedResult);

                MobilePhone = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.MobilePhone, MobilePhoneNew, expectedResult);

                EmailAddress = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.EmailAddress, EmailAddressNew, expectedResult);

                FaxNumber = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FaxNumber, FaxNumberNew, expectedResult);

                Assert.IsTrue(FirstName && LastName && BusinessPhone && MobilePhone && EmailAddress && FaxNumber, expectedResult);
            }
            catch (Exception e)
            {
                Exception = e;
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
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "New Case");


                var newCasePage = new CaseEntity(XrmApp, XrmBrowser);

                var FirstNameField = Logger.LogSet(() =>
                        newCasePage.FirstName, "FirstName");
                var LastNameField = Logger.LogSet(() =>
                        newCasePage.LastName, "LastName");

                var BusinessPhoneField = Logger.LogSet(() =>
                    newCasePage.BusinessPhone, "BusinessPhone");

                var MobilePhoneField = Logger.LogSet(() =>
                    newCasePage.MobilePhone, "MobilePhone");

                var EmailAddressField = Logger.LogSet(() =>
                    newCasePage.EmailAddress, "EMailAddress");

                var FaxNumberField = Logger.LogSet(() =>
                    newCasePage.FaxNumber, "Fax");

                Logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);

                XrmApp.ThinkTime(2000);

                var FirstName = Logger.LogGetExpectedResultCheck(() =>
                          newCasePage.FirstName, FirstNameField, expectedResult);

                var LastName = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.LastName, LastNameField, expectedResult);

                var BusinessPhone = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.BusinessPhone, BusinessPhoneField, expectedResult);

                var MobilePhone = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.MobilePhone, MobilePhoneField, expectedResult);

                var EmailAddress = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.EmailAddress, EmailAddressField, expectedResult);

                var FaxNumber = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.FaxNumber, FaxNumberField, expectedResult);

                Assert.IsTrue(FirstName && LastName && BusinessPhone && MobilePhone && EmailAddress && FaxNumber, expectedResult);
            }
            catch (Exception e)
            {
                Exception = e;
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
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "New Case");


                var countryName = institution.Countryid.Name;
                var newCasePage = new CaseEntity(XrmApp, XrmBrowser);

                Logger.LogSet(() =>
                    newCasePage.ExistingInstitution, institution.Name);

                XrmApp.ThinkTime(8000);


                var Name = Logger.LogGetExpectedResultCheck(() =>
                          newCasePage.Name, institution.Name, expectedResult);

                var Street = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.Street, institution.Address1_Line1, expectedResult);

                var City = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.City, institution.Address1_City, expectedResult);

                var Postalcode = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.Postalcode, institution.Address1_PostalCode, expectedResult);

                var Country = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.Country, countryName, expectedResult);


                Assert.IsTrue(Name && Street && City && Postalcode && Country, expectedResult);
            }
            catch (Exception e)
            {
                Exception = e;
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
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Sales", "Cases");
                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "New Case");

                var newCasePage = new CaseEntity(XrmApp, XrmBrowser);

                var name = Logger.LogSet(() =>
                    newCasePage.Name, "OneTimeInstitution");
                XrmApp.ThinkTime(5000);

                var Country = Logger.LogGetExpectedResultCheck(() =>
                    newCasePage.ExistingInstitution, name, expectedResult);

                Assert.IsTrue(Country);



            }
            catch (Exception e)
            {
                Exception = e;
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
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Sales", "Cases");

                Logger.Log<Action<string, bool>>(
                    XrmApp.Grid.Search, caseData.Title);

                Logger.Log<Action<int>>(
                    XrmApp.Grid.HighLightRecord, 0);

                Logger.Log<Action<int>>(
                    XrmApp.Grid.OpenRecord, 0);

                Logger.Log<Action<string,string,bool>>(
                    XrmApp.CommandBar.ClickCommand,"Reactivate Case");


                var confirmDialog = Logger.LogExpectedResultCheck<Func<bool,bool>,bool>(
                    XrmApp.Dialogs.ConfirmationDialog, true, expectedResult, true);

                XrmApp.ThinkTime(2000);

                var status = Logger.LogExpectedResultCheck<Func<string>, string>(
                    XrmApp.Entity.GetFooterStatusValue, "Active", expectedResult);

               
                Assert.IsTrue(confirmDialog && status, expectedResult);

            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }

        }

    }
}
