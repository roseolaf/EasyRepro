using Draeger.Dynamics365.Testautomation.Common.Enums;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using System;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class CaseEntity : EntityPageBase
    {
        public CaseEntity(XrmApp xrm, WebClient client = null) : base(xrm, client)
        {
        }

        public string ExistingInstitution
        {
            get => GetLookupField("customerid");
            set => SetLookupField("customerid", value);
        }

        public string ExistingContact
        {
            get => GetLookupField("primarycontactid");
            set => SetLookupField("primarycontactid", value);
        }

        public string Origin
        {
            get => GetOptionField("caseorigincode");
            set => SetOptionField("caseorigincode", value);
        }

        public string CaseTitle
        {
            get => GetTextField("title");
            set => SetTextField("title", value);
        }

        public string InquiryType
        {
            get => GetLookupField("dw_inquirytype");
            set => SetLookupField("dw_inquirytype", value);
        }

        public string FirstName
        {
            get => GetTextField("dw_firstname");
            set => SetTextField("dw_firstname", value);
        }
        public string LastName
        {
            get => GetTextField("dw_lastname");
            set => SetTextField("dw_lastname", value);
        }
        public string BusinessPhone
        {
            get => GetTextField("dw_businessphone");
            set => SetTextField("dw_businessphone", value);
        }

        public string MobilePhone
        {
            get => GetTextField("dw_mobilephone");
            set => SetTextField("dw_mobilephone", value);
        }
        public string EmailAddress
        {
            get => GetTextField("dw_emailaddress");
            set => SetTextField("dw_emailaddress", value);
        }

        public string FaxNumber
        {
            get => GetTextField("dw_faxnumber");
            set => SetTextField("dw_faxnumber", value);
        }

        public string CallbackNumber
        {
            get => GetTextField("dw_callbacknumber");
            set => SetTextField("dw_callbacknumber", value);
        }

        public string ReportedTopic
        {
            get => GetTextField("dw_reportedtopic");
            set => SetTextField("dw_reportedtopic", value);
        }

        public string Name
        {
            get => GetTextField("dw_name");
            set => SetTextField("dw_name", value);
        }

        public string Street
        {
            get => GetTextField("dw_street");
            set => SetTextField("dw_street", value);
        }

        public string City
        {
            get => GetTextField("dw_city");
            set => SetTextField("dw_city", value);
        }

        public string Postalcode
        {
            get => GetTextField("dw_postalcode");
            set => SetTextField("dw_postalcode", value);
        }

        public string Country
        {
            get => GetTextField("dw_country");
            set => SetTextField("dw_country", value);
        }
        public bool CallbackRequired
        {
            get
            {
                return GetBooleanItem("dw_callbackrequired");
            }
            set
            {
                SetBooleanItem("dw_callbackrequired", value);
            }
        }
        public bool DeviceComplaint
        { 
            get 
            {
                return GetBooleanItem("dw_devicecomplaint");
            }
            set
            {
                SetBooleanItem("dw_devicecomplaint", value);
            }
        }


 

    }
}
