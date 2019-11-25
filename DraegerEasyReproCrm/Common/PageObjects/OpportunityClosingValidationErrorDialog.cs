using System;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class OpportunityClosingValidationErrorDialog : AlertJsDialog
    {
        public OpportunityClosingValidationErrorDialog(XrmApp xrm, WebClient client) : base(xrm, client)
        {

        }

        public bool DialogIsOpen(bool value) => AlertJsDialogVisible(By.Id("alertJs-alert-validationErrorCloseOpportunity"), value);


        public string GetDialogMessage()
        {
            browser.Driver.WaitForElement(OpportunityLocators.ValidationErrorDialog.Root, TimeSpan.FromSeconds(5));
            return browser.Driver.FindElement(OpportunityLocators.ValidationErrorDialog.MessageText)
                .Text;
        }

        public bool PressOk()
        {
            if (!DialogIsOpen(true)) return false;
            // Check if 'Reason for closing' field has red border
            browser.Driver.FindElement(OpportunityLocators.ValidationErrorDialog.OkButton).ClickWait();

            return true;
        }
    }
}