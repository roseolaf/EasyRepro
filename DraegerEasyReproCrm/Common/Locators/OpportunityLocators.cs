using OpenQA.Selenium;

namespace Draeger.Dynamics365.Testautomation.Common.Locators
{
    public static class OpportunityLocators
    {
        public static class ClosingDialog
        {
            public static readonly By Root = By.Id("alertJs-alert-dialogOpportunityCloseDialog"); //"//*[@id='alertJs-alert-dialogOpportunityCloseDialog']";

            public static readonly string ReasonForClosingFieldId = "alertJs-dialogOpportunityCloseDialog-field1";
            public static readonly By ReasonForClosingField = By.Id(ReasonForClosingFieldId); //"//*[@id='alertJs-dialogOpportunityCloseDialog-field1']";
            public static readonly By ReasonForClosingFieldOptions = By.XPath("//*[@id='alertJs-dialogOpportunityCloseDialog-field1']/option"); //"//*[@id='alertJs-dialogOpportunityCloseDialog-field1']/option";

            public static readonly string MainCompetitorThatWonTheDealId = "alertJs-dialogOpportunityCloseDialog-field2";
            public static readonly By MainCompetitorThatWonTheDealField = By.Id(MainCompetitorThatWonTheDealId);
            public static readonly By MainCompetitorThatWonTheDealOptions = By.XPath($"//*[@id='{MainCompetitorThatWonTheDealId}']/option");

            public static readonly string SalesAbandonedId = "alertJs-dialogOpportunityCloseDialog-field4";
            public static readonly By SalesAbandonedField = By.XPath(SalesAbandonedId);

            public static readonly string CommentFieldIdWon = "alertJs-dialogOpportunityCloseDialog-field2";
            public static readonly By CommentFieldWon = By.Id(CommentFieldIdWon); //"//*[@id='alertJs-dialogOpportunityCloseDialog-field2']";

            public static readonly string CommentFieldIdLost = "alertJs-dialogOpportunityCloseDialog-field6";
            public static readonly By CommentFieldLost = By.Id(CommentFieldIdLost); //"//*[@id='alertJs-dialogOpportunityCloseDialog-field2']";

            public static readonly By FinishButton = By.XPath("//div[@id='alertJs-buttonContainer']/button[text() ='Finish']"); //"//*[@id='alertJs-buttonContainer']/button[text() ='Finish']";

            public static readonly By CancelButton = By.XPath("//div[@id='alertJs-buttonContainer']/button[text() ='Cancel']"); // "//*[@id='alertJs-buttonContainer']/button[text() ='Cancel']";
        }

        public static class ValidationErrorDialog
        {
            public static readonly By Root = By.Id("alertJs-alert-validationErrorCloseOpportunity"); // "//*[@id='alertJs-alert-validationErrorCloseOpportunity']";
            public static readonly By MessageText = By.XPath("//div[@id='alertJs-message' and ./text()='Please fill in all mandatory fields to close the opportunity']"); // "//*[@id='alertJs-message' and ./text()='Please fill in all mandatory fields to close the opportunity']";
            public static readonly By OkButton = By.XPath("//div[@id='alertJs-buttonContainer']/button[@type='button' and text()='OK']"); // "//*[@id='alertJs-buttonContainer']/button[@type='button' and text()='OK']";
        }

        public static readonly string CloseAsWon = "Close As Won";
        public static readonly string CloseAsLost = "Close As Lost";
        public static readonly string CloseAsCanceled = "Close As Canceled";
        public static readonly string ReopenOpportunity = "Reopen Opportunity";

        public static readonly string WarningNotification = "warningNotification";
    }
}