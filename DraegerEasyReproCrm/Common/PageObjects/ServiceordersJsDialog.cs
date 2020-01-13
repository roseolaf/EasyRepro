using System;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class ServiceordersJsDialog : AlertJsDialog
    {
        public ServiceordersJsDialog(XrmApp xrm, WebClient client) : base(xrm, client)
        {
        }

        public string Title => GetValue(ServiceordersLocators.AlertJsDialog.Title);


        public void Ok()
        {

            browser.Driver.WaitForElement(ServiceordersLocators.AlertJsDialog.Ok).Click();
        }

     
        public bool DialogIsOpen(bool value) => AlertJsDialogVisible(ServiceordersLocators.AlertJsDialog.Root, value);

       
    }

}