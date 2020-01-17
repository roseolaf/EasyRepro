using System;
using Draeger.Dynamics365.Testautomation.Common.Enums;
using Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class ServiceOrderEntity : EntityPageBase
    {
        public ServiceOrderEntity(XrmApp xrm, WebClient client) : base(xrm, client)
        {
        }


        public string QuickCreate_TimeAndMaterial
        {
            get => GetSimpleLookupField("dw_localmaterialid");
            set => SetSimpleLookupField("dw_localmaterialid", value);
        }
        public string QuickCreate_Price
        {
            get => GetTextField("dw_price");
            set => SetSimpleLookupField("dw_price", value);
        }


    }
}
