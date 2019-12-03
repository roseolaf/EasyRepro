﻿//using Draeger.CrmConnector.CrmOnline;

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
using OpenQA.Selenium;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Screenshot = Draeger.Dynamics365.Testautomation.Common.Helper.Screenshot;
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

            //    CrmConnection.Instance.Dispose();
            CredentialsManager.Instance.Dispose();
            CrmConnection.Instance.Dispose();
            //CrmConnection adminConnection = new CrmConnection();
            //adminConnection.Dispose();
            //Environment.Exit(Environment.ExitCode);

        }

        [TestInitialize]
        public void TestInitialize()
        {
            CrmConnection ac = new CrmConnection();
            var context = ac.GetContext();
            var solutionsHistory = context.CreateQuery("msdyn_solutionhistory");

            TestContext.Properties.Add("SolutionVersion", solutionsHistory.First()["msdyn_solutionversion"].ToString());

            LoggerSinkList = new List<ListSinkInfo>();
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.With(new QaLogEnricher(TestContext))
                .WriteTo.File(new JsonFormatter(renderMessage: true),
                    Path.Combine(Directory.GetCurrentDirectory(), "/logs/log.json"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel:LogEventLevel.Verbose,
                    retainedFileCountLimit: null)
                .WriteTo.MSTestOutput(TestContext, LogEventLevel.Debug)
                .WriteTo.ListOutput(LoggerSinkList,TestContext);
            Logger = new LoggerWrapper(loggerConfig.CreateLogger());

            Logger.Debug("Test Init");

            Users = CredentialsManager.Instance.GetCredentials(this,
                TestContext.TestName, Logger);

            Logger.Debug("Test Users");
            XrmBrowser = new WebClient(TestSettings.Options);

            Logger.Debug("Test XrmBrowser");
            XrmApp = new XrmApp(XrmBrowser);

            Logger.Debug("Test XrmApp");
            TestContext.Properties.Add("WebClient", XrmBrowser);

            Logger.Debug("Test WebClient");
#if DEBUG
            var monitor = Screen.FromPoint(new Point(Screen.PrimaryScreen.Bounds.Right + 1,
                Screen.PrimaryScreen.Bounds.Top));
            XrmBrowser.Browser.Driver.Manage().Window.Position = new Point(monitor.Bounds.X, monitor.Bounds.Y);
            XrmBrowser.Browser.Driver.Manage().Window.Maximize();
#endif
            XrmBrowser.Browser.Driver.Navigate().GoToUrl(XrmUri);


            Logger.Debug("Test Init Complete");

        }


        [TestCleanup]
        public void TestCleanUp()
        {
            Logger.Debug("{Call} {Driver} {XrmApp} {CredManager} {CrmConnection}", "Cleanup Start", XrmBrowser.Browser.Driver != null, XrmApp != null, CrmConnection.Instance != null, CredentialsManager.Instance != null);
            foreach (var kvp in Users)
            {
                Logger.Debug($"Return Credentials for user {kvp.Value.Username.ToUnsecureString()}");
                kvp.Value.Return(Logger);
            }
            Logger.Debug("{Call} {Driver} {XrmApp} {CredManager} {CrmConnection}", "User Dispose", XrmBrowser.Browser.Driver != null, XrmApp != null, CrmConnection.Instance != null, CredentialsManager.Instance != null);

            try
            {

                Console.WriteLine("Test Cleanup");
                var uri = new Uri(XrmBrowser.Browser.Driver.Url);
                var scheme = uri.Scheme;
                var authority = uri.Authority;
                var qs = HttpUtility.ParseQueryString(uri.Query.ToLower());
                var appId = qs.Get("appid");
                TestContext.Properties.Add("Scheme", scheme);
                TestContext.Properties.Add("Authority", authority);
                TestContext.Properties.Add("AppId", appId);


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
            }
            catch (WebDriverException e)
            {
                Logger.Debug(e, "WebDriverException during Bug creation");
            }
            catch (Exception e)
            {
                Logger.Debug(e, "Unexpected Exception during Bug creation");
            }


            //XrmBrowser.Browser.Driver?.Close();
            //XrmBrowser.Browser.Driver.Quit();
            //XrmBrowser.Browser.Driver?.Dispose();
            try
            {
                XrmApp.Dispose();
                XrmApp = null;
            }
            catch (WebDriverException e)
            {
                Logger.Debug(e, "WebDriverException during dispose");
            }
            //CredentialsManager.Instance.Dispose();
            //CrmConnection.Instance.Dispose();
            Logger.Debug("{Call} {Driver} {XrmApp} {CredManager} {CrmConnection}", "End",XrmBrowser.Browser.Driver != null, XrmApp != null, CrmConnection.Instance != null, CredentialsManager.Instance != null);
            Console.WriteLine("Test Cleanup complete");
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