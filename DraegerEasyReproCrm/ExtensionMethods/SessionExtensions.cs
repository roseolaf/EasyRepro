using System;
using System.Collections.Generic;
using Draeger.Dynamics365.Testautomation.Common;
using Draeger.Dynamics365.Testautomation.DTO;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace Draeger.Dynamics365.Testautomation.ExtensionMethods
{
    public static class SessionExtensions
    {
        public static WindowsElement Element(this WindowsDriver<WindowsElement> session, By element)
        {
            try
            {
                var windowsElement = session.FindElement(element);
                session.SaveScreenshot(windowsElement);
                return windowsElement;
            }
            catch (Exception)
            {
                session.SaveScreenshot();
                throw new NotFoundException("No element (" + element + ") found");
            }
        }

        public static void SaveScreenshot(this WindowsDriver<WindowsElement> session)
        {
            try
            {
                var screenshot = session.GetScreenshot();
                UtilityHelper.SaveScreenshot(screenshot);
            }
            catch (Exception)
            {
                // Do nothing
            }
        }

        public static void SaveScreenshot(this WindowsDriver<WindowsElement> session, WindowsElement element)
        {
            try
            {
                var location = element.Location;
                var size = element.Size;
                var screenshot = session.GetScreenshot();

                UtilityHelper.Screenshot_HighlightElement(screenshot, location.X, location.Y, size.Width, size.Height);
            }
            catch (Exception)
            {
                // Do nothing
            }
        }

        public static WindowsElement Element(this WindowsDriver<WindowsElement> session, ByAttribute element)
        {
            WindowsElement windowsElement = null;

            foreach (var item in session.FindElements(element.by))
                if (item.GetAttribute(element.attributeName).Contains(element.attributeValue))
                {
                    windowsElement = item;
                    session.SaveScreenshot(windowsElement);
                    break;
                }

            if (windowsElement == null)
            {
                session.SaveScreenshot();
                throw new NotFoundException("No element (" + element.by + ") found");
            }

            return windowsElement;
        }

        public static List<WindowsElement> Elements(this WindowsDriver<WindowsElement> session, ByAttribute element)
        {
            var windowsElements = new List<WindowsElement>();

            foreach (var item in session.FindElements(element.by))
                if (item.GetAttribute(element.attributeName).Contains(element.attributeValue))
                    windowsElements.Add(item);

            if (windowsElements == null)
            {
                session.SaveScreenshot();
                throw new NotFoundException("No element (" + element.by + ") found");
            }

            return windowsElements;
        }

        public static T GetPage<T>(this WindowsDriver<WindowsElement> session)
        {
                return (T)Activator.CreateInstance(typeof(T), new object[] { session });
        }

    }
}