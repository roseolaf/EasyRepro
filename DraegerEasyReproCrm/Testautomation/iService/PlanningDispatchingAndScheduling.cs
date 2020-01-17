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
using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Framework.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using StringExtensions = Draeger.Dynamics365.Testautomation.ExtensionMethods.StringExtensions;
using Entity = Microsoft.Xrm.Sdk.Entity;
using Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements;
using TaADOLog.Logger;
using static Draeger.Dynamics365.Testautomation.Common.Enums.Global;

namespace Draeger.Dynamics365.Testautomation.Testautomation.iService
{
    [TestClass]
    public class PlanningDispatchingAndScheduling : TestBase
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

                    var institutionEntity = new BaseComponent(Logger).GetEntityRecord(this, "account",
                        Guid.Parse("b149c976-141b-e911-a961-000d3a29f712"));
                    var installedBaseEntity = new BaseComponent(Logger).GetEntityRecord(this, "speed1_installedbase",
                        Guid.Parse("618a220f-901d-e911-a965-000d3a29fac0"));

                    testData.Add("64837Serviceorder",
                        new TestcaseNameDecorator(
                            new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                            new SpecificAttributeDecorator("dw_serviceordernumber", "5XXXXXTC64837", ReplaceType.Number,
                                'X',
                                new ReferenceToDecorator("dw_soldtopartyid", institutionEntity,
                                    new ReferenceToDecorator("dw_accountid", institutionEntity,
                                        new ReferenceToDecorator("dw_installedbaseid", installedBaseEntity,
                                            new BaseComponent(Logger))))))).CreateEntityRecord(this, new Serviceorder()));
                    // new BaseComponent(Logger).CloneEntityRecord(Serviceorder.EntityLogicalName,  new KeyValuePair<string, object>("dw_serviceordernumber", "118854979"))));
                    break;

                case "66789":

                    var institution66879 = new BaseComponent(Logger).GetEntityRecord(this, "account",
                        Guid.Parse("b149c976-141b-e911-a961-000d3a29f712"));
                    var equipment1 = new BaseComponent(Logger).GetEntityRecord(this, "speed1_installedbase",
                        Guid.Parse("618a220f-901d-e911-a965-000d3a29fac0"));
                    var equipment2 = new BaseComponent(Logger).GetEntityRecord(this, "speed1_installedbase",
                        Guid.Parse("54087a3f-901d-e911-a965-000d3a29fac0"));
                    var equipment3 = new BaseComponent(Logger).GetEntityRecord(this, "speed1_installedbase",
                        Guid.Parse("cf81220f-901d-e911-a965-000d3a29fac0"));

                    var rnd = new Random().Next(10000,99999);
                    testData.Add("66789Serviceorder1",
                        new TestcaseNameDecorator(
                            new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                new SpecificAttributeDecorator("dw_serviceordernumber", $"5{rnd}XXXTC66789", ReplaceType.Number,
                                    'X',
                                    new ReferenceToDecorator("dw_soldtopartyid", institution66879,
                                        new ReferenceToDecorator("dw_accountid", institution66879,
                                            new ReferenceToDecorator("dw_installedbaseid", equipment1,
                                                new BaseComponent(Logger))))))).CreateEntityRecord(this, new Serviceorder()));
                    testData.Add("66789Serviceorder2",
                        new TestcaseNameDecorator(
                            new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                new SpecificAttributeDecorator("dw_serviceordernumber", $"5{rnd}XXXTC66789", ReplaceType.Number,
                                    'X',
                                    new ReferenceToDecorator("dw_soldtopartyid", institution66879,
                                        new ReferenceToDecorator("dw_accountid", institution66879,
                                            new ReferenceToDecorator("dw_installedbaseid", equipment2,
                                                new BaseComponent(Logger))))))).CreateEntityRecord(this, new Serviceorder()));
                    testData.Add("66789Serviceorder3",
                        new TestcaseNameDecorator(
                            new OwnerDecorator(Users[UserAlias].Username.ToUnsecureString(),
                                new SpecificAttributeDecorator("dw_serviceordernumber", $"5{rnd}XXXTC66789", ReplaceType.Number,
                                    'X',
                                    new ReferenceToDecorator("dw_soldtopartyid", institution66879,
                                        new ReferenceToDecorator("dw_accountid", institution66879,
                                            new ReferenceToDecorator("dw_installedbaseid", equipment3,
                                                new BaseComponent(Logger))))))).CreateEntityRecord(this, new Serviceorder()));


                    // new BaseComponent(Logger).CloneEntityRecord(Serviceorder.EntityLogicalName,  new KeyValuePair<string, object>("dw_serviceordernumber", "118854979"))));
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


        [TestProperty("TestCaseId", "67450")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void DispatchLifeCycleCreateProcessAndCloseDispatch()
        {
            try
            {
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.NextStep();
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Installed Bases");
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Active IB - Testautomation");

                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser, "");

                var expectedResultStep3 = "List of usable Installed Bases is displayed";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Active IB - Testautomation", expectedResultStep3);
                Assert.IsTrue(check, expectedResultStep3);

                Logger.NextStep();
                Logger.Log<Action<int>>(XrmApp.Grid.HighLightRecord, 0);


                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");
                var installedBaseDialog = new InstalledBasesJsDialog(XrmApp, XrmBrowser);

                var resultStep3a = Logger.LogGetExpectedResultCheck(() => installedBaseDialog.Title, InstalledBasesConstants.AlertJsTitle.Finished, "A popup appears with the success message.");
                var resultStep3b = Logger.LogGetExpectedResultCheck(() => installedBaseDialog.Message, "", "no additional error message");
                Assert.IsTrue(resultStep3a && resultStep3b, "A popup appears with the success message: \"Installedbases processing finished!\" and no additional error message");
                Logger.NextStep();

                Logger.Log<Action>(installedBaseDialog.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(installedBaseDialog.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                XrmApp.ThinkTime(2000);
                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);

                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);
                Logger.NextStep();
                var getRecordCrumbText = Logger.LogGet(() => breadCrumb.GetRecordCrumbText);
                var expectedResultStep5 = Logger.LogExpectedResultCheck<Func<string, string, bool>, bool>(Regex.IsMatch, true,
                    "There´s still a Dispatch Number on the tab which corresponds exactly to the specifications from \"Dispatch number range\" in the settings ==>  A[0-9]{8} ", getRecordCrumbText, "A[0-9]{8}");

                Assert.IsTrue(expectedResultStep5);
                Logger.NextStep();
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Service Orders");

                GridElement soGrid = new GridElement(XrmApp, XrmBrowser, "");
                var resultStep6 = Logger.LogExpectedResultCheck<Func<int>, int>(soGrid.ItemsCount, 1, "Service Order is created");
                Assert.IsTrue(resultStep6, "Service Order is created");
                Logger.NextStep();

                Logger.Log<Action<int>>(XrmApp.Grid.HighLightRecord, 0);
                Logger.Log<Action<string, string>>(soGrid.Button, "Edit");
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Time & Material");

                GridElement timeGrid = new GridElement(XrmApp, XrmBrowser, "Time");
                Logger.Log<Action<string, string>>(timeGrid.Button, "New Service Order Item");

                ServiceOrderEntity serviceOrder = new ServiceOrderEntity(XrmApp, XrmBrowser);
                Logger.LogSetExpectedResult(() => serviceOrder.QuickCreate_TimeAndMaterial, "R001", "Activity (R001) should be found");
                XrmApp.ThinkTime(2000);
                var priceValue = Logger.LogGet(() => serviceOrder.QuickCreate_Price);
                var expectedResultStep7 = Logger.LogExpectedResultCheck<Func<string, string, bool>, bool>(Regex.IsMatch, true,
                    "A price is automatically found and placed into the field \"Price\"", priceValue, @"\d+");
                Assert.IsTrue(expectedResultStep7, "price is automatically found and placed into the field \"Price\"");

                // TODO -> Blocked
            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "66774")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void CreateNewDispatchForMultipleServiceOrderFromSOViewDifferingInShipToParty()
        {
            try
            {
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.NextStep();

                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Service Orders");
                Logger.NextStep();
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Non-dispatched Service Orders - Testautomation");
                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser);
                var expectedResultStep2 = "List of usable non-dispatched Service Orders appears";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Non-dispatched Service Orders - Testautomation", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);
                Logger.NextStep();

                var gridItemsToSelect = new List<GridItemInfo>();
                bool found = false;
                while (!found)
                {
                    var gridItems = grid.GetGridItems();
                    var groupedGridItems = gridItems.GroupBy(gI =>
                            gI.Attribute["Sold-To-Party"]).Select(grp =>
                                grp.GroupBy(con =>
                                    con.Attribute["Contract No."]).Select(grp2 =>
                                    grp2.GroupBy(ship =>
                                        ship.Attribute["Ship-To-Party"]).Select(grp3 => grp3.ToList())));

                    var contractEnumerable = groupedGridItems.FirstOrDefault(shipTo => shipTo.FirstOrDefault(e => e.Count() > 1) != null);
                    if (contractEnumerable == null)
                    {
                        grid.NextPage();
                        continue;
                    }
                    var shipToEnumerable = contractEnumerable.FirstOrDefault(e => e.Count() > 1);
                    if (shipToEnumerable == null)
                    {
                        grid.NextPage();
                        continue;
                    }
                    found = true;

                    gridItemsToSelect.AddRange(shipToEnumerable.Select(sTE => sTE[0]));
                }


                Logger.Log<Func<List<GridItemInfo>, List<GridItemInfo>>, List<GridItemInfo>>(
                    grid.SelectGridItems, gridItemsToSelect);


                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");
                var serviceOrderJsAlert = new ServiceordersJsDialog(XrmApp, XrmBrowser);

                var resultStep3a = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Title, ServiceordersConstants.AlertJsTitle.Finished, "A pop-up appears, stating \"Service Orders processing finished!\"");
                Assert.IsTrue(resultStep3a);
                var resultStep3b = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Content, ServiceordersConstants.AlertJsContent.NotUnderSameCustomerOrContractError, "Service Orders are not under same customer or contract");
                //Assert.IsTrue(resultStep3b); TODO

                Logger.NextStep();

                Logger.Log<Action>(serviceOrderJsAlert.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(serviceOrderJsAlert.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                XrmApp.ThinkTime(2000);

                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);

                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);
                Logger.NextStep();
                var getRecordCrumbText = Logger.LogGet(() => breadCrumb.GetRecordCrumbText);

                var expectedResultStep5 = Logger.LogExpectedResultCheck<Func<string, string, bool>, bool>(Regex.IsMatch, true,
                    "There´s still a Dispatch Number on the tab which corresponds exactly to the specifications from \"Dispatch number range\" in the settings ==>  A[0-9]{8} ", getRecordCrumbText, "A[0-9]{8}");
                Logger.NextStep();
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Service Orders");
                var serviceOrdersGridItemInfos = grid.GetGridItems();
                var serviceOrderFromGrid = gridItemsToSelect[0];
                Logger.LogGetExpectedResultCheck(() => serviceOrdersGridItemInfos.Count, 1, "All expected Service Orders are dispatched.");
                Logger.LogGetExpectedResultCheck(() => serviceOrdersGridItemInfos[0].Attribute["Service Order No."], serviceOrderFromGrid.Attribute["Service Order No."], "All expected Service Orders are dispatched.");


            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }


        [TestProperty("TestCaseId", "65002")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void CreateNewDispatchForMultipleServiceOrderFromSOViewDifferingInContractNo()
        {
            try
            {
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.NextStep();

                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Service Orders");
                Logger.NextStep();
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Non-dispatched Service Orders - Testautomation");
                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser);
                var expectedResultStep2 = "List of usable non-dispatched Service Orders appears";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Non-dispatched Service Orders - Testautomation", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);
                Logger.NextStep();

                var gridItemsToSelect = new List<GridItemInfo>();
                bool found = false;
                while (!found)
                {
                    var gridItems = grid.GetGridItems();
                    var groupedGridItems = gridItems.GroupBy(gI =>
                            gI.Attribute["Sold-To-Party"]).Select(grp =>
                                grp.GroupBy(con =>
                                    con.Attribute["Ship-To-Party"]).Select(grp2 =>
                                    grp2.GroupBy(ship =>
                                        ship.Attribute["Contract No."]).Select(grp3 => grp3.ToList())));

                    var contractEnumerable = groupedGridItems.FirstOrDefault(shipTo => shipTo.FirstOrDefault(e => e.Count() > 1) != null);
                    if (contractEnumerable == null)
                    {
                        grid.NextPage();
                        continue;
                    }
                    var shipToEnumerable = contractEnumerable.FirstOrDefault(e => e.Count() > 1);
                    if (shipToEnumerable == null)
                    {
                        grid.NextPage();
                        continue;
                    }
                    found = true;

                    gridItemsToSelect.AddRange(shipToEnumerable.Select(sTE => sTE[0]));
                }

                Logger.Log<Func<List<GridItemInfo>, List<GridItemInfo>>, List<GridItemInfo>>(
                    grid.SelectGridItems, gridItemsToSelect);


                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");
                var serviceOrderJsAlert = new ServiceordersJsDialog(XrmApp, XrmBrowser);

                var resultStep3a = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Title, ServiceordersConstants.AlertJsTitle.Finished, "A pop-up appears, stating \"Service Orders processing finished!\"");
                Assert.IsTrue(resultStep3a);
                var resultStep3b = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Content, ServiceordersConstants.AlertJsContent.NotUnderSameCustomerOrContractError, "Service Orders are not under same customer or contract");
                //Assert.IsTrue(resultStep3b); TODO

                Logger.NextStep();

                Logger.Log<Action>(serviceOrderJsAlert.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(serviceOrderJsAlert.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                XrmApp.ThinkTime(2000);

                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);

                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);
                Logger.NextStep();
                var getRecordCrumbText = Logger.LogGet(() => breadCrumb.GetRecordCrumbText);

                var expectedResultStep5 = Logger.LogExpectedResultCheck<Func<string, string, bool>, bool>(Regex.IsMatch, true,
                    "There´s still a Dispatch Number on the tab which corresponds exactly to the specifications from \"Dispatch number range\" in the settings ==>  A[0-9]{8} ", getRecordCrumbText, "A[0-9]{8}");
                Logger.NextStep();
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Service Orders");
                var serviceOrdersGridItemInfos = grid.GetGridItems();
                var serviceOrderFromGrid = gridItemsToSelect[0];
                Logger.LogGetExpectedResultCheck(() => serviceOrdersGridItemInfos.Count, 1, "All expected Service Orders are dispatched.");
                Logger.LogGetExpectedResultCheck(() => serviceOrdersGridItemInfos[0].Attribute["Service Order No."], serviceOrderFromGrid.Attribute["Service Order No."], "All expected Service Orders are dispatched.");


            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "66776")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void CreateNewDispatchForMultipleServiceOrderFromSoViewDifferingInSoldToParty()
        {
            try
            {
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.NextStep();

                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Service Orders");
                Logger.NextStep();
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Non-dispatched Service Orders - Testautomation");
                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser);
                var expectedResultStep2 = "List of usable non-dispatched Service Orders appears";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Non-dispatched Service Orders - Testautomation", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);
                Logger.NextStep();

                var gridItemsToSelect = new List<GridItemInfo>();
                bool found = false;
                while (!found)
                {
                    var gridItems = grid.GetGridItems();
                    var groupedGridItems = gridItems.GroupBy(gI =>
                            gI.Attribute["Ship-To-Party"]).Select(grp =>
                                grp.GroupBy(con =>
                                    con.Attribute["Contract No."]).Select(grp2 =>
                                    grp2.GroupBy(ship =>
                                        ship.Attribute["Sold-To-Party"]).Select(grp3 => grp3.ToList())));

                    var contractEnumerable = groupedGridItems.FirstOrDefault(shipTo => shipTo.FirstOrDefault(e => e.Count() > 1) != null);
                    if (contractEnumerable == null)
                    {
                        grid.NextPage();
                        continue;
                    }
                    var shipToEnumerable = contractEnumerable.FirstOrDefault(e => e.Count() > 1);
                    if (shipToEnumerable == null)
                    {
                        grid.NextPage();
                        continue;
                    }
                    found = true;

                    gridItemsToSelect.AddRange(shipToEnumerable.Select(sTE => sTE[0]));
                }



                Logger.Log<Func<List<GridItemInfo>, List<GridItemInfo>>, List<GridItemInfo>>(
                    grid.SelectGridItems, gridItemsToSelect);


                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");
                var serviceOrderJsAlert = new ServiceordersJsDialog(XrmApp, XrmBrowser);

                var resultStep3a = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Title, ServiceordersConstants.AlertJsTitle.Finished, "A pop-up appears, stating \"Service Orders processing finished!\"");
                Assert.IsTrue(resultStep3a);
                var resultStep3b = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Content, ServiceordersConstants.AlertJsContent.NotUnderSameCustomerOrContractError, "Service Orders are not under same customer or contract");
                //Assert.IsTrue(resultStep3b); TODO

                Logger.NextStep();

                Logger.Log<Action>(serviceOrderJsAlert.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(serviceOrderJsAlert.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                XrmApp.ThinkTime(2000);

                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);

                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);
                Logger.NextStep();
                var getRecordCrumbText = Logger.LogGet(() => breadCrumb.GetRecordCrumbText);

                var expectedResultStep5 = Logger.LogExpectedResultCheck<Func<string, string, bool>, bool>(Regex.IsMatch, true,
                    "There´s still a Dispatch Number on the tab which corresponds exactly to the specifications from \"Dispatch number range\" in the settings ==>  A[0-9]{8} ", getRecordCrumbText, "A[0-9]{8}");
                Logger.NextStep();
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Service Orders");
                var serviceOrdersGridItemInfos = grid.GetGridItems();
                var serviceOrderFromGrid = gridItemsToSelect[0];
                Logger.LogGetExpectedResultCheck(() => serviceOrdersGridItemInfos.Count, 1, "All expected Service Orders are dispatched.");
                Logger.LogGetExpectedResultCheck(() => serviceOrdersGridItemInfos[0].Attribute["Service Order No."], serviceOrderFromGrid.Attribute["Service Order No."], "All expected Service Orders are dispatched.");


            }
            catch (Exception e)
            {
                Exception = e;
                throw;
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


                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Service Orders");
                Logger.NextStep();
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Non-dispatched Service Orders");

                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser);

                var expectedResultStep2 = "List of 'Non-dispatched Service Orders' appears";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Non-dispatched Service Orders", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);

                Logger.Log<Action<string, bool>>(XrmApp.Grid.Search, serviceOrder.Serviceordernumber);

                var gridItem = Logger.Log<Func<Guid, GridItemInfo>, GridItemInfo>(
                     grid.SelectGridItem, serviceOrder.Id);

                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");

                var serviceOrderJsAlert = new ServiceordersJsDialog(XrmApp, XrmBrowser);

                var resultStep3 = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Title, ServiceordersConstants.AlertJsTitle.Finished, "A popup appears with the success message.");
                Assert.IsTrue(resultStep3);

                Logger.NextStep();

                Logger.Log<Action>(serviceOrderJsAlert.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(serviceOrderJsAlert.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                XrmApp.ThinkTime(2000);

                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);

                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);
                Logger.NextStep();
                var getRecordCrumbText = Logger.LogGet(() => breadCrumb.GetRecordCrumbText);

                var expectedResultStep5 = Logger.LogExpectedResultCheck<Func<string, string, bool>, bool>(Regex.IsMatch, true,
                    "There´s still a Dispatch Number on the tab which corresponds exactly to the specifications from \"Dispatch number range\" in the settings ==>  A[0-9]{8} ", getRecordCrumbText, "A[0-9]{8}");

                Assert.IsTrue(expectedResultStep5);

            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "64978")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void CreateDispatchFromASingleInstalledBase()
        {
            try
            {

                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Installed Bases");
                Logger.NextStep();
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Active IB - Testautomation");

                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser, "");

                var expectedResultStep2 = "List of usable Installed Bases is displayed";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Active IB - Testautomation", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);

                Logger.NextStep();
                var gridItems = grid.GetGridItems();

                Logger.Log<Func<GridItemInfo, GridItemInfo>, GridItemInfo>(grid.SelectGridItem, gridItems[0]);

                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");
                var installedBaseDialog = new InstalledBasesJsDialog(XrmApp, XrmBrowser);
                var resultStep3a = Logger.LogGetExpectedResultCheck(() => installedBaseDialog.Title, InstalledBasesConstants.AlertJsTitle.Finished, "A popup appears with the success message.");
                var resultStep3b = Logger.LogGetExpectedResultCheck(() => installedBaseDialog.Message, "", "no additional error message");
                Assert.IsTrue(resultStep3a && resultStep3b, "A popup appears with the success message: \"Installedbases processing finished!\" and no additional error message");
                Logger.NextStep();

                Logger.Log<Action>(installedBaseDialog.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(installedBaseDialog.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);
                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);

                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Service Orders");
                var serviceOrderGrid = new GridElement(XrmApp, XrmBrowser);
                var sOGridItemsList = Logger.Log<Func<List<GridItemInfo>>, List<GridItemInfo>>(serviceOrderGrid.GetGridItems);

                var expectedResultStep5 = sOGridItemsList.Select(sOG => sOG.Attribute["Equipment No. (Equipment)"])
                                              .Intersect(gridItems.Select(gI => gI.Attribute["Equipment No."])).Count() ==
                                          sOGridItemsList.Count();


                Assert.IsTrue(expectedResultStep5, "Service Order is created");
            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "65000")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void CreateDispatchForMultipleInstalledBasesFromIBView()
        {
            try
            {

                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Installed Bases");
                Logger.NextStep();
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Active IB - Testautomation");

                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser, "");

                var expectedResultStep2 = "List of usable Installed Bases is displayed";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Active IB - Testautomation", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);

                Logger.NextStep();
                var gridItems = grid.GetGridItems();
                int gIToSelect = 4;
                var gridItemsToSelect = gridItems.Take(gIToSelect).ToList();
                
                Logger.Log<Func<List<GridItemInfo>, List<GridItemInfo>>, List<GridItemInfo>>(grid.SelectGridItems, gridItemsToSelect);
                

                Logger.Log<Action<string, string, bool>>(XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");
                var installedBaseDialog = new InstalledBasesJsDialog(XrmApp, XrmBrowser);
                var resultStep3a = Logger.LogGetExpectedResultCheck(() => installedBaseDialog.Title, InstalledBasesConstants.AlertJsTitle.Finished, "A popup appears with the success message.");
                var resultStep3b = Logger.LogGetExpectedResultCheck(() => installedBaseDialog.Message, "", "no additional error message");
                Assert.IsTrue(resultStep3a && resultStep3b, "A popup appears with the success message: \"Installedbases processing finished!\" and no additional error message");
                Logger.NextStep();

                Logger.Log<Action>(installedBaseDialog.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(installedBaseDialog.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);
                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);
                Logger.NextStep();

                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Service Orders");
                var serviceOrderGrid = new GridElement(XrmApp, XrmBrowser);
                var sOGridItemsList = Logger.Log<Func<List<GridItemInfo>>, List<GridItemInfo>>(serviceOrderGrid.GetGridItems);

                var expectedResultStep5 = sOGridItemsList.Select(sOG => sOG.Attribute["Equipment No. (Equipment)"])
                        .Intersect(gridItemsToSelect.Select(gI => gI.Attribute["Equipment No."])).Count() ==
                    sOGridItemsList.Count();


                Assert.IsTrue(expectedResultStep5, "All required entries found.");
            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }

        [TestProperty("TestCaseId", "64999")]
        [TestCategory("iService")]
        [CrmUser(UserAlias, UserGroup = UserGroup.Service)]
        [Priority(2)]
        [TestMethod]
        public void CreateDispatchForMultipleUniformServiceOrdersFromSOView()
        {
            try
            {
                Login(XrmApp, UserAlias);
                Logger.Log<Action<string>>(
                    XrmApp.Navigation.OpenApp, "Dräger Sales App");
                Logger.NextStep();

                Logger.Log<Action<string, string>>(
                    XrmApp.Navigation.OpenSubArea, "Service", "Service Orders");
                Logger.NextStep();
                Logger.Log<Action<string>>(
                    XrmApp.Grid.SwitchView, "Non-dispatched Service Orders - Testautomation");
                XrmApp.ThinkTime(2000);
                GridElement grid = new GridElement(XrmApp, XrmBrowser);
                var expectedResultStep2 = "List of Service Orders visible";
                var check = Logger.LogExpectedResultCheck<Func<string>, string>(
                    grid.GridLabel, "Non-dispatched Service Orders - Testautomation", expectedResultStep2);
                Assert.IsTrue(check, expectedResultStep2);
                Logger.NextStep();

                var gridItemsToSelect = new List<GridItemInfo>();
                bool found = false;
                while (!found)
                {
                    var gridItems = grid.GetGridItems();
                    var groupedGridItems = gridItems.GroupBy(gI =>
                            gI.Attribute["Sold-To-Party"]).Select(grp =>
                                grp.GroupBy(con =>
                                    con.Attribute["Ship-To-Party"]).Select(grp2 =>
                                    grp2.GroupBy(ship =>
                                        ship.Attribute["Contract No."]).Select(grp3 => grp3.ToList())));

                    var contractEnumerable = groupedGridItems.FirstOrDefault(shipTo => shipTo.FirstOrDefault(contract => contract.FirstOrDefault(element =>element.Count > 1) != null) != null);
                    if (contractEnumerable == null)
                    {
                        grid.NextPage();
                        continue;
                    }
                    found = true;
                    
                    gridItemsToSelect = contractEnumerable.First().First();
                }

                Logger.Log<Func<List<GridItemInfo>, List<GridItemInfo>>, List<GridItemInfo>>(
                    grid.SelectGridItems, gridItemsToSelect);


                Logger.Log<Action<string, string, bool>>(
                    XrmApp.CommandBar.ClickCommand, "Dispatch", "Create Dispatch");
                var serviceOrderJsAlert = new ServiceordersJsDialog(XrmApp, XrmBrowser);

                var resultStep3a = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Title, ServiceordersConstants.AlertJsTitle.Finished, "A pop-up appears, stating \"Service Orders processing finished!\"");
                Assert.IsTrue(resultStep3a);
                var resultStep3b = Logger.LogGetExpectedResultCheck(() => serviceOrderJsAlert.Content, "", "No additional error message.");
                Assert.IsTrue(resultStep3b); 

                Logger.NextStep();

                Logger.Log<Action>(serviceOrderJsAlert.Ok);

                var expectedResultStep4a = Logger.LogExpectedResultCheck<Func<bool, bool>, bool>(serviceOrderJsAlert.DialogIsOpen, false, "The popup disappears", false);
                Assert.IsTrue(expectedResultStep4a);
                XrmApp.ThinkTime(2000);

                var breadCrumb = new BreadCrumb(XrmApp, XrmBrowser);

                var expectedResultStep4b = Logger.LogGetExpectedResultCheck(() => breadCrumb.GetEntityBreadCrumbText, "Dispatches",
                    "The window refreshes with the display of the newly created Dispatch in a dispatch form.");
                Assert.IsTrue(expectedResultStep4b);
                Logger.NextStep();
                var getRecordCrumbText = Logger.LogGet(() => breadCrumb.GetRecordCrumbText);

                var expectedResultStep5 = Logger.LogExpectedResultCheck<Func<string, string, bool>, bool>(Regex.IsMatch, true,
                    "There´s still a Dispatch Number on the tab which corresponds exactly to the specifications from \"Dispatch number range\" in the settings ==>  A[0-9]{8} ", getRecordCrumbText, "A[0-9]{8}");
                Logger.NextStep();
                Logger.Log<Action<string, string>>(XrmApp.Entity.SelectTab, "Service Orders");
                var serviceOrdersGridItemInfos = grid.GetGridItems();
               
                Logger.LogGetExpectedResultCheck(() => serviceOrdersGridItemInfos.Count, gridItemsToSelect.Count, "All expected Service Orders are dispatched.");
                var expectedResultStep6 = serviceOrdersGridItemInfos.All(sOG =>
                    gridItemsToSelect.Any(gits =>
                        gits.Attribute["Service Order No."].Equals(sOG.Attribute["Service Order No."])));
                Assert.IsTrue(expectedResultStep6);


            }
            catch (Exception e)
            {
                Exception = e;
                throw;
            }
        }

    }
}
