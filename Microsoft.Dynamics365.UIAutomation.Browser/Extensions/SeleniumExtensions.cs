// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web.Script.Serialization;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.Extensions;
using SeleniumExtras.WaitHelpers;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Microsoft.Dynamics365.UIAutomation.Browser
{
    public static class SeleniumExtensions
    {
        #region ClickWait



        public static IWebElement ClickWait(this IWebElement element, bool ignoreStaleElementException = true)
        {
            try
            {
                Thread.Sleep(200);
                element.WaitForElement();
                element.Click();
                element.WaitForLoading();
                element.WaitForSaving();
                Thread.Sleep(200);
            }
            catch (StaleElementReferenceException ex)
            {
                if (!ignoreStaleElementException)
                    throw ex;
            }

            return element;
        }

        public static void Hover(this IWebElement element, IWebDriver driver, bool ignoreStaleElementException = true)
        {
            try
            {
                element.WaitForElement();
                Actions action = new Actions(driver);
                action.MoveToElement(element).Build().Perform();
            }
            catch (StaleElementReferenceException)
            {
                if (!ignoreStaleElementException)
                    throw;
            }
        }

        #endregion ClickWait

        #region Double ClickWait

        public static void DoubleClick(this IWebDriver driver, IWebElement element, bool ignoreStaleElementException = false)
        {
            try
            {
                element.WaitForElement();
                Actions actions = new Actions(driver);
                actions.DoubleClick(element).Perform();
                driver.WaitForSaving();
                driver.WaitForLoading();
            }
            catch (StaleElementReferenceException ex)
            {
                if (!ignoreStaleElementException)
                    throw ex;
            }
        }

        public static void DoubleClick(this IWebDriver driver, By by, bool ignoreStaleElementException = false)
        {
            try
            {
                var element = driver.WaitForElement(by);
                driver.DoubleClick(element, ignoreStaleElementException);
                driver.WaitForSaving();
                driver.WaitForLoading();
            }
            catch (StaleElementReferenceException ex)
            {
                if (!ignoreStaleElementException)
                    throw ex;
            }
        }

        #endregion

        #region Script Execution

        [DebuggerNonUserCode()]
        public static object ExecuteScript(this IWebDriver driver, string script, params object[] args)
        {
            var scriptExecutor = (driver as IJavaScriptExecutor);

            if (scriptExecutor == null)
                throw new InvalidOperationException(
                    $"The driver type '{driver.GetType().FullName}' does not support Javascript execution.");

            return scriptExecutor.ExecuteScript(script, args);
        }

        [DebuggerNonUserCode()]
        public static JObject GetJsonObject(this IWebDriver driver, string @object)
        {
            @object = SanitizeReturnStatement(@object);

            var results = ExecuteScript(driver, $"return JSON.stringify({@object});").ToString();

            return JObject.Parse(results);
        }

        [DebuggerNonUserCode()]
        public static JArray GetJsonArray(this IWebDriver driver, string @object)
        {
            @object = SanitizeReturnStatement(@object);

            var results = ExecuteScript(driver, $"return JSON.stringify({@object});").ToString();

            return JArray.Parse(results);
        }

        [DebuggerNonUserCode()]
        public static T GetJsonObject<T>(this IWebDriver driver, string @object)
        {
            @object = SanitizeReturnStatement(@object);

            var results = ExecuteScript(driver, $"return JSON.stringify({@object});").ToString();
            var jsSerializer = new JavaScriptSerializer();

            jsSerializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            var jsonObj = new JavaScriptSerializer().Deserialize<T>(results);

            return jsonObj;
        }

        private static string SanitizeReturnStatement(string script)
        {
            if (script.EndsWith(";"))
            {
                script = script.TrimEnd(script[script.Length - 1]);
            }

            if (script.StartsWith("return "))
            {
                script = script.TrimStart("return ".ToCharArray());
            }

            return script;
        }

        #endregion Script Execution

        #region Browser Options

        [DebuggerNonUserCode()]
        public static void ResetZoom(this IWebDriver driver)
        {
            IWebElement element = driver.FindElement(By.TagName("body"));
            element.SendKeysWait(Keys.Control + "0");
        }

        #endregion Browser Options

        #region Screenshot

        [DebuggerNonUserCode()]
        public static Screenshot TakeScreenshot(this IWebDriver driver)
        {
            var screenshotDriver = (driver as ITakesScreenshot);

            if (screenshotDriver == null)
                throw new InvalidOperationException(
                    $"The driver type '{driver.GetType().FullName}' does not support taking screenshots.");

            return screenshotDriver.GetScreenshot();
        }

        [DebuggerNonUserCode()]
        public static Bitmap TakeScreenshot(this IWebDriver driver, By by)
        {
            var screenshot = TakeScreenshot(driver);
            var bmpScreen = new Bitmap(new MemoryStream(screenshot.AsByteArray));

            // Measure the location of a specific element
            IWebElement element = driver.FindElement(by);
            var crop = new Rectangle(element.Location, element.Size);

            return bmpScreen.Clone(crop, bmpScreen.PixelFormat);
        }

        #endregion Screenshot

        #region Elements

        public static T GetAttribute<T>(this IWebElement element, string attributeName)
        {
            string value = element.GetAttribute(attributeName) ?? string.Empty;

            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }

        public static string GetAuthority(this IWebDriver driver)
        {
            string url = driver.Url;                // get the current URL (full)
            Uri currentUri = new Uri(url);          // create a Uri instance of it
            string baseUrl = currentUri.Authority;  // just get the "base" bit of the URL

            return baseUrl;
        }

        public static string GetBodyText(this IWebDriver driver)
        {
            return driver.FindElement(By.TagName("body")).Text;
        }

        public static bool HasAttribute(this IWebElement element, string attributeName)
        {
            return element.GetAttribute(attributeName) != null;
        }


        public static bool ElementExists(this IWebElement element, By by)
        {
            try
            {
                element.WaitUntilElement(e => e.Enabled,TimeSpan.FromSeconds(1));
                return element.FindElements(by).Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool TryFindElement(this IWebDriver driver, By by, out IWebElement element)
        {
            try
            {
                element = driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                element = null;
                return false;
            }
        }

        public static bool TryFindElement(this IWebElement element, By by, out IWebElement foundElement)
        {
            try
            {
                foundElement = element.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                foundElement = null;
                return false;
            }
        }

        public static bool IsVisible(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool IsVisible(this IWebElement element, By by)
        {
            try
            {
                return element.FindElement(by).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static void SetVisible(this IWebDriver driver, By by, bool visible)
        {
            IWebElement element = driver.FindElement(by);
            if (visible)
                driver.ExecuteScript($"document.getElementById('{element.GetAttribute("Id")}').setAttribute('style', 'display: inline;')");
            else
                driver.ExecuteScript($"document.getElementById('{element.GetAttribute("Id")}').setAttribute('style', 'display: none;')");
        }

        public static IWebElement SendKeysWait(this IWebElement element, string value, bool clear = false)
        {
            if (clear)
            {
                Thread.Sleep(200);
                element.WaitForElement();
                element.Clear();
                Thread.Sleep(200);
                element.SendKeys(Keys.Control+"a");
                Thread.Sleep(50);
                element.SendKeys(Keys.Backspace);
                element.SendKeys(value);
                Thread.Sleep(200);
            }
            else
            {

                Thread.Sleep(200);
                element.WaitForElement();
                element.SendKeys(value);
                Thread.Sleep(200);
            }

            return element;
        }

        public static bool AlertIsPresent(this IWebDriver driver)
        {
            return AlertIsPresent(driver, new TimeSpan(0, 0, 2));
        }

        public static bool AlertIsPresent(this IWebDriver driver, TimeSpan timeout)
        {
            var returnvalue = false;

            WebDriverWait wait = new WebDriverWait(driver, timeout);

            try
            {
                wait.Until(ExpectedConditions.AlertIsPresent());

                returnvalue = true;
            }
            catch (NoSuchElementException)
            {
                returnvalue = false;
            }
            catch (WebDriverTimeoutException)
            {
                returnvalue = false;
            }

            return returnvalue;

        }

        public static IWebDriver LastWindow(this IWebDriver driver)
        {
            return driver.SwitchTo().Window(driver.WindowHandles.Last());
        }

        /// <summary>Clears the focus from all elements.</summary>
        /// <param name="driver">The driver.</param>
        public static void ClearFocus(this IWebDriver driver)
        {
            driver.FindElement(By.TagName("body")).ClickWait();
        }

        #endregion Elements

        #region Waits



   

        public static void WaitForLoading(this IWebDriver driver)
        {
            Thread.Sleep(100);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            wait.Until(d => !d.FindElements(By.XPath("//div[@class='progressDot']")).Any(elem =>  int.Parse(elem.GetAttribute("clientHeight")) > 0));
        }

        public static void WaitForSaving(this IWebDriver driver)
        {
            Thread.Sleep(100);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            wait.Until(d => !d.FindElements(By.XPath("//span[starts-with(text(),'Saving')]")).Any());
        }

        public static void WaitForElement(this IWebElement element)
        {
            element.WaitUntilElement(e => e.Displayed && e.Enabled, TimeSpan.FromSeconds(60));
            Thread.Sleep(50);
        }

        public static void WaitForLoading(this IWebElement element)
        {
            Thread.Sleep(100);
            element.WaitUntilElement(e => !e.FindElements(By.XPath("//div[@class='progressDot']")).Any(elem => int.Parse(elem.GetAttribute("clientHeight")) > 0), TimeSpan.FromSeconds(180));
        }

        public static void WaitForSaving(this IWebElement element)
        {
            Thread.Sleep(100);
            element.WaitUntilElement(e => !e.FindElements(By.XPath("//span[starts-with(text(),'Saving')]")).Any(), TimeSpan.FromSeconds(180));
        }

        public static void _ScrollIntoViewIfNeeded(this IWebDriver driver, IWebElement webElement)
        {
            driver.ExecuteScript("arguments[0].scrollIntoViewIfNeeded();", webElement);
        }
        public static void WaitUntilElement(this IWebElement element, Func<IWebElement, bool> condition, TimeSpan time)
        {
            DateTime startTime = DateTime.Now;
            while (true)
            {
                try
                {
                    var driver = ((IWrapsDriver) element).WrappedDriver;
                    driver._ScrollIntoViewIfNeeded(element);
                    if (condition.Invoke(element))
                        return;
                }
                catch (StaleElementReferenceException)
                {
                    // Element disappeared
                    return;
                }
                catch (Exception)
                {
                }
                if (DateTime.Now > startTime.Add(time))
                {
                    string exceptionMessage = $"Timed out after {time} for {element.Text} and condition {condition.Method.Name}";
                    throw new ElementNotInteractableException(exceptionMessage);
                }
                Thread.Sleep(100);
            }
        }


        public static bool WaitForNotExists(this IWebElement element, TimeSpan time)
        {
            DateTime startTime = DateTime.Now;
            while (true)
            {
                try
                {
                    var elementEnabled = element.Displayed;
                    if (!elementEnabled)
                        return true;
                }
                catch (StaleElementReferenceException)
                {
                    // Element disappeared
                    return true;
                }
                catch (Exception)
                {
                }
                if (DateTime.Now > startTime.Add(time))
                {
                    string exceptionMessage = $"Timed out after {time} for {element.Text}";
                    throw new InvalidElementStateException(exceptionMessage);
                }
                Thread.Sleep(100);
            }
        }


        public static bool WaitFor(this IWebDriver driver, Predicate<IWebDriver> predicate)
        {
            return WaitFor(driver, predicate, Constants.DefaultTimeout);
        }

        public static bool WaitFor(this IWebDriver driver, Predicate<IWebDriver> predicate, TimeSpan timeout)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout);

            var result = wait.Until(d => predicate(d));

            return result;
        }

        public static bool WaitForPageToLoad(this IWebDriver driver)
        {
            return WaitForPageToLoad(driver, Constants.DefaultTimeout.Seconds);
        }

        public static bool WaitForTransaction(this IWebDriver driver)
        {
            return WaitForTransaction(driver, Constants.DefaultTimeout.Seconds);
        }

        //public static bool WaitForPageToLoad(this IWebDriver driver, TimeSpan timeout)
        //{
        //    object readyState = WaitForScript(driver, "if (document.readyState) return document.readyState;", timeout);

        //    if (readyState != null)
        //        return readyState.ToString().ToLower() == "complete";

        //    return false;
        //}

        public static bool WaitForPageToLoad(this IWebDriver driver, int maxWaitTimeInSeconds)
        {
            string state = string.Empty;
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));

                //Checks every 500 ms whether predicate returns true if returns exit otherwise keep trying till it returns ture
                wait.Until(d =>
                {

                    try
                    {
                        state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        //Ignore
                    }
                    catch (NoSuchWindowException)
                    {
                        //when popup is closed, switch to last windows
                        driver.SwitchTo().Window(driver.WindowHandles.Last(driver));
                    }
                    //In IE7 there are chances we may get state as loaded instead of complete
                    return (state.Equals("complete", StringComparison.InvariantCultureIgnoreCase));

                });
            }
            catch (TimeoutException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                    throw;
            }
            catch (NullReferenceException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                    throw;
            }
            catch (WebDriverException)
            {
                if (driver.WindowHandles.Count == 1)
                {
                    driver.SwitchTo().Window(driver.WindowHandles[0]);
                }
                state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();
                if (!(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase)))
                    throw;
            }
            return true;
        }

        public static bool WaitForTransaction(this IWebDriver driver, int maxWaitTimeInSeconds)
        {
            bool state = false;
            try
            {
                //Poll every half second to see if UCI is idle
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(500));
                wait.Until(d =>
                {
                    try
                    {
                        //Check to see if UCI is idle
                        state = (bool)driver.ExecuteScript("return window.UCWorkBlockTracker.isAppIdle()", "");
                    }
                    catch (TimeoutException)
                    {

                    }
                    catch (NullReferenceException)
                    {

                    }

                    return state;
                });
            }
            catch (Exception)
            {

            }

            return state;
        }
        public static string Last(this System.Collections.ObjectModel.ReadOnlyCollection<string> handles, IWebDriver driver)
        {
            return handles[handles.Count - 1];
        }
        public static object WaitForScript(this IWebDriver driver, string script)
        {
            return WaitForScript(driver, script, Constants.DefaultTimeout);
        }

        public static object WaitForScript(this IWebDriver driver, string script, TimeSpan timeout)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout);

            wait.Until((d) =>
            {
                try
                {
                    object returnValue = ExecuteScript(driver, script);

                    return returnValue;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                catch (WebDriverException)
                {
                    return null;
                }
            });

            return null;
        }

        //public static IWebElement WaitForElement(this IWebDriver driver, By by)
        //{
        //    return WaitForElement(driver, by, Constants.DefaultTimeout, null, null);
        //}

        //public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout)
        //{
        //    return WaitForElement(driver, by, timeout, null, null);
        //}

        //public static IWebElement WaitForElement(this IWebDriver driver, By by, string exceptionMessage)
        //{
        //    return WaitForElement(driver, by, Constants.DefaultTimeout, null, d =>
        //    {
        //        throw new InvalidOperationException(exceptionMessage);
        //    });
        //}

        //public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, string exceptionMessage)
        //{
        //    return WaitForElement(driver, by, timeout, null, d =>
        //    {
        //        throw new InvalidOperationException(exceptionMessage);
        //    });
        //}

        //public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback)
        //{
        //    return WaitForElement(driver, by, timeout, successCallback, null);
        //}

        //public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback, Action<IWebDriver> failureCallback)
        //{
        //    WebDriverWait wait = new WebDriverWait(driver, timeout);
        //    bool? success;
        //    IWebElement returnElement = null;

        //    wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        //    try
        //    {
        //        returnElement = wait.Until(d =>
        //        {
        //            ExpectedConditions.ElementExists(by);
        //            ExpectedConditions.ElementToBeClickable(by);
        //            var element = d.FindElement(by);
        //            //driver.ExecuteScript("arguments[0].scrollIntoViewIfNeeded();", element);
        //            //driver.IsElementInteractable(element);
        //            return element;
        //        });


        //        success = true;
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        success = false;
        //    }
        //    catch (WebDriverTimeoutException)
        //    {
        //        success = false;
        //    }

        //    if (success.HasValue && success.Value && successCallback != null)
        //        successCallback(driver);
        //    else if (success.HasValue && !success.Value && failureCallback != null)
        //        failureCallback(driver);

        //    return returnElement;
        //}



        public static IWebElement WaitForElement(this IWebDriver driver, By by)
        {
            return WaitForElement(driver, by, Constants.DefaultTimeout, null, null);
        }

        public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout)
        {
            return WaitForElement(driver, by, timeout, null, null);
        }

        public static IWebElement WaitForElement(this IWebDriver driver, By by, string exceptionMessage)
        {
            return WaitForElement(driver, by, Constants.DefaultTimeout, null, d =>
            {
                throw new InvalidOperationException(exceptionMessage);
            });
        }

        public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, string exceptionMessage)
        {
            return WaitForElement(driver, by, timeout, null, d =>
            {
                throw new InvalidOperationException(exceptionMessage);
            });
        }

        public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback)
        {
            return WaitForElement(driver, by, timeout, successCallback, null);
        }

        public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback, Action<IWebDriver> failureCallback)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout);
            bool? success;
            IWebElement returnElement = null;

            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

            try
            {
                returnElement = wait.Until(d => d.FindElement(by));

                success = true;
            }
            catch (NoSuchElementException)
            {
                success = false;
            }
            catch (WebDriverTimeoutException)
            {
                success = false;
            }

            if (success.HasValue && success.Value && successCallback != null)
                successCallback(driver);
            else if (success.HasValue && !success.Value && failureCallback != null)
                failureCallback(driver);

            return returnElement;
        }

        public static ReadOnlyCollection<IWebElement> WaitForElements(this IWebDriver driver, By by)
        {
            return WaitForElements(driver, by, Constants.DefaultTimeout);
        }

        public static ReadOnlyCollection<IWebElement> WaitForElements(this IWebDriver driver, By by, TimeSpan time)
        {
            DateTime startTime = DateTime.Now;
            while (true)
            {
                var elements = driver.FindElements(by);
                if (elements.Any())
                    return elements;
                

                if (DateTime.Now > startTime.Add(time))
                {
                    string exceptionMessage = $"Timed out after {time} for {by}";
                    throw new TimeoutException(exceptionMessage);
                }
                Thread.Sleep(100);
            }
        }


        //public static bool WaitForElement(this IWebDriver driver, By by)
        //{
        //    return WaitForElement(driver, by, Constants.DefaultTimeout, null, null);
        //}

        //public static bool WaitForElement(this IWebDriver driver, By by, TimeSpan timeout)
        //{
        //    return WaitForElement(driver, by, timeout, null, null);
        //}

        //public static bool WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback)
        //{
        //    return WaitForElement(driver, by, timeout, successCallback, null);
        //}

        //public static bool WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback, Action<IWebDriver> failureCallback)
        //{
        //    WebDriverWait wait = new WebDriverWait(driver, timeout);
        //    bool? success;

        //    wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        //    try
        //    {
        //        wait.Until(ExpectedConditions.ElementIsVisible(by));

        //        success = true;
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        success = false;
        //    }
        //    catch (WebDriverTimeoutException)
        //    {
        //        success = false;
        //    }

        //    if (success.HasValue && success.Value && successCallback != null)
        //        successCallback(driver);
        //    else if (success.HasValue && !success.Value && failureCallback != null)
        //        failureCallback(driver);

        //    return success.Value;
        //}

        public static bool ElementExists(this IWebDriver driver, By by)
        {
            return ElementExists(driver, by, TimeSpan.FromSeconds(1), null, null);
        }

        public static bool ElementExists(this IWebDriver driver, By by, TimeSpan timeout)
        {
            return ElementExists(driver, by, timeout, null, null);
        }

        public static bool ElementExists(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback)
        {
            return ElementExists(driver, by, timeout, successCallback, null);
        }
        public static bool ElementExists(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback, Action<IWebDriver> failureCallback)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout);
            bool? success;

            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

            try
            {
                wait.Until(ExpectedConditions.ElementExists(by));

                success = true;
            }
            catch (NoSuchElementException)
            {
                success = false;
            }
            catch (WebDriverTimeoutException)
            {
                success = false;
            }

            if (success.HasValue && success.Value && successCallback != null)
                successCallback(driver);
            else if (success.HasValue && !success.Value && failureCallback != null)
                failureCallback(driver);

            return success.Value;
        }

        public static bool ElementNotExists(this IWebDriver driver, By by)
        {
            return ElementNotExists(driver, by, TimeSpan.FromSeconds(1), null, null);
        }

        public static bool ElementNotExists(this IWebDriver driver, By by, TimeSpan timeout)
        {
            return ElementNotExists(driver, by, timeout, null, null);
        }

        public static bool ElementNotExists(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback)
        {
            return ElementNotExists(driver, by, timeout, successCallback, null);
        }
        public static bool ElementNotExists(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback, Action<IWebDriver> failureCallback)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout);
            bool? success;

            try
            {
                wait.Until(d => !d.FindElements(by).Any());

                success = true;
            }
            catch (WebDriverTimeoutException)
            {
                success = false;
            }

            if (success.Value && successCallback != null)
                successCallback(driver);
            else if (!success.Value && failureCallback != null)
                failureCallback(driver);

            return success.Value;
        }

        //public static bool WaitForElement(this IWebDriver driver, By by)
        //{
        //    return WaitForElement(driver, by, Constants.DefaultTimeout, null, null);
        //}

        //public static bool WaitForElement(this IWebDriver driver, By by, TimeSpan timeout)
        //{
        //    return WaitForElement(driver, by, timeout, null, null);
        //}

        //public static bool WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback)
        //{
        //    return WaitForElement(driver, by, timeout, successCallback, null);
        //}

        //public static bool WaitForElement(this IWebDriver driver, By by, TimeSpan timeout, Action<IWebDriver> successCallback, Action<IWebDriver> failureCallback)
        //{
        //    WebDriverWait wait = new WebDriverWait(driver, timeout);
        //    bool? success;


        //    wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        //    try
        //    {
        //        wait.Until(d => ExpectedConditions.ElementToBeClickable(by)); //!= null && d.IsElementInteractable(d.FindElement(by)));
        //        Thread.Sleep(250);

        //        success = true;
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        success = false;
        //    }
        //    catch (WebDriverTimeoutException)
        //    {
        //        success = false;
        //    }

        //    if (success.HasValue && success.Value && successCallback != null)
        //        successCallback(driver);
        //    else if (success.HasValue && !success.Value && failureCallback != null)
        //        failureCallback(driver);

        //    return success.Value;
        //}

        #endregion Waits

        #region Args / Tracing

        public static string ToTraceString(this FindElementEventArgs e)
        {
            try
            {
                if (e.Element != null)
                {
                    return string.Format("{4} - [{0},{1}] - <{2}>{3}</{2}>", e.Element.Location.X, e.Element.Location.Y, e.Element.TagName, e.Element.Text, e.FindMethod);
                }
                else
                {
                    return e.FindMethod.ToString();
                }
            }
            catch (Exception)
            {
                return e.FindMethod.ToString();
            }
        }

        #endregion Args / Tracing
    }

}