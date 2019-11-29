//using Draeger.CrmConnector.CrmOnline;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Web;
using System.Windows.Forms;
using Draeger.Dynamics365.Testautomation.Attributes;
using Draeger.Dynamics365.Testautomation.Common.EntityManager;
using Draeger.Dynamics365.Testautomation.Common.Helper;
using Draeger.Testautomation.CredentialsManagerCore;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using WebClient = Microsoft.Dynamics365.UIAutomation.Api.UCI.WebClient;

[assembly: Parallelize(Workers = 100, Scope = ExecutionScope.MethodLevel)]

namespace Draeger.Dynamics365.Testautomation.Common
{
    [TestClass]
    public abstract class TestBase
    {
        protected SecureString Username { get; private set; } = ConfigurationManager.AppSettings["OnlineUsername"].ToSecureString();
        protected SecureString Password { get; private set; } = ConfigurationManager.AppSettings["OnlinePassword"].ToSecureString();
        protected Uri XrmUri { get; } = new Uri(ConfigurationManager.AppSettings["OnlineCrmUrl"]);
        protected Dictionary<string, ManagedCredentials> Users = null;
        public TestContext TestContext { get; set; }
        public LoggerWrapper Logger;
        public List<ListSinkInfo> LoggerSinkList;
        public Exception Exception;
        public WebClient XrmBrowser;
        public XrmApp XrmApp;


        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            CredentialsManager.Instance.Init(new XrmManagementHelper());
            ICredentials credentials = new NetworkCredential("tmp-QA-TA-001", "DraegerQA01");
            WebRequest.DefaultWebProxy.Credentials = credentials;
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            AdminConnection.Instance.Dispose();
            CredentialsManager.Instance.Dispose();
            //AdminConnection adminConnection = new AdminConnection();
            //adminConnection.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {

            AdminConnection ac = new AdminConnection();
            var context = ac.GetContext();
            var solutionsHistory = context.CreateQuery("msdyn_solutionhistory");

            TestContext.Properties.Add("SolutionVersion", solutionsHistory.First()["msdyn_solutionversion"].ToString());

            LoggerSinkList = new List<ListSinkInfo>();
            var loggerConfig = new LoggerConfiguration()
                .Enrich.With(new QaLogEnricher(TestContext))
                .WriteTo.File(new JsonFormatter(renderMessage: true),
                    Path.Combine(Directory.GetCurrentDirectory(), "/logs/log.json"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel:LogEventLevel.Debug,
                    retainedFileCountLimit: null)
                .WriteTo.MSTestOutput(TestContext, LogEventLevel.Debug)
                .WriteTo.ListOutput(LoggerSinkList,TestContext);
            Logger = new LoggerWrapper(loggerConfig.CreateLogger());


            Users = CredentialsManager.Instance.GetCredentials(this,
                TestContext.TestName, Logger);

            XrmBrowser = new WebClient(TestSettings.Options);
            XrmApp = new XrmApp(XrmBrowser);
            TestContext.Properties.Add("WebClient", XrmBrowser);
#if DEBUG
            var monitor = Screen.FromPoint(new Point(Screen.PrimaryScreen.Bounds.Right + 1,
                Screen.PrimaryScreen.Bounds.Top));
            XrmBrowser.Browser.Driver.Manage().Window.Position = new Point(monitor.Bounds.X, monitor.Bounds.Y);
            XrmBrowser.Browser.Driver.Manage().Window.Maximize();
#endif
            XrmBrowser.Browser.Driver.Navigate().GoToUrl(XrmUri);

        }


        [TestCleanup]
        public void TestCleanUp()
        {
            var uri = new Uri(XrmBrowser.Browser.Driver.Url);
            var scheme = uri.Scheme;
            var authority = uri.Authority;
            var qs = HttpUtility.ParseQueryString(uri.Query.ToLower());
            var appId = qs.Get("appid");
            TestContext.Properties.Add("Scheme",scheme);
            TestContext.Properties.Add("Authority",authority);
            TestContext.Properties.Add("AppId",appId);


            Logger.TestResult("{TestResult}", TestContext.CurrentTestOutcome);
#if !DEBUG
            if (Exception != null)
            {
                if (Exception.Message.Contains("Assert"))
                    Logger.Fail("{InnerException} - {@Exception}", Exception.InnerException, Exception);
                else
                    Logger.Error("{InnerException} - {@Exception}", Exception.InnerException, Exception);

                WorkItems.CreateOrUpdateBug(int.Parse(TestContext.Properties["TestCaseId"].ToString()),
                     LoggerSinkList,
                     Exception,
                     (new Screenshot()).SaveScreenshotFullPage(XrmBrowser, TestContext));
            }
#else
            (new Screenshot()).SaveScreenshotFullPage(XrmBrowser, TestContext);
#endif

            foreach (var kvp in Users)
            {
                kvp.Value.Dispose();
            }

            //XrmBrowser.Browser.Driver?.Close();
            //XrmBrowser.Browser.Driver?.Quit();
            //XrmBrowser.Browser.Driver?.Dispose();
            XrmApp.Dispose();


        }



        protected void Login(XrmApp app, string userAlias)
        {
            try
            {
                //fallback for now
                var username = Users[userAlias].Username;
                var password = Users[userAlias].Password; 
                Logger.Step("Logged in to {@XrmUri} as {@Username}", XrmUri, Marshal.PtrToStringUni(SecureStringMarshal.SecureStringToGlobalAllocUnicode(Username)));
                app.OnlineLogin.Login(XrmUri, username, password, XrmBrowser.ADFSLoginAction);

                 XrmApp.ThinkTime(2000);
            }
            catch (Exception e)
            {
                Logger.Fatal("Login failed: {@exception}", e);
                throw;
            }
        }


    
        protected string GetAcceptanceCriteriaMessage(MethodBase method)
        {
            AcceptanceCriteriaAttribute attr = (AcceptanceCriteriaAttribute)method.GetCustomAttributes(typeof(AcceptanceCriteriaAttribute), true)[0];
            return $"[{attr.Sort}] {attr.WorkItem}: {attr.Description}";
        }

    }
}