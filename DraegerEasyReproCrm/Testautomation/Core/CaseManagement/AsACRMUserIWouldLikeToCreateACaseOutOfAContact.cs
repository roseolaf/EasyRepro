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
using Draeger.Testautomation.CredentialsManagerCore.Attributes;
using OpenQA.Selenium;
using Microsoft.Crm.Sdk.Messages;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using Entity = Microsoft.Xrm.Sdk.Entity;

namespace Draeger.Dynamics365.Testautomation.Testautomation.Core.CaseManagement
{
    [TestClass]
    public class AsACRMUserIWouldLikeToCreateACaseOutOfAContact : TestBase
    {
        protected static Dictionary<string, Entity> testData = new Dictionary<string, Entity>();

        private const string UserAlias = "UserAlias";

        [TestInitialize]
        public void TestClassSetup()
        {


            var TestCaseId = TestContext.Properties["TestCaseId"];

            switch (TestCaseId)
            {
                case "30033":
                    testData.Add("30033Institution", new TestcaseNameDecorator(
                                                        new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                        new BaseComponent(logger))).CreateEntityRecord(this, new Account()));

                    testData.Add("30033Contact", new TestcaseNameDecorator(
                                                        new ReferenceToDecorator("parentcustomerid", testData["30033Institution"],
                                                        new OwnerDecorator(users[UserAlias].Username.ToUnsecureString(),
                                                        new BaseComponent(logger))))
                                                    .CreateEntityRecord(this, new Contact()));


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


        [TestProperty("TestCaseId", "30033")]
        [TestCategory("Core")]
        [TestCategory("Case Management")]
        [CrmUser(UserAlias, SecurityRole.DwBasicCrmAccess, SecurityRole.DwCaseManagement, UserGroup = UserGroup.Sales)]
        [Priority(2)]
        [TestMethod]
        public void CaseCreateAndResolve()
        {
            var institution = testData["30033Institution"] as Account;
            var contact = testData["30033Contact"] as Contact;
            string expectedResult = "A new tab opens an empty case form with prefilled values from contacts details and institution details. Make sure that the JavaScript is triggered even when this is invoked by opening it from contact form. The attachment shows the areas which must be prefilled (use a contact and institution which has at least these fields prefilled). It is accepted that Last Name is set to not business required during this process.";

            try
            {
                

                Login(xrmApp, UserAlias);
                logger.Log<Action<string, Guid>>(xrmApp.Entity.OpenEntity, contact.LogicalName, contact.Id);
                var newCase = new CaseEntity(xrmApp, XrmBrowser);
                logger.Log<Action<string, string, bool>>(xrmApp.CommandBar.ClickCommand, "Create New Case");
                xrmApp.ThinkTime(2000);
                var existingInstitution = logger.LogGetExpectedResultCheck(() => newCase.ExistingInstitution, institution.Name, expectedResult);
                var existingContact = logger.LogGetExpectedResultCheck(() => newCase.ExistingContact, $"{contact.LastName}, {contact.FirstName}", expectedResult);

                var firstName = logger.LogGetExpectedResultCheck(() => newCase.FirstName, contact.FirstName, expectedResult);
                var lastName = logger.LogGetExpectedResultCheck(() => newCase.LastName, contact.LastName, expectedResult);
                var businessPhone = logger.LogGetExpectedResultCheck(() => newCase.BusinessPhone, contact.Telephone1, expectedResult);
                var mobilePhone = logger.LogGetExpectedResultCheck(() => newCase.MobilePhone, contact.MobilePhone, expectedResult);
                var emailAddress = logger.LogGetExpectedResultCheck(() => newCase.EmailAddress, contact.EMailAddress1, expectedResult);

                var name = logger.LogGetExpectedResultCheck(() => newCase.Name, institution.Name, expectedResult);
                var city = logger.LogGetExpectedResultCheck(() => newCase.City, institution.Address1_City, expectedResult);
                var country = logger.LogGetExpectedResultCheck(() => newCase.Country, institution.Countryid.Name, expectedResult);

                Assert.IsTrue(existingInstitution && existingContact && firstName && lastName && businessPhone && mobilePhone && emailAddress && name && city && country, expectedResult);

            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
        }

    }
}
