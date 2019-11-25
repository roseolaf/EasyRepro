using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using ImageMagick;
using OpenQA.Selenium;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public static class UtilityHelper
    {
        public static List<string> ImagePathList = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gifName"></param>
        /// <param name="imageList"></param>
        public static void GenerateGif(string gifName)
        {
            //using (var collection = new MagickImageCollection())
            //{
            //    foreach (var image in ImagePathList)
            //    {
            //        //var convertedImage = new MagickImage(image);
            //        collection.Add(image);

            //        collection.Last().AnimationDelay = 100;
            //    }

            //    System.IO.Directory.CreateDirectory("Gifs");
            //    collection.Write("Gifs\\" + gifName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".gif");
            //}

            //ImagePathList.Clear();
        }

        public static void SaveScreenshot(Screenshot screenshot)
        {
            System.IO.Directory.CreateDirectory("Screenshots");
            string screenshotName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
            screenshot.SaveAsFile("Screenshots\\" + screenshotName, OpenQA.Selenium.ScreenshotImageFormat.Png);
            ImagePathList.Add("Screenshots\\" + screenshotName);
        }

        public static void Screenshot_HighlightElement(Screenshot screenshot, int x, int y, int width, int height)
        {
            screenshot.SaveAsFile(@"tmp.png", ScreenshotImageFormat.Png);
            Bitmap bitMapImage = new Bitmap(@"tmp.png");
            Graphics graphicImage = Graphics.FromImage(bitMapImage);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

            graphicImage.DrawRectangle(new Pen(Color.Red), x, y, width, height);
            System.IO.Directory.CreateDirectory("Screenshots");

            string screenshotName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
            bitMapImage.Save("Screenshots\\" + screenshotName, ImageFormat.Png);
            ImagePathList.Add("Screenshots\\" + screenshotName);
            bitMapImage.Dispose();
            graphicImage.Dispose();
        }
    }
}
