using System;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public class TestSettings
    {
        public static string InvalidAccountLogicalName = "accounts";

        public static string LookupField = "primarycontactid";
        public static string LookupName = "Rene Valdes (sample)";
#if DEBUG
        private static readonly string Type = "Chrome";
#else
        private static readonly string Type = "Remote";
#endif

        public static BrowserOptions Options = new BrowserOptions
        {
            BrowserType = (BrowserType)Enum.Parse(typeof(BrowserType), Type),
            RemoteBrowserType = BrowserType.Chrome,
            RemoteHubServer = new Uri ("http://10.247.134.133:4444/wd/hub"),
            PrivateMode = false,
            CleanSession = true,
            FireEvents = false,
            Headless = false,
            UserAgent = false,
            DefaultThinkTime = 2000,
            UCITestMode = false,
            PageLoadTimeout = TimeSpan.FromMinutes(10),
            CommandTimeout = TimeSpan.FromMinutes(5),
            StartMaximized = true,
            
        };

        public static string GetRandomString(int minLen, int maxLen)
        {
            char[] Alphabet = ("ABCDEFGHIJKLMNOPQRSTUVWXYZabcefghijklmnopqrstuvwxyz0123456789").ToCharArray();
            Random m_randomInstance = new Random();
            Object m_randLock = new object();

            int alphabetLength = Alphabet.Length;
            int stringLength;
            lock (m_randLock) { stringLength = m_randomInstance.Next(minLen, maxLen); }
            char[] str = new char[stringLength];

            // max length of the randomizer array is 5
            int randomizerLength = (stringLength > 5) ? 5 : stringLength;

            int[] rndInts = new int[randomizerLength];
            int[] rndIncrements = new int[randomizerLength];

            // Prepare a "randomizing" array
            for (int i = 0; i < randomizerLength; i++)
            {
                int rnd = m_randomInstance.Next(alphabetLength);
                rndInts[i] = rnd;
                rndIncrements[i] = rnd;
            }

            // Generate "random" string out of the alphabet used
            for (int i = 0; i < stringLength; i++)
            {
                int indexRnd = i % randomizerLength;
                int indexAlphabet = rndInts[indexRnd] % alphabetLength;
                str[i] = Alphabet[indexAlphabet];

                // Each rndInt "cycles" characters from the array, 
                // so we have more or less random string as a result
                rndInts[indexRnd] += rndIncrements[indexRnd];
            }
            return (new string(str));
        }
    }

    public static class UCIAppName
    {
        public static string Sales = "Sales Hub";
        public static string CustomerService = "Customer Service Hub";
        public static string Project = "Project Resource Hub";
        public static string FieldService = "Field Resource Hub";
    }
}