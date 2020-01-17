using System;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using OpenQA.Selenium.Support.Extensions;
using Draeger.Dynamics365.Testautomation.Properties;
using System.Collections.Generic;

namespace Draeger.Dynamics365.Testautomation.ExtensionMethods
{
    public static class BrowserExtensions
    {
        /// <summary>
        /// Extension Methods on Interfaces will work
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="browser"></param>
        /// <returns></returns>
        public static T GetCustomPage<T>(this Browser browser) where T : XrmPage
        {
            if (browser is InteractiveBrowser interactiveBrowser)
            {
                return interactiveBrowser.GetPage<T>();
            }

            throw new NullReferenceException("");
        }

        public static T GetCustomPage<T>(this XrmApp xrmApp, params object[] args) where T : XrmApp
        {
            return (T)Activator.CreateInstance(typeof(T), xrmApp, args);
        }

        public static void ScrollIntoView(this IWebDriver driver, IWebElement webElement)
        {
            driver.ExecuteScript("arguments[0].scrollIntoView();", webElement);
        }

        public static void ScrollIntoViewIfNeeded(this IWebDriver driver, IWebElement webElement)
        {
            driver.ExecuteScript("arguments[0].scrollIntoViewIfNeeded();", webElement);
        }

        public static void ScrollLeft(this IWebDriver driver, IWebElement webElement)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var scrollLeft = int.Parse(js.ExecuteScript("return arguments[0].scrollLeft;", webElement).ToString());

            var clientWidth = int.Parse(js.ExecuteScript("return arguments[0].clientWidth;", webElement).ToString());

            int scrollWidth = int.Parse(js.ExecuteScript("return arguments[0].scrollWidth;", webElement).ToString());

            var visibleWidth = scrollLeft + clientWidth;

            if (visibleWidth <= scrollWidth)
            {
                //visibleWidth += clientWidth;
                js.ExecuteScript($"arguments[0].scrollLeft = {visibleWidth};", webElement);
            }
        }

        public static void ScrollTop(this IWebDriver driver, IWebElement webElement)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var scrollTop = int.Parse(js.ExecuteScript("return arguments[0].scrollTop;", webElement).ToString());

            var clientHeight = int.Parse(js.ExecuteScript("return arguments[0].clientHeight;", webElement).ToString());

            int scrollHeight = int.Parse(js.ExecuteScript("return arguments[0].scrollHeight;", webElement).ToString());

            var visibleHeight = scrollTop + clientHeight;

            if (visibleHeight <= scrollHeight)
            {
                //visibleHeight += clientHeight;
                js.ExecuteScript($"arguments[0].scrollTop = {visibleHeight};", webElement);
            }
        }

        public static void ScrollTopIndex(this IWebDriver driver, IWebElement webElement,int index)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            var clientHeight = int.Parse(js.ExecuteScript("return arguments[0].clientHeight;", webElement).ToString());

            int scrollHeight = int.Parse(js.ExecuteScript("return arguments[0].scrollHeight;", webElement).ToString());

            var visibleHeight = clientHeight*index;

            if (visibleHeight <= scrollHeight)
            {
                //visibleHeight += clientHeight;
                js.ExecuteScript($"arguments[0].scrollTop = {visibleHeight};", webElement);
            }
        }

        public static Tuple<double, double,int,int,int> ScrollTopInfo(this IWebDriver driver, IWebElement webElement)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var scrollTop = int.Parse(js.ExecuteScript("return arguments[0].scrollTop;", webElement).ToString());

            var clientHeight = int.Parse(js.ExecuteScript("return arguments[0].clientHeight;", webElement).ToString());

            int scrollHeight = int.Parse(js.ExecuteScript("return arguments[0].scrollHeight;", webElement).ToString());

            var visibleHeight = scrollTop + clientHeight;
            double scrollCount = ((double)scrollHeight - (double)visibleHeight) / (double)clientHeight;
            // %, scroll count left, scrolltop, clientheight, scrollheight
            return new Tuple<double, double, int, int, int>((visibleHeight / scrollHeight) * 100, Math.Ceiling(scrollCount), scrollTop, clientHeight, scrollHeight);
        }

        public static void ScrollTop(this IWebDriver driver, IWebElement webElement, int pixel)
        {
            driver.ExecuteScript($"arguments[0].scrollTop = {pixel};", webElement);
        }

        public static void ScrollLeft(this IWebDriver driver, IWebElement webElement, int pixel)
        {    
            driver.ExecuteScript($"arguments[0].scrollLeft = {pixel};", webElement);            
        }
        public static void ScrollTopReset(this IWebDriver driver, IWebElement webElement)
        {
            driver.ExecuteScript($"arguments[0].scrollTop = 0;", webElement);
        }

        public static void ScrollLeftReset(this IWebDriver driver, IWebElement webElement)
        {
            driver.ExecuteScript($"arguments[0].scrollLeft = 0;", webElement);
        }

        //public static void WaitForLoading(this IWebDriver driver)
        //{
        //    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        //    wait.Until(d => d.FindElements(By.XPath("//div[@class='progressDot']")).Where( elem => int.Parse(elem.GetAttribute("clientHeight")) > 0 ).Count() == 0);
        //}

        public static void SetElementHidden(this IWebDriver driver, IWebElement element)
        {
            driver.ExecuteJavaScript("arguments[0].hidden = true", element);
        }


        public static void SetElementVisible(this IWebDriver driver, IWebElement element)
        {
            driver.ExecuteJavaScript("arguments[0].hidden = false", element);
        }

        public static void HideScrollBar(this IWebDriver driver, IWebElement element)
        {
            driver.ExecuteJavaScript("arguments[0].style.overflow = \"hidden\"", element);
        }
        public static void ShowScrollBar(this IWebDriver driver, IWebElement element)
        {
            driver.ExecuteJavaScript("arguments[0].style.overflow = \"\"", element);
        }

        public static string GetElementAbsoluteXPath(this IWebDriver driver, IWebElement element)
        {
            return driver.ExecuteJavaScript<string>(Resources.GetElementAbsolutXPath, element);
        }

        public static List<IWebElement> GetAllElementsWithScrollbars(this IWebDriver driver)
        {
            IReadOnlyCollection<IWebElement> arr = new List<IWebElement>();
            try
            {
                arr = driver.ExecuteJavaScript<IReadOnlyCollection<IWebElement>>(Resources
                    .GetAllElementsWithScrollBars);
            }
            catch (Exception)
            {
                // nothing to do, elements with scrollbar not exists
            }

            return arr?.ToList();
        }

        public static IWebElement GetElementWithActiveScrollBar(this IWebDriver driver)
        {
            var allElementsWithScrollbar = driver.GetAllElementsWithScrollbars();
            allElementsWithScrollbar = allElementsWithScrollbar.Where(o => o.Displayed).ToList();
            if (allElementsWithScrollbar.Count == 0) return driver.GetDocumentScrollingElement();
            var element =
                driver.ExecuteJavaScript<IWebElement>(Resources.GetElementWithActiveScrollbar,
                    allElementsWithScrollbar);
            if (element == null || element.TagName.ToLower() == "body" ||
                element.TagName.ToLower() == "html") element = driver.GetDocumentScrollingElement();
            return element;
        }
        internal static IWebElement GetDocumentScrollingElement(this IWebDriver driver)
        {
            return driver.ExecuteJavaScript<IWebElement>("return document.scrollingElement");
        }
    }
}