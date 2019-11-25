using System;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace Draeger.Dynamics365.Testautomation.ExtensionMethods
{
    public static class ElementExtension
    {
        public static void Operate(this WindowsElement element)
        {
            element.SendKeysWait(Keys.Return);
        }

        public static void ExpandTreeNavigation(this WindowsElement element)
        {
            element.SendKeysWait(Keys.Right);
        }

        public static void FocusAndOperate(this WindowsElement element)
        {
            element.SendKeysWait(Keys.Space);
            element.SendKeysWait(Keys.Return);
        }

        public static void SendText(this WindowsElement element, string text)
        {
            var now = DateTime.Now;
            element.SendKeysWait(text);
            while (!element.Text.Equals(text))
            {
                element.Clear();
                element.SendKeysWait(text);

                if (now.AddSeconds(60) < DateTime.Now)
                    throw new WebDriverTimeoutException("Could not type text (" + text + ") within 30 seconds");
            }
        }


        public static bool ElementExists(this WindowsDriver<WindowsElement> session, By element)
        {
            try
            {
                var windowsElement = session.FindElement(element);
                session.SaveScreenshot(windowsElement);
                return true;
            }
            catch (WebDriverException)
            {
                session.SaveScreenshot();
                return false;
            }
        }
    }
}