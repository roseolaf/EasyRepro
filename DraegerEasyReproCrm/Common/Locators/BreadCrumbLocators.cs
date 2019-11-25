using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Dynamics365.Testautomation.Common.Locators
{
    public static class BreadCrumbLocators
    {
        public static readonly By entitybreadCrumbText = By.Id("entityBreadCrumbText");
        public static readonly By entitybreadCrumb = By.Id("entityBreadCrumb");
        public static readonly By areaBreadCrumb = By.Id("areaBreadCrumb");
        public static readonly By areaBreadCrumbText = By.Id("areaBreadCrumbText");
        public static readonly By appBreadCrumb = By.Id("appBreadCrumb");
        public static readonly By appBreadCrumbText = By.Id("appBreadCrumbText");
    }
}
