using System.Collections.Generic;

namespace Draeger.Dynamics365.Testautomation.Common.Locators
{
    public static class OpportunityConstants
    {
        public static class Probability
        {
            public static readonly string OrderLost = "0 % - Order lost";
            public static readonly string VeryUnlikely = "10 % - Very Unlikely";
            public static readonly string FewChances = "30 % - Few chances";
            public static readonly string GoodChance = "50 % - Good Chance";
            public static readonly string OrderExpected = "70 % - Order expected";
            public static readonly string OrderReceived = "100 % - Order received";
        }

        public static class TimingAccuracy
        {
            public static readonly string Poor = "Poor (Could slip 3 months or more)";
            public static readonly string Fair = "Fair (Could slip between 1 and 3 months)";
            public static readonly string Good = "Good (Will not slip more than 1 month)";
        }
        
        public static class ClosingDialogLocators
        {
            public static readonly string ClosingDialogRootXpath = "";
        }

        public static List<string> ClosingReasonsLost
        {
            get
            {
                return new List<string>
                {
                    "Ability to Deliver (Lost)",
                    "Life Cycle/Service Costs (Lost)",
                    "Non-Compliant Business (Lost)",
                    "Product Price (Lost)",
                    "Product Quality (Lost)",
                    "Relationship (Lost)",
                    "Technical Specs (Lost)",
                    "Terms/Conditions/Warranty (Lost)",
                    "Total Cost of Ownership (Lost)"
                };
            }
        }
        public static List<string> ClosingReasonsWon
        {
            get
            {
                return new List<string>
                {
                    "Ability to Deliver (Won)",
                    "Life Cycle/Service Costs (Won)",
                    "Product Price (Won)",
                    "Product Quality (Won)",
                    "Relationship (Won)",
                    "Technical Specs (Won)",
                    "Terms/Conditions/Warranty (Won)",
                    "Total Cost of Ownership (Won)"
                };
            }
        }

        public static List<string> ClosingReasonsCanceled
        {
            get
            {
                return new List<string>
                {
                    "Canceled",
                    "Duplicate"

                };
            }
        }
    }
}