using System.IO;
using Microsoft.Dynamics365.UIAutomation.Browser;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public static class TestOptionsManager
    {
        public static BrowserOptions GetDefaultOptions(BrowserType browserType = BrowserType.IE)
        {
            return new BrowserOptions
            {
                // Todo: Set Drivers Path
                DriversPath = Path.Combine(Directory.GetCurrentDirectory()),
                BrowserType = browserType,
                PrivateMode = false,
                FireEvents = false,
#if DEBUG
                Headless = false,
#else
                Headless = true,
#endif
                UserAgent = false,
                DefaultThinkTime = 2000,
            };
        }
    }
}