using System;
using System.Collections.ObjectModel;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects.PageElements
{
    public class TimelineElement : EntityPageBase
    {
        public TimelineElement(XrmApp xrm, WebClient client = null) : base(xrm, client)
        {
        }

        public bool IsVisible
        {
            get
            {
                browser.Driver.WaitForElement(By.XPath("//div[contains(@id, 'TimelineGroupsMainContainer')]"),TimeSpan.FromSeconds(5));
                return browser.Driver.FindElements(By.XPath("//div[contains(@id, 'TimelineGroupsMainContainer')]")).Count > 0;
            }
        }

        public IWebElement Container
        {
            get
            {
                if (!IsVisible) throw new NoSuchElementException("Timeline element not found");
                return browser.Driver.FindElement(By.XPath("//div[contains(@id, 'TimelineGroupsMainContainer')]"));
            }
        }

            

        public TimelineRecord this[int index] 
        {
            get
            {
                if (!IsVisible) throw new NoSuchElementException("Timeline element not found");
                return new TimelineRecord(this, index);
            }
        }


    }

    public class TimelineRecord 
    {
        private readonly TimelineElement _parent;
        private readonly int _index;

        public TimelineRecord(TimelineElement parent, int index)
        {
            _parent = parent;
            _index = index;
        }

        public IWebElement Element
        {
            get
            {
                if (!_parent.IsVisible) throw new NoSuchElementException("Timeline element not found");
                return _parent.Container.FindElements(By.XPath(".//div[contains(@id,'timeline_record_control')]"))[_index];
            }
        }

        public string HeaderTitle 
        {
            get
            {
                if (!_parent.IsVisible) throw new NoSuchElementException("Timeline element not found");
                return Element.FindElement(By.XPath(".//div[contains(@id,'timeline_record_header_title')]")).Text;//.GetAttribute("textContent");
            }
        }

        public string TitleText
        {
            get
            {
                if (!_parent.IsVisible) throw new NoSuchElementException("Timeline element not found");
                return Element.FindElements(By.XPath(".//div[contains(@id,'timeline_record_title_text')]")).Count > 0 ? Element.FindElement(By.XPath(".//div[contains(@id,'timeline_record_title_text')]")).Text : null;
            }
        }

        public string Content
        {
            get
            {
                if (!_parent.IsVisible) throw new NoSuchElementException("Timeline element not found");
                var elem = Element.FindElement(By.XPath(".//div[contains(@id,'timeline_record_content')]"));
                //var childs = elem.FindElements(By.XPath(".//*"));
                //var text = "";
                //foreach (var child in childs)
                //    text += child.Text;
                var text = elem.GetAttribute("textContent");//.GetAttribute("textContent");
                return text;
            }
        }


        public DateTime CreatedDate
        {
            get
            {
                if (!_parent.IsVisible) throw new NoSuchElementException("Timeline element not found");
                return DateTime.Parse(Element.FindElement(By.XPath(".//div[contains(@id,'timeline_record_time')]")).Text);//.GetAttribute("textContent");
            }
        }


    }
}