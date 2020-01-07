using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.Extensions;

namespace TaADOLog.Logger
{
    class Screenshot
    {
        public string SaveScreenshot(WebClient webClient, TestContext testContext)
        {
            var driver = webClient.Browser.Driver;
            var screenshot = driver.TakeScreenshot().AsByteArray;

            using (var image = Image.FromStream(new MemoryStream(screenshot)))
            {
                var name = $"{DateTime.Now.ToString("yyyyddMMHHmmssFFF")}{Guid.NewGuid()}";
                var path = $"{Directory.GetCurrentDirectory()}{name}.png";
                image.Save(path, ImageFormat.Png);
                testContext.AddResultFile(path);
                return path;
            }
        }
    }
}
