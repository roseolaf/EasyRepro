using System;
using Draeger.Dynamics365.Testautomation.Common.Locators;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class OpportunityClosingDialog : AlertJsDialog
    {
        public OpportunityClosingDialog(XrmApp xrm, WebClient client) : base(xrm, client)
        {
        }

        public string ReasonForClosing
        {
            get => GetValue(new OptionSet(){Name = OpportunityLocators.ClosingDialog.ReasonForClosingFieldId});

            set => SetValue(new OptionSet(){Name = OpportunityLocators.ClosingDialog.ReasonForClosingFieldId, Value = value});
        }

        public string MainCompetitorThatWonTheDeal
        {
            get => GetValue(new OptionSet() { Name = OpportunityLocators.ClosingDialog.MainCompetitorThatWonTheDealId });

            set => SetValue(new OptionSet() { Name = OpportunityLocators.ClosingDialog.MainCompetitorThatWonTheDealId, Value = value });
        }

        public string CommentWonOrCanceled
        {
            get => GetValue(OpportunityLocators.ClosingDialog.CommentFieldIdWon);
            set => SetValue(OpportunityLocators.ClosingDialog.CommentFieldIdWon, value);
        }
        public string CommentLost
        {
            get => GetValue(OpportunityLocators.ClosingDialog.CommentFieldIdLost);
            set => SetValue(OpportunityLocators.ClosingDialog.CommentFieldIdLost, value);
        }

        public void Finish()
        {
            browser.Driver.WaitForElement(OpportunityLocators.ClosingDialog.FinishButton).Click();
            new WebDriverWait(browser.Driver, TimeSpan.FromSeconds(60)).Until(x => browser.Driver.FindElements(By.XPath("//*[contains(@id, 'alertJs-alert-loading')]")).Count == 0);
        }

        public void Cancel()
        {
            browser.Driver.WaitForElement(OpportunityLocators.ClosingDialog.CancelButton).ClickWait();
            browser.Driver.WaitForTransaction();
        }

        public bool DialogIsOpen(bool value) => AlertJsDialogVisible(By.Id("alertJs-alert-dialogOpportunityCloseDialog"),value);

        public bool SalesAbandoned
        {
            get => GetBooleanItem(OpportunityLocators.ClosingDialog.SalesAbandonedId);
            set => SetBooleanItem(OpportunityLocators.ClosingDialog.SalesAbandonedId, value);
        }
    }

}