// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Microsoft.Dynamics365.UIAutomation.Api
{
    /// <summary>
    /// Xrm Report Page
    /// </summary>
    public class Processes
        : XrmPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mobile"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public Processes(InteractiveBrowser browser)
            : base(browser)
        {
            SwitchToContentFrame();
        }

        public enum ProcessType
        {
            Action,
            BusinessProcessFlow,
            Dialog,
            Workflow
        }
        public BrowserCommandResult<bool> CreateProcess(string name, ProcessType type, string entity, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Create Process"), driver =>
            {
                SwitchToDialogFrame();

                driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Process.Name]))
                      .SendKeysWait(name);

                SetValue(new OptionSet() { Name = Elements.ElementId[Reference.Process.Category], Value = type.ToString() });
                SetValue(new OptionSet() { Name = Elements.ElementId[Reference.Process.Entity], Value = entity });

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.BlankWorkflow])).ClickWait();

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.Create])).ClickWait();

                return true;
            });
        }

        public BrowserCommandResult<bool> Activate(int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Activate"), driver =>
            {
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.Activate])).ClickWait();

                SwitchToDialogFrame();

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.Begin])).ClickWait();

                return true;
            });
        }

        public BrowserCommandResult<bool> Deactivate(int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Activate"), driver =>
            {
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.Deactivate])).ClickWait();

                SwitchToDialogFrame();

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.Begin])).ClickWait();

                return true;
            });
        }
        public BrowserCommandResult<bool> Delete(int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("Activate"), driver =>
            {
                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.Delete])).ClickWait();

                SwitchToDialogFrame();

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Process.Begin])).ClickWait();

                return true;
            });
        }
    }
}
