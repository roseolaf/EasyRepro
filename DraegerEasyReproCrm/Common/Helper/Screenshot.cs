using Draeger.Dynamics365.Testautomation.ExtensionMethods;
using ImageMagick;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace Draeger.Dynamics365.Testautomation.Common.Helper
{
    class Screenshot
    {
        public string SaveScreenshot(WebClient webClient, TestContext testContext)
        {
            var driver = webClient.Browser.Driver;
            var footer = By.XPath(@"//div[@data-id = 'form-footer']");
            var elementWithScrollBar = driver.GetElementWithActiveScrollBar();
            int footerHeight = 0;
            IWebElement footerElement = null;

            if (driver.FindElements(footer).Any())
            {
                footerElement = driver.FindElement(footer);

                footerHeight = int.Parse(footerElement.GetAttribute("clientHeight"));
                driver.SetElementHidden(footerElement);
            }
            driver.HideScrollBar(elementWithScrollBar);
            driver.ScrollTopReset(elementWithScrollBar);

            using (var imagesCollection = new MagickImageCollection())
            {
                byte[] screenshot;
                while (driver.ScrollTopInfo(elementWithScrollBar).Item2 > 0)
                {
                    screenshot = driver.TakeScreenshot().AsByteArray;

                    MagickImage magickImage = new MagickImage(screenshot);

                    if (screenshot != null)
                    {
                        // %, scroll-count left, scroll-top, client-height, scroll-height
                        var (_, _, scrollTop, clientheight, _) = driver.ScrollTopInfo(elementWithScrollBar);

                        // first screen shot does not need to get cropped
                        if (scrollTop != 0)
                        {
                            var cropHeight = scrollTop % clientheight == 0
                                ? clientheight
                                : scrollTop % clientheight;
                            magickImage.Crop(magickImage.Width, cropHeight, Gravity.South);
                        }

                        imagesCollection.Add(magickImage);
                    }
                    driver.ScrollTop(elementWithScrollBar);
                    Thread.Sleep(250);
                }

                if (!imagesCollection.Any())
                {
                    screenshot = driver.TakeScreenshot().AsByteArray;
                    var magickImage = new MagickImage(screenshot);
                    imagesCollection.Add(magickImage);
                }


                if (footerElement != null)
                {
                    driver.SetElementVisible(footerElement);
                }

                screenshot = driver.TakeScreenshot().AsByteArray;
                MagickImage footerImage = new MagickImage(screenshot);
                var scrollBarInfoForFooter = driver.ScrollTopInfo(elementWithScrollBar);
                var scrollTopForFooter = scrollBarInfoForFooter.Item3;
                var clientheightForFooter = scrollBarInfoForFooter.Item4;
                var scrollheightForFooter = scrollBarInfoForFooter.Item5;
                var footerCrop = scrollheightForFooter - scrollTopForFooter - clientheightForFooter + footerHeight;
                footerImage.Crop(footerImage.Width, footerCrop, Gravity.South);
                imagesCollection.Add(footerImage);


                var overallImage = imagesCollection.AppendVertically();

                driver.ShowScrollBar(elementWithScrollBar);

                using (var image = Image.FromStream(new MemoryStream(overallImage.ToByteArray())))
                {
                    var name = Guid.NewGuid();
                    var path = $"{Directory.GetCurrentDirectory()}{name}.png";
                    image.Save(path, ImageFormat.Png);
                    testContext.AddResultFile(path);
                    return path;
                }
                //}
            }
        }
    }
}
