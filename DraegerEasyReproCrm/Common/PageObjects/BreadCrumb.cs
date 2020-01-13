using Draeger.Dynamics365.Testautomation.Common.Locators;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class BreadCrumb
    {
        private XrmApp xrmApp;
        private WebClient browser;
        public BreadCrumb(XrmApp xrm, WebClient b = null)
        {
            browser = b;
            xrmApp = xrm;
        }

        public string GetEntityBreadCrumbText
        {
            get => GetBreadCrumbText(BreadCrumbLocators.entitybreadCrumbText);
        }
        public string GetAreaBreadCrumbText
        {
            get => GetBreadCrumbText(BreadCrumbLocators.areaBreadCrumbText);
        }
        public string GetAppBreadCrumbText
        {
            get => GetBreadCrumbText(BreadCrumbLocators.appBreadCrumbText);
        }
        public string GetRecordCrumbText
        {
            get => GetBreadCrumbText(BreadCrumbLocators.recordBreadCrumbText);
        }

        public void ClickEntityBreadCrumb()
        {
            ClickBreadCrumb(BreadCrumbLocators.entitybreadCrumb);
        }
        public void ClickAreaBreadCrumb()
        {
            ClickBreadCrumb(BreadCrumbLocators.areaBreadCrumb);
        }
        public void ClickAppBreadCrumbText()
        {
            ClickBreadCrumb(BreadCrumbLocators.appBreadCrumb);
        }

        private string GetBreadCrumbText(By locator)
        {
            return browser.Browser.Driver.FindElement(locator).Text;
        }
        private void ClickBreadCrumb(By locator)
        {
            browser.Browser.Driver.FindElement(locator).ClickWait();
        }

    }
}
