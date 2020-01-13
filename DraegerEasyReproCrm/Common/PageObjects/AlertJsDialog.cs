using System;
using System.Collections.Generic;
using System.Linq;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OptionSet = Microsoft.Dynamics365.UIAutomation.Api.UCI.OptionSet;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class AlertJsDialog : EntityPageBase
    {
        public AlertJsDialog(XrmApp xrmApp, WebClient client) : base(xrmApp, client)
        {
        }

        /// <summary>
        /// Tries to close the Opportunity with the given values.
        /// Checks also if the 'Reason for closing' field is marked as mandatory.
        /// </summary>
        /// <param name="reasonForClosing"></param>
        /// <param name="comment"></param>
        /// <param name="thinkTime"></param>
        /// <returns></returns>
        //public BrowserCommandResult<bool> CloseOpportunityAsWon(string reasonForClosing, string comment, int thinkTime = Constants.DefaultThinkTime)
        //{
        //    var t = new BrowserCommandOptions();
        //    return Execute(GetOptions("Close Opportunity As Won"), driver =>
        //    {
        //        Browser.ThinkTime(thinkTime);

        //        // Check if Opportunity Closing dialog appeared
        //        driver.WaitForElement(By.XPath("//*[@id=\"alertJs-alert-dialogOpportunityCloseDialog\"]"),
        //            new TimeSpan(0, 0, 10),
        //            "The Close Opportunity dialog is not available.");

        //        // Check if 'Reason for closing' field has red border
        //        if (!HasRedBorder(By.Id("alertJs-dialogOpportunityCloseDialog-field1"))) return false;

        //        SetValue(new OptionSet { Name = "alertJs-dialogOpportunityCloseDialog-field1", Value = reasonForClosing });
        //        SetValue("alertJs-dialogOpportunityCloseDialog-field2", comment);

        //        // Try to close without filling out the reason for closing
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[text() ='Finish']"));

        //        return true;
        //    });
        //}

        ///// <summary>
        ///// Try to Close the Opportunity Closing Dialog blank reason for closing field and check result.
        ///// If error dialog appears the opportunity is closed with the given values.
        ///// Checks also if the 'Reason for closing' field is marked as mandatory.
        ///// </summary>
        ///// <param name="reasonForClosing"></param>
        ///// <param name="comment"></param>
        ///// <param name="thinkTime"></param>
        ///// <returns></returns>
        //public BrowserCommandResult<bool> CloseOpportunityAsWonWithoutReasonPressCancel(string reasonForClosing, string comment, int thinkTime = Constants.DefaultThinkTime)
        //{
        //    return Execute(GetOptions("Close Opportunity"), driver =>
        //    {
        //        Browser.ThinkTime(thinkTime);

        //        // Check if Opportunity Closing dialog appeared
        //        driver.WaitForElement(By.XPath("//*[@id=\"alertJs-alert-dialogOpportunityCloseDialog\"]"),
        //            new TimeSpan(0, 0, 10),
        //            "The Close Opportunity dialog is not available.");

        //        // Check if 'Reason for closing' field has red border
        //        if (!HasRedBorder(By.Id("alertJs-dialogOpportunityCloseDialog-field1"))) return false;

        //        // Try to close without filling out the reason for closing
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[text() ='Finish']"));

        //        // Check for validation error dialog
        //        driver.WaitForElement(By.XPath("//*[@id='alertJs-alert-validationErrorCloseOpportunity']"),
        //            new TimeSpan(0, 0, 10),
        //            "The Validation Error dialog is not available.");

        //        // Close Validation Error dialog by clicking OK
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[@type='button' and text()='OK']"));

        //        // Fill in Comment
        //        SetValue("alertJs-dialogOpportunityCloseDialog-field2", comment);

        //        // Try to close again
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[text() ='Finish']"));

        //        // Validation Error should appear again
        //        driver.WaitForElement(By.XPath("//*[@id='alertJs-alert-validationErrorCloseOpportunity']"),
        //            new TimeSpan(0, 0, 10),
        //            "The Validation Error dialog is not available.");

        //        // Close Validation Error dialog by clicking OK
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[@type='button' and text()='OK']"));

        //        // Reason for closing is empty by default
        //        var reasonForClosingValue =
        //            driver.FindElement(By.XPath("//*[@id='alertJs-dialogOpportunityCloseDialog-field1']"));

        //        var reasonsForClosingIs =
        //            GetOptionsForMultiOptionField(By.XPath("//*[@id='alertJs-dialogOpportunityCloseDialog-field1']"));


        //        List<string> reasonsForClosingShouldBe = OpportunityConstants.ClosingReasons;

        //        var res = reasonsForClosingIs.Except(reasonsForClosingShouldBe);
        //        if (res.Any())
        //        {
        //            throw new InvalidOperationException("Reason for Closing list not as expected");
        //        }

        //        SetValue(new OptionSet { Name = "alertJs-dialogOpportunityCloseDialog-field1", Value = reasonForClosing });

        //        // Try to close without filling out the reason for closing
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[text() ='Cancel']"));

        //        return true;
        //    });
        //}

        //public BrowserCommandResult<bool> CloseOpportunityAsWonWithoutReasonPressFinish(string reasonForClosing, string comment, int thinkTime = Constants.DefaultThinkTime)
        //{
        //    return Execute(GetOptions("Close Opportunity"), driver =>
        //    {
        //        Browser.ThinkTime(thinkTime);

        //        // Check if Opportunity Closing dialog appeared
        //        driver.WaitForElement(By.XPath("//*[@id=\"alertJs-alert-dialogOpportunityCloseDialog\"]"),
        //            new TimeSpan(0, 0, 10),
        //            "The Close Opportunity dialog is not available.");

        //        // Check if 'Reason for closing' field has red border
        //        if (!HasRedBorder(By.Id("alertJs-dialogOpportunityCloseDialog-field1"))) return false;

        //        // Try to close without filling out the reason for closing
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[text() ='Finish']"));

        //        // Check for validation error dialog
        //        driver.WaitForElement(By.XPath("//*[@id='alertJs-alert-validationErrorCloseOpportunity']"),
        //            new TimeSpan(0, 0, 10),
        //            "The Validation Error dialog is not available.");

        //        // Close Validation Error dialog by clicking OK
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[@type='button' and text()='OK']"));

        //        // Fill in Comment
        //        SetValue("alertJs-dialogOpportunityCloseDialog-field2", comment);

        //        // Try to close again
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[text() ='Finish']"));

        //        // Validation Error should appear again
        //        driver.WaitForElement(By.XPath("//*[@id='alertJs-alert-validationErrorCloseOpportunity']"),
        //            new TimeSpan(0, 0, 10),
        //            "The Validation Error dialog is not available.");

        //        // Close Validation Error dialog by clicking OK
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[@type='button' and text()='OK']"));

        //        // Reason for closing is empty by default
        //        var reasonForClosingValue =
        //            driver.FindElement(By.XPath("//*[@id='alertJs-dialogOpportunityCloseDialog-field1']"));

        //        var reasonsForClosingIs =
        //            GetOptionsForMultiOptionField(By.XPath("//*[@id='alertJs-dialogOpportunityCloseDialog-field1']"));

        //        List<string> reasonsForClosingShouldBe = OpportunityConstants.ClosingReasons;

        //        var res = reasonsForClosingIs.Except(reasonsForClosingShouldBe);
        //        if (res.Any())
        //        {
        //            throw new InvalidOperationException("Reason for Closing list not as expected");
        //        }

        //        SetValue(new OptionSet { Name = "alertJs-dialogOpportunityCloseDialog-field1", Value = reasonForClosing });

        //        // Try to close without filling out the reason for closing
        //        driver.ClickWhenAvailable(By.XPath("//*[@id='alertJs-buttonContainer']/button[text() ='Finish']"));

        //        return true;
        //    });
        //}

        internal void SetValue(OptionSet option)
        {
            var element = browser.Driver.FindElement(By.Id(option.Name));
            var dropdown = new SelectElement(element);

            dropdown.SelectByText(option.Value, true);
        }

        internal string GetValue(OptionSet option)
        {
            var element = browser.Driver.FindElement(By.Id(option.Name));
            var dropdown = new SelectElement(element);

            return dropdown.SelectedOption.Text;
        }


        protected bool HasRedBorder(By by)
        {
            var webDriver = browser.Driver;
            var webElement = webDriver.WaitForElement(by);
            var borderColor = webElement.GetCssValue("border-color");
            var retVal = (borderColor.ToUpper() == "RGB(255, 0, 0)") || (borderColor.ToUpper() == "#FF0000") || (borderColor.ToUpper() == "RED");

            return retVal;
        }

        public IEnumerable<string> GetOptionsForMultiOptionField(By by, bool filterEmptyEntries = true)
        {
            var webDriver = Client.Browser.Driver;

            var dropdown = new SelectElement(webDriver.FindElement(by));
            var options = dropdown.Options;

            var optionList = options
                .Where(option => !filterEmptyEntries || (option != null && option.Text != string.Empty))
                .Select(option => option.Text);

            return optionList;

        }

        protected override bool GetBooleanItem(string name)
        {
            IWebElement webElement = browser.Driver.WaitForElement(By.XPath("//input[contains(@id,'[NAME]')]".Replace("[NAME]", name)));
            if (webElement == null)
                throw new NoSuchElementException();

            return webElement.Selected;
        }

        protected override void SetBooleanItem(string name, bool value)
        {
            IWebElement webElement = browser.Driver.WaitForElement(By.XPath("//input[contains(@id,'[NAME]')]".Replace("[NAME]", name)));
            if (webElement == null)
                throw new NoSuchElementException();
            if(webElement.Selected != value)
                webElement.ClickWait();
        }

        public bool GetIsMandatory(By by)
        {
            return HasRedBorder(by);
        }

        protected bool AlertJsDialogVisible(By by, bool visible)
        {
            browser.Driver.WaitFor(d => AlertJsDialogVisible(by) == visible, TimeSpan.FromSeconds(5));
            return AlertJsDialogVisible(by);
        }

        private bool AlertJsDialogVisible(By by)
        {
            var jsDialog = browser.Driver.FindElements(by);
            if (!jsDialog.Any())
                return false;
            var alertJs = jsDialog.First();
            var attribute = alertJs.GetAttribute("style");
            return !attribute.Contains("display: none");
        }

        public string GetValue(By by)
        {
            var webElement = browser.Driver.WaitForElement(by);
            return webElement.Text;
        }
        internal new bool SetValue(string field, string value, bool clearFocusOnEnd = true)
        {
            try
            {
                XrmApp.ThinkTime(1000);
                var fieldContainer = browser.Driver.WaitForElement(By.XPath("//textarea[contains(@id, '[NAME]')]".Replace("[NAME]", field)));

                if (fieldContainer.Displayed)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        fieldContainer.SendKeysWait(Keys.Control + "a");
                        fieldContainer.SendKeysWait(Keys.Backspace);
                    }
                    else
                    {
                        fieldContainer.SendKeysWait(Keys.Control + "a");
                        fieldContainer.SendKeysWait(Keys.Backspace);
                        fieldContainer.SendKeysWait(value, true);
                    }
                }
            }catch (Exception)
            {
                throw new Exception($"Field with name {field} does not exist.");
            }

            // Needed to transfer focus out of special fields (email or phone)
            if (clearFocusOnEnd)
                browser.Driver.FindElement(By.TagName("body")).ClickWait();

            return true;
        }

        protected string GetValue(string field)
        {
            string str = string.Empty;
            IWebElement webElement = browser.Driver.WaitForElement(By.XPath("//textarea[contains(@id,'[NAME]')]".Replace("[NAME]", field)));
            if (webElement == null)
                throw new NoSuchElementException();
            str = webElement.GetAttribute("value");

            return str;
        }
    }
}