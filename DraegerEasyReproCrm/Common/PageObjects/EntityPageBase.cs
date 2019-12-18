using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Draeger.Dynamics365.Testautomation.Common.Enums;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class EntityPageBase : XrmApp
    {
        protected XrmApp XrmApp;
        protected WebClient Client;

        protected InteractiveBrowser browser => Client.Browser;

        public EntityPageBase(XrmApp xrm, WebClient client = null) : base(client)
        {
            Client = client;
            XrmApp = xrm;
        }

        protected DateTime GetDateField(string name)
        {
            return DateTime.Parse(XrmApp.Entity.GetValue(name));
        }

        protected void SetDateField(string name, DateTime date)
        {
            SetValue(name, date, "M\\/d\\/yyyy");
        }

        private void SetValue(string field, DateTime date, string format)
        {
            browser.Driver.WaitForTransaction();
            string xpathToFind = AppElements.Xpath[AppReference.Entity.FieldControlDateTimeInputUCI].Replace("[FIELD]", field);
            if (!Client.Browser.Driver.ElementExists(By.XPath(xpathToFind), TimeSpan.FromSeconds(2)))
                throw new InvalidOperationException("Field: " + field + " Does not exist");
            IWebElement fieldElement = browser.Driver.FindElement(By.XPath(xpathToFind)).ClickWait();
            var datePickerRoot = Client.Browser.Driver.FindElement(By.XPath("//div[contains(@id, \"[NAME]\") and contains(@id, \"date-container\")]/div".Replace("[NAME]", field)));
            browser.Driver.WaitFor(d => datePickerRoot.GetAttribute("class").Contains("is-open"));
            browser.Driver.WaitForTransaction();
            fieldElement.ClickWait();
            browser.Driver.WaitFor(d => !datePickerRoot.GetAttribute("class").Contains("is-open"));
            XrmApp.ThinkTime(300);
            var stringToSend = date.ToString(format);
            fieldElement.SendKeysWait(stringToSend, false);
            //browser.Driver.ExecuteScript("arguments[0].value = arguments[1];", fieldElement, stringToSend);

            /*
            var separate = stringToSend.Split('/');
            for (int i = 0; i<separate.Length;i++)
            {
                fieldElement.SendKeysWait(separate[i],false);
                if(i<separate.Length-1)
                    fieldElement.SendKeysWait("/", false);
            }
            */
            browser.Driver.WaitFor(d => fieldElement.GetAttribute("value") == date.ToString(format));
            browser.Driver.ClearFocus();
        }

        protected string GetTextField(string name)
        {
            return XrmApp.Entity.GetValue(name);
        }

        protected void SetTextField(string name, string value)
        {
            SetValue(name, value);
        }

        protected void SetLookupField(string name, string value)
        {
            XrmApp.Entity.SetValue(new LookupItem { Name = name, Value = value });
            //SetLookUpByValue(new LookupItem { Name = name, Value = value }, 0);

        }

        protected void SetCustomLookupField(string name, string value)
        {
            SetValue(name, value, false);
            XrmApp.ThinkTime(300);
            SetLookUpByValue(new LookupItem() { Name = name, Value = value }, 0);
        }

        protected void SetSimpleLookupField(string name, string value)
        {
            SimpleLookupField(new LookupItem() { Name = name, Value = value });
        }

        private void SetLookUpByValue(LookupItem control, int index)
        {
            new WebDriverWait(Client.Browser.Driver, TimeSpan.FromSeconds(10.0)).Until<IWebElement>((Func<IWebDriver, IWebElement>)(d => d.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.LookupFieldNoRecordsText].Replace("[NAME]", control.Name) + "|" + AppElements.Xpath[AppReference.Entity.LookupFieldResultList].Replace("[NAME]", control.Name)))));
            Client.Browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.Entity.TextFieldLookupMenu].Replace("[NAME]", control.Name)));
            IWebElement element = Client.Browser.Driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.TextFieldLookupMenu].Replace("[NAME]", control.Name)));
            Client.Browser.Driver.WaitForTransaction();
            if (Client.Browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.Entity.LookupFieldResultListItem].Replace("[NAME]", control.Name))) == null)
                throw new NotFoundException("No Results Matching " + control.Value + " Were Found.");
            List<ListItem> listItemList = Client.OpenDialog(element).Value;
            Client.Browser.Driver.WaitForTransaction();
            if (listItemList.Count == 0)
                throw new InvalidOperationException("List does not contain a record with the name:  " + control.Value);
            if (index + 1 > listItemList.Count)
                throw new InvalidOperationException(string.Format("List does not contain {0} records. Please provide an index value less than {1} ", (object)(index + 1), (object)listItemList.Count));
            ListItem listItem = listItemList[index];
            Client.Browser.Driver.FindElement(By.Id(listItem.Id)).ClickWait();
            Client.Browser.Driver.WaitForTransaction();
        }
        protected string GetLookupField(string name)
        {
            return XrmApp.Entity.GetValue(new LookupItem { Name = name });
        }

        protected void SetOptionField(string name, string value)
        {
            XrmApp.Entity.SetValue(new OptionSet { Name = name, Value = value });
            browser.Driver.WaitForTransaction();
        }


        protected string GetOptionField(string name)
        {
            return XrmApp.Entity.GetValue(new OptionSet { Name = name });
        }

        protected virtual bool GetBooleanItem(string name)
        {
            return XrmApp.Entity.GetValue(new BooleanItem { Name = name });
        }

        protected virtual void SetBooleanItem(string name, bool value)
        {
            XrmApp.Entity.SetValue(new BooleanItem { Name = name, Value = value });
        }

        public bool GetCommandBarButtonAvailability(string button)
        {
            return browser.Driver.ElementExists(By.XPath(AppElements.Xpath[AppReference.CommandBar.Button]
                .Replace("[NAME]", button))); ;
        }

        public string GetEntityNameNewForm
        {
            get => browser.Driver.WaitForElement(By.XPath("//*[@data-id=\"entity_name_span\"]")).Text;
        }
        public string GetEntityHeaderTitleNewForm
        {
            get => browser.Driver.WaitForElement(By.XPath("//*[@data-lp-id=\"form-header-title\"]")).Text;
        }
        public string GetFormLabel
        {
            get => browser.Driver.WaitForElement(By.XPath("//div[@data-id=\"editFormRoot\"]")).GetAttribute("aria-label");
        }

        public Requirement.Status GetFieldRequirement(string name)
        {
            var field = browser.Driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.TextFieldContainer].Replace("[NAME]", name)));
            var fieldValue = int.Parse(field.GetAttribute("data-fieldrequirement"));
            var enumValue = (Requirement.Status)fieldValue;
            return enumValue;//.ToString();
        }

        public string GetHeaderControlItem(string item)
        {
            var hCL = browser.Driver.WaitForElement(By.Id("headerControlsList"));
            foreach (var divElement in hCL.FindElements(By.XPath("./div")))
            {
                if (divElement.Text.Contains(item))
                {
                    return divElement.FindElement(By.XPath("./div/*[@aria-label]")).Text;
                }
            }
            throw new NotFoundException($"Header control item with name {item} not found");
        }

        public void ClickBusinessProcessFlowFinish()
        {
            browser.Driver.FindElement(By.XPath("//button[contains(@data-id,'finishButtonContainer')]")).ClickWait(true);
        }

        internal BrowserCommandResult<bool> SetValue(string field, string value, bool clearFocusOnEnd = true)
        {
            var fieldContainer = browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.Entity.TextFieldContainer].Replace("[NAME]", field)));
            
            if (fieldContainer.ElementExists(By.TagName("input")))
            {
                var input = fieldContainer.FindElement(By.TagName("input"));
                if (input != null)
                {
                    input.ClickWait();

                    if (string.IsNullOrEmpty(value))
                    {
                        input.SendKeysWait(Keys.Control + "a");
                        input.SendKeysWait(Keys.Backspace);
                    }
                    else
                    {
                        input.SendKeysWait(Keys.Control + "a");
                        input.SendKeysWait(Keys.Backspace);
                        browser.ThinkTime(100);
                        input.SendKeysWait(value, true);
                    }
                }
            }
            else if (fieldContainer.ElementExists(By.TagName("textarea")))
            {
                if (string.IsNullOrEmpty(value))
                {
                    fieldContainer.FindElement(By.TagName("textarea")).SendKeysWait(Keys.Control + "a");
                    fieldContainer.FindElement(By.TagName("textarea")).SendKeysWait(Keys.Backspace);
                }
                else
                {
                    fieldContainer.FindElement(By.TagName("textarea")).SendKeysWait(Keys.Control + "a");
                    fieldContainer.FindElement(By.TagName("textarea")).SendKeysWait(Keys.Backspace);
                    fieldContainer.FindElement(By.TagName("textarea")).SendKeysWait(value, true);
                }
            }
            else
            {
                throw new Exception($"Field with name {field} does not exist.");
            }

            // Needed to transfer focus out of special fields (email or phone)
            if (clearFocusOnEnd)
                browser.Driver.FindElement(By.TagName("body")).ClickWait();

            return true;
        }

        public bool GetNotificationShown(string name, string value = "")
        {

            if (!browser.Driver.ElementExists(By.XPath("//span[contains(@data-id, '[NAME]')]".Replace("[NAME]", name)), TimeSpan.FromSeconds(5)))
                return false;

            var notifications = browser.Driver.FindElements(By.XPath("//span[contains(@data-id, '[NAME]')]".Replace("[NAME]", name)));
            if (notifications.Count == 0)
                return false;
            if (String.IsNullOrEmpty(value))
                return true;
            foreach (var notification in notifications)
            {
                if (notification.Text.Contains(value))
                    return true;
            }

            return false;
        }

        public void SectionMoreCommands(string section, string command)
        {
            XrmApp.ThinkTime(800);
            browser.Driver.FindElement(By.XPath($"//li[contains(@id,'{section}')]/button")).ClickWait();
            browser.Driver.WaitForElement(By.XPath($"//div[contains(@id,'{section}_flyout')]//button[@aria-label='{command}']"), TimeSpan.FromSeconds(5)).ClickWait();
        }

        public void ClickAddRecord()
        {
            browser.Driver.WaitForElement(By.Id("lookupDialogSaveBtnText")).ClickWait();
        }

        internal void SimpleLookupField(LookupItem lookupitem)
        {
            browser.Driver.WaitForElement(By.XPath($"//*[contains(@id,'{lookupitem.Name}')]/input")).SendKeysWait(lookupitem.Value);
            XrmApp.ThinkTime(500);
            browser.Driver.WaitForElement(By.XPath($"//*[contains(@id,'{lookupitem.Name}') and contains(@id,'resultsContainer') and contains(@aria-label,'{lookupitem.Value}')]"), TimeSpan.FromSeconds(5)).Click();
            XrmApp.ThinkTime(500);
        }

        internal string GetReadOnlyPicklistField(string field)
        {
            var fieldContainer = browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.Entity.TextFieldContainer].Replace("[NAME]", field)));

            if (fieldContainer.FindElements(By.XPath(".//span[contains(@id, '[NAME].fieldControl-pickliststatus-comboBox_text-value')]".Replace("[NAME]", field))).Count > 0)
            {
                var element = fieldContainer.FindElement(By.XPath(
                    ".//span[contains(@id, '[NAME].fieldControl-pickliststatus-comboBox_text-value')]".Replace("[NAME]",
                        field)));
                if (element != null)
                    return element.Text;
                throw new NoSuchElementException("Field with name " + field + " does not exist.");
            }
            throw new NoSuchElementException("Field with name " + field + " does not exist.");
        }

        internal void ClickCommand(string name, string subname = "")
        {
            IWebElement webElement = (IWebElement)null;
            if (browser.Driver.ElementExists(By.XPath(AppElements.Xpath[AppReference.CommandBar.Container])))
                webElement = browser.Driver.FindElement(By.XPath(AppElements.Xpath[AppReference.CommandBar.Container]));
            if (webElement == null)
            {
                if (!browser.Driver.ElementExists(By.XPath(AppElements.Xpath[AppReference.CommandBar.ContainerGrid]), TimeSpan.FromSeconds(2)))
                    throw new InvalidOperationException("Unable to find the ribbon.");
                webElement = browser.Driver.FindElement(By.XPath(AppElements.Xpath[AppReference.CommandBar.ContainerGrid]));
            }
            var elements = webElement.FindElements(By.TagName("li"));
            if (elements.Any<IWebElement>((Func<IWebElement, bool>)(x => x.GetAttribute("aria-label").Equals(name, StringComparison.OrdinalIgnoreCase))))
            {
                elements.FirstOrDefault<IWebElement>((Func<IWebElement, bool>)(x => x.GetAttribute("aria-label").Equals(name, StringComparison.OrdinalIgnoreCase))).ClickWait(true);
                browser.Driver.WaitForTransaction();
            }
            else
            {
                if (!elements.Any<IWebElement>((Func<IWebElement, bool>)(x => x.GetAttribute("aria-label").Equals("More Commands", StringComparison.OrdinalIgnoreCase))))
                    throw new InvalidOperationException("No command with the name '" + name + "' exists inside of Commandbar.");
                elements.FirstOrDefault<IWebElement>((Func<IWebElement, bool>)(x => x.GetAttribute("aria-label").Equals("More Commands", StringComparison.OrdinalIgnoreCase))).ClickWait(true);
                browser.Driver.WaitForTransaction();
                if (!browser.Driver.ElementExists(By.XPath(AppElements.Xpath[AppReference.CommandBar.Button].Replace("[NAME]", name)), TimeSpan.FromSeconds(2)))
                    throw new InvalidOperationException("No command with the name '" + name + "' exists inside of Commandbar.");
                browser.Driver.FindElement(By.XPath(AppElements.Xpath[AppReference.CommandBar.Button].Replace("[NAME]", name))).ClickWait(true);
                browser.Driver.WaitForTransaction();
            }
            if (!string.IsNullOrEmpty(subname))
            {
                IWebElement element = browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.CommandBar.MoreCommandsMenu])).FindElements(By.TagName("button")).FirstOrDefault<IWebElement>((Func<IWebElement, bool>)(x => x.Text == subname));
                if (element == null)
                    throw new InvalidOperationException("No sub command with the name '" + subname + "' exists inside of Commandbar.");
                element.ClickWait(true);
            }
            browser.Driver.WaitForTransaction();
        }


        internal List<GridItem> GetGridItems(int thinkTime = Constants.DefaultThinkTime)
        {
            XrmApp.ThinkTime(thinkTime);

            var returnList = new List<GridItem>();

            browser.Driver.WaitForElement(By.XPath(AppElements.Xpath[AppReference.Grid.Container]));

            var rows = browser.Driver.FindElements(By.ClassName("wj-row"));
            var columnGroup = browser.Driver.FindElement(By.ClassName("wj-colheaders"));

            foreach (var row in rows)
            {
                if (!string.IsNullOrEmpty(row.GetAttribute("data-lp-id")) && !string.IsNullOrEmpty(row.GetAttribute("role")))
                {
                    //MscrmControls.Grid.ReadOnlyGrid|entity_control|account|00000000-0000-0000-00aa-000010001001|account|cc-grid|grid-cell-container
                    var datalpid = row.GetAttribute("data-lp-id").Split('|');
                    var cells = row.FindElements(By.ClassName("wj-cell"));
                    var currentindex = 0;
                    string link;
                    if (cells[currentindex + 1].GetAttribute("title") != "---")
                    {
                        link = cells[currentindex + 1].FindElement(By.TagName("a")).GetAttribute("href");
                    }
                    else
                        link =

                    link =
                       $"{new Uri(browser.Driver.Url).Scheme}://{new Uri(browser.Driver.Url).Authority}/main.aspx?etn={datalpid[2]}&pagetype=entityrecord&id=%7B{datalpid[3]}%7D";

                    var item = new GridItem
                    {
                        EntityName = datalpid[2],
                        Url = new Uri(link),

                    };
                    foreach (var column in columnGroup.FindElements(By.ClassName("wj-row")))
                    {
                        var rowHeaders = column.FindElements(By.TagName("div"))
                            .Where(c => !string.IsNullOrEmpty(c.GetAttribute("title")) && !string.IsNullOrEmpty(c.GetAttribute("id")));

                        foreach (var header in rowHeaders)
                        {
                            var id = header.GetAttribute("id");
                            var className = header.GetAttribute("class");
                            var cellData = cells[currentindex + 1].GetAttribute("title");

                            if (!string.IsNullOrEmpty(id)
                                && className.Contains("wj-cell")
                                && !string.IsNullOrEmpty(cellData)
                                && cells.Count > currentindex
                            )
                            {
                                item[id] = cellData.Replace("-", "");
                            }
                            currentindex++;
                        }
                        returnList.Add(item);
                    }
        

                }
            }
            return returnList;
        }
    }
}