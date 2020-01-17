using OpenQA.Selenium;

namespace Draeger.Dynamics365.Testautomation.Common.Locators
{
    public static class InstalledBasesLocators
    {
        public static class AlertJsDialog
        {
            public static readonly By
                Root = By.Id("alertJs-alert-finalMessage"); //"//*[@id='alertJs-alert-dialogOpportunityCloseDialog']";

            public static readonly By
                Title = By.Id("alertJs-title");

            public static readonly By
                Message = By.Id("alertJs-message");

            public static readonly By
                Content = By.Id("alertJs-content");

            public static readonly By
                Ok = By.XPath("//div[@id='alertJs-buttonContainer']/button");
        }
    }
}