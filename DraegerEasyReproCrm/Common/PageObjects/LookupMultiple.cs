using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class LookupMultiple : XrmPage
    {
        //public int Index
        //{
        //    set => SwitchToDialog(value);
        //}

        public LookupMultiple(InteractiveBrowser browser)
            : base(browser)
        {
            //SwitchToDialog();
        }

        ///// <summary>
        ///// Searches based on searchCriteria in Lookup
        ///// </summary>
        ///// <param name="searchCriteria"></param>
        ///// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        //public BrowserCommandResult<bool> Search(string searchCriteria, int thinkTime = Constants.DefaultThinkTime)
        //{
        //    Browser.ThinkTime(thinkTime);

        //    return this.Execute(GetOptions("Search"), driver =>
        //    {
        //        driver.FindElement(By.XPath(Elements.Xpath[Reference.Grid.FindCriteria])).Clear();
        //        driver.FindElement(By.XPath(Elements.Xpath[Reference.Grid.FindCriteria])).SendKeysWait(searchCriteria);
        //        driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Grid.FindCriteriaImg]));

        //        return true;
        //    });
        //}

        ///// <summary>
        ///// Selects the Item
        ///// </summary>
        ///// <param name="index">The Index</param>
        ///// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        ///// <example>xrmBrowser.Lookup.SelectItem(0);</example>
        //public BrowserCommandResult<bool> SelectItem(int index, int thinkTime = Constants.DefaultThinkTime)
        //{
        //    Browser.ThinkTime(thinkTime);

        //    return this.Execute(GetOptions("Select Item"), driver =>
        //    {
        //        var itemsTable = driver.WaitForElement(By.XPath(Elements.Xpath[Reference.Grid.GridBodyTable]));
        //        var items = itemsTable.FindElements(By.TagName("tr"));

        //        var item = items[index + 1];
        //        var checkbox = item.FindElements(By.TagName("td"))[0];

        //        checkbox.Click();

        //        return true;
        //    });
        //}

        //public BrowserCommandResult<bool> PressSelect(int thinkTime = Constants.DefaultThinkTime)
        //{
        //    Browser.ThinkTime(thinkTime);

        //    return this.Execute(GetOptions("Press Select"), driver =>
        //    {
        //        driver.ClickWhenAvailable(By.Id("btnAdd"));

        //        return true;
        //    });
        //}

        ///// <summary>
        ///// Add Lookup
        ///// </summary>
        ///// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        ///// <example>xrmBrowser.Lookup.Add();</example>
        //public BrowserCommandResult<bool> Add(int thinkTime = Constants.DefaultThinkTime)
        //{
        //    Browser.ThinkTime(thinkTime);

        //    return this.Execute(GetOptions("Add"), driver =>
        //    {
        //        driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.LookUp.Begin]));

        //        return true;
        //    });
        //}
    }
}