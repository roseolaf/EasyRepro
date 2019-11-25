using System;
using Draeger.Dynamics365.Testautomation.Common.Enums;
using Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class OpportunityEntity : EntityPageBase
    {
        public OpportunityEntity(XrmApp xrm, WebClient client) : base(xrm, client)
        {
            TimelineControl = new TimelineElement(XrmApp, Client);
        }

        public string Topic
        {
            get => GetTextField("name");
            set => SetTextField("name", value);
        }

        public TimelineElement TimelineControl { get; }

        public string SoldToParty
        {
            get => throw new NotImplementedException();
            set => SetCustomLookupField("dw_soldtopartyid", value);
        }

        public string ShipToParty
        {
            get => throw new NotImplementedException();
            set => SetLookupField("dw_shiptopartyid", value);
        }

        public string EndUserParty
        {
            get => throw new NotImplementedException();
            set => SetLookupField("dw_enduserpartyid", value);
        }

        public string SalesChannelPartner
        {
            get => throw new NotImplementedException();
            set => SetLookupField("dw_saleschannelpartnerid", value);
        }

        public string Value
        {
            get => GetTextField("estimatedvalue");
            set => SetTextField("estimatedvalue", value);
        }

        public string Currency
        {
            get => GetLookupField("transactioncurrencyid");
            set => SetLookupField("transactioncurrencyid", value);
        }

        public string Probability
        {
            get => GetOptionField("dw_probability");
            set => SetOptionField("dw_probability", value);
        }

        public DateTime OrderEntryDate
        {
            get => GetDateField("estimatedclosedate");
            set => SetDateField("estimatedclosedate", value);
        }

        public DateTime CustomerDeliveryDate
        {
            get => GetDateField("dw_customerdeliverydate");
            set => SetDateField("dw_customerdeliverydate", value);
        }

        /// <summary>
        /// Field Name: dw_timingaccuracy
        /// </summary>
        public string TimingAccuracy
        {
            get => GetOptionField("dw_timingaccuracy");
            set => SetOptionField("dw_timingaccuracy", value);
        }

 
        public string PrimaryContact
        {
            get => GetLookupField("dw_primarycontactid");
            set => SetLookupField("dw_primarycontactid", value);
        }

        public string MainProductArea
        {
            get => GetLookupField("dw_mainproductareaid");
            set => SetLookupField("dw_mainproductareaid", value);
        }

        public string AdditionalInformation
        {
            get => GetTextField("description");
            set => SetTextField("description", value);
        }

        public string CustomerSolution
        {
            get => GetLookupField("dw_customersolutionid");
            set => SetLookupField("dw_customersolutionid", value);
        }

        public bool IsFunnelRelevant
        {

            get => GetBooleanItem("dw_isfunnelrelevant");
            set => SetBooleanItem("dw_isfunnelrelevant", value);
        }

        public bool SalesAbandoned
        {

            get => GetBooleanItem("dw_salesabandoned");
            set => SetBooleanItem("dw_salesabandoned", value);
        }

        public bool GetUnsavedChangesNotificationVisible()
        {
            return GetNotificationShown("warningNotification",
                "You have unsaved changes. Please save/refresh the opportunity to close it.");
        }
        public bool GetReadOnlyNotificationVisible()
        {
            return GetNotificationShown("warningNotification",
                "Read-only This record’s status:");
        }

        public string SelectRecord
        {
            get => throw new NotImplementedException();
            set => SetSimpleLookupField("falseBoundLookup", value);
        }

        public bool GetUnsavedChangesFooterVisible()
        {
            if (!browser.Driver.ElementExists(By.XPath("//div[contains(@id,'footerWrapper')]"), TimeSpan.FromSeconds(2)))
                throw new NotFoundException("Unable to find footer on the form");

            var element = browser.Driver.FindElement(By.XPath("//div[contains(@id,'footerWrapper')]//div[contains(@role,'alert')]/span"));
            var text = element.GetAttribute("textContent");
            return !string.IsNullOrEmpty(text) && text.Contains("unsaved changes");
        }

        public string Status
        {
            get => GetOptionField("statecode");
            set => SetOptionField("statecode", value);
        }

        public string StatusReason
        {
            get => GetReadOnlyPicklistField("statuscode");
            set => SetTextField("statuscode", value);
        }

        public string ClosingComment
        {
            get => GetTextField("dw_closingcomment");
            set => SetTextField("dw_closingcomment", value);
        }

        public string MainCompetitor
        {
            get => GetLookupField("dw_maincompetitorid");
            //set => SetTextField("dw_maincompetitorid", value);
        }


        //public BrowserCommandResult<string> GetStatus()
        //{
        //    return Execute(GetOptions("Close Opportunity"), driver =>
        //    {
        //        //Browser.ThinkTime(2000);

        //        var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
        //        var res = wait.Until(ExpectedConditions.ElementExists(By.Id("Status_label")));

        //        //var res = driver.FindElement(By.XPath("//*[@data-for-id='statecode']"));
        //        //driver.WaitForElement(By.XPath("//*[@data-for-id='statecode_label']"));

        //        return res.Text;
        //        //return driver.FindElement(By.Id("Status_label")).Text;
        //    });
        //}

        //public BrowserCommandResult<string> GetStatusReason()
        //{
        //    return Execute(GetOptions("Close Opportunity"), driver =>
        //    {
        //        Browser.ThinkTime(2000);

        //        return driver.FindElement(By.Id("Status Reason_label")).Text;
        //    });
        //}

        //public BrowserCommandResult<bool> AddSalesTeamUser(string userName)
        //{
        //    Browser.ThinkTime(Constants.DefaultThinkTime);

        //    return Execute(GetOptions("Add Sales Team User"), driver =>
        //    {
        //        var xrmBrowser = Browser as Browser;

        //        driver.SwitchTo().DefaultContent();
        //        driver.SwitchTo().Frame("contentIFrame1");
        //        driver.ClickWhenAvailable(By.Id("Pursuit_Team_addImageButtonImage"));

        //        // Click Search Icon
        //        driver.ClickWhenAvailable(By.Id("lookup_Pursuit_Team_i"), TimeSpan.FromSeconds(2));

        //        // Find 'Look Up More Records' item
        //        driver.ClickWhenAvailable(By.ClassName("ms-crm-IL-MenuItem-Anchor-Lookupmore"), TimeSpan.FromSeconds(2));
        //        xrmBrowser.ThinkTime(3000);

        //        // Switch to Dialog Context
        //        // InlineDialog_Iframe
        //        driver.SwitchTo().DefaultContent();
        //        driver.SwitchTo().Frame("InlineDialog_Iframe");

        //        // Enter Text in Search Field
        //        // crmGrid_findCriteria
        //        var searchField = driver.ClickWhenAvailable(By.Id("crmGrid_findCriteria"), TimeSpan.FromSeconds(2));
        //        searchField.SendKeysWait(Keys.Clear);
        //        searchField.SendKeysWait("jennifer");
        //        searchField.SendKeysWait(Keys.Enter);
        //        Browser.ThinkTime(2000);

        //        // Click first Entry
        //        //var firstEntry = driver.FindElement(By.ClassName("ms-crm-List-Row"));
        //        // xpath=//table[@id='gridBodyTable']/tbody/tr/td[2]/nobr
        //        //driver.ClickWhenAvailable(By.ClassName("ms-crm-List-Row"),TimeSpan.FromSeconds(2));
        //        // "tr.ms-crm-List-DataCell";
        //        //var elem = driver.ClickWhenAvailable(By.CssSelector("tr.ms-crm-List-DataCell"));
        //        var elem = driver.FindElement(By.CssSelector("tr.ms-crm-List-DataCell"));

        //        //var elem = driver.ClickWhenAvailable(By.XPath("//table[@id='gridBodyTable']/tbody/tr/td/img"));

        //        // Switch back to Content Frame
        //        driver.SwitchTo().Frame("contentIFrame1");

        //        return true;
        //    });
        //}

        public void AddStakeholder(string stakeholder)
        {

        }

        public void AddCompetitor(string competitor)
        {

        }
    }
}
