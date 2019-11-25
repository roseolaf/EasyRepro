using System;
using Draeger.Dynamics365.Testautomation.Attributes;
using Draeger.Dynamics365.Testautomation.Common;
using Draeger.Dynamics365.Testautomation.Common.EntityManager;
using Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator;
using Infoman.Xrm.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Draeger.Dynamics365.Testautomation.Common.Enums;
using Draeger.Dynamics365.Testautomation.Common.Helper;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Draeger.Dynamics365.Testautomation.Common.PageObjects;
using Draeger.Dynamics365.Testautomation.ExtensionMethods;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Attributes;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using System.Linq;
using Microsoft.TeamFoundation.Framework.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using StringExtensions = Draeger.Dynamics365.Testautomation.ExtensionMethods.StringExtensions;
using Entity = Microsoft.Xrm.Sdk.Entity;
using Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements;
using static Draeger.Dynamics365.Testautomation.Common.Enums.Global;

namespace Draeger.Dynamics365.Testautomation.Testautomation.iService
{
    [TestClass]
    public class Unknown : TestBase
    {
        protected static Dictionary<string, Entity> testData = new Dictionary<string, Entity>();

        private const string UserAlias = "UserAlias";

        [TestInitialize]
        public void TestClassSetup()
        {


            var TestCaseId = TestContext.Properties["TestCaseId"];

            switch (TestCaseId)
            {
                case "64837":
                    testData.Add("64837Serviceorder", 
                        new SpecificAttributeDecorator("dw_serviceordernumber", "5XXXXXTC64837", ReplaceType.Number, 'X',
                            new BaseComponent(logger)).CreateEntityRecord(this, 
                                new BaseComponent(logger).CloneEntityRecord(Serviceorder.EntityLogicalName,  new KeyValuePair<string, object>("dw_serviceordernumber", "118854979"))));
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

        [TestProperty("TestCaseId", "64837")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void CreateDispatchFromASingleNonDispatchedServiceOrder()
        {
            try
            {
                var serviceOrder = testData["64837Serviceorder"] as Serviceorder;

                Login(xrmApp, UserAlias);
                logger.Log<Action<string>>(
                    xrmApp.Navigation.OpenApp, "Dräger Sales App");
                logger.Log<Action<string,string>>(
                    xrmApp.Navigation.OpenSubArea, "Service", "Service Orders");
                logger.NextStep();
                logger.Log<Action<string>>(
                    xrmApp.Grid.SwitchView,"Non-dispatched Service Orders");

                xrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(xrmApp,XrmBrowser);

                var expectedResultStep2 = "List of 'Non-dispatched Service Orders' appears";
                var check = logger.LogExpectedResultCheck<Func<string>,string>(
                    grid.GridLabel, "Non-dispatched Service Orders", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);

                var gridItem = logger.Log<Func<Guid, GridItemInfo>,GridItemInfo>(
                     grid.SelectGridItem, serviceOrder.Id);

                logger.Log<Action<string, string,bool>>(
                    xrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");



            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
        }



    }
}
