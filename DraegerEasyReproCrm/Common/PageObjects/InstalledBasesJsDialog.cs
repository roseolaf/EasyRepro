using System;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class InstalledBasesJsDialog : AlertJsDialog
    {
        public InstalledBasesJsDialog(XrmApp xrm, WebClient client) : base(xrm, client)
        {
        }

        public string Title => GetValue(InstalledBasesLocators.AlertJsDialog.Title);

        public string Message => GetValue(InstalledBasesLocators.AlertJsDialog.Message);

        public void Ok()
        {

            browser.Driver.WaitForElement(InstalledBasesLocators.AlertJsDialog.Ok).Click();
        }

     
        public bool DialogIsOpen(bool value) => AlertJsDialogVisible(InstalledBasesLocators.AlertJsDialog.Root, value);

       
    }

}