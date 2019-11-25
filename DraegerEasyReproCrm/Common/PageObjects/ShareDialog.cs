using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;

namespace Draeger.Dynamics365.Testautomation.Common.PageObjects
{
    public class ShareDialog : XrmPage
    {
        public ShareDialog(InteractiveBrowser browser)
            : base(browser)
        {
            Browser.Driver.SwitchTo().Frame("InlineDialog_Iframe");
        }

        //public BrowserCommandResult<bool> AddUser(string user, int thinkTime = Constants.DefaultThinkTime)
        //{
        //    Browser.ThinkTime(thinkTime);

        //    return this.Execute(GetOptions("Select"), driver =>
        //    {
        //        var xrmBrowser = Browser as Browser;
        //        Browser.Driver.ClickWhenAvailable(By.XPath("//*[@id=\"_TBAddUsersToShare\"]"));
        //        Browser.ThinkTime(2000);
        //        Browser.Driver.SwitchTo().DefaultContent();

        //        var multipleLookup = xrmBrowser.GetCustomPage<LookupMultiple>();
        //        multipleLookup.Index = 1;
        //        multipleLookup.Search(user);
        //        multipleLookup.SelectItem(0);
        //        multipleLookup.PressSelect();
        //        multipleLookup.Add();

        //        return true;
        //    });
        //}

        //public BrowserCommandResult<bool> ClickShare(int thinkTime = Constants.DefaultThinkTime)
        //{
        //    Browser.ThinkTime(thinkTime);

        //    return this.Execute(GetOptions("Select"), driver =>
        //    {
        //        driver.ClickWhenAvailable(By.Id("butBegin"));

        //        return true;
        //    });
        //}

        //public BrowserCommandResult<bool> ClickCancel(int thinkTime = Constants.DefaultThinkTime)
        //{
        //    Browser.ThinkTime(thinkTime);

        //    return this.Execute(GetOptions("Select"), driver =>
        //    {
        //        driver.ClickWhenAvailable(By.Id("cmdDialogCancel"));

        //        return true;
        //    });
        //}
    }
}