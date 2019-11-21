// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Microsoft.Dynamics365.UIAutomation.Api
{
    public class Office365
        : XrmPage
    {
        public Office365(InteractiveBrowser browser)
            : base(browser)
        {
            SwitchToPopup();
        }

        private static BrowserCommandOptions NavigationRetryOptions
        {
            get
            {
                return new BrowserCommandOptions(
                    Constants.DefaultTraceSource,
                    "Add User",
                    0,
                    1000,
                    null,
                    true,
                    typeof(StaleElementReferenceException));
            }
        }

        public BrowserCommandResult<bool> CreateUser(string firstname, string lastname, string displayname, string username, int thinkTime = Constants.DefaultThinkTime)
        {
            this.Browser.ThinkTime(thinkTime);

            return this.Execute(NavigationRetryOptions, driver =>
            {
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Office365.AddUser])).ClickWait();

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Office365.FirstName])).SendKeysWait(firstname, true).ClickWait();
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Office365.LastName])).SendKeysWait(lastname, true).ClickWait();
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Office365.DisplayName])).SendKeysWait(displayname,true).ClickWait();
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Office365.UserName])).SendKeysWait(username, true).ClickWait();

                //ClickWait the Microsoft Dynamics CRM Online Professional License
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Office365.License])).ClickWait();

                //ClickWait Add
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Office365.Add])).ClickWait();

                driver.LastWindow().Close();
                driver.LastWindow();
                
            return true;
            });
        }
    }
}