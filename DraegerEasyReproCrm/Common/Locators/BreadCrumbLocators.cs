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
        public static readonly By entitybreadCrumbText = By.XPath("//span[@data-id='entityBreadCrumbText']");
        public static readonly By entitybreadCrumb = By.XPath("//span[@data-id='entityBreadCrumb']");
        public static readonly By areaBreadCrumb = By.XPath("//span[@data-id='areaBreadCrumb']");
        public static readonly By areaBreadCrumbText = By.XPath("//span[@data-id='areaBreadCrumbText']");
        public static readonly By appBreadCrumb = By.XPath("//span[@data-id='appBreadCrumb']");
        public static readonly By appBreadCrumbText = By.XPath("//span[@data-id='appBreadCrumbText']");
        public static readonly By recordBreadCrumbText = By.XPath("//span[@data-id='recordBreadCrumbText']");
    }
}
