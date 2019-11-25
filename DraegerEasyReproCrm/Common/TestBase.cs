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
using System.Windows.Forms;
using Draeger.CrmConnector.CrmOnline;
using Draeger.Dynamics365.Testautomation.Attributes;
using Draeger.Dynamics365.Testautomation.Common.Helper;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;
using Infoman.Xrm.Services;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Sinks.ListOfString;
using WebClient = Microsoft.Dynamics365.UIAutomation.Api.UCI.WebClient;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]

namespace Draeger.Dynamics365.Testautomation.Common
{
    [TestClass]
    public abstract class TestBase
    {
        protected SecureString Username { get; private set; } = ConfigurationManager.AppSettings["OnlineUsername"].ToSecureString();
        protected SecureString Password { get; private set; } = ConfigurationManager.AppSettings["OnlinePassword"].ToSecureString();
        protected Uri XrmUri { get; } = new Uri(ConfigurationManager.AppSettings["OnlineCrmUrl"]);
        protected Dictionary<string, ManagedCredentials> users = null;
        public TestContext TestContext { get; set; }
        public LoggerWrapper logger;
        public List<string> loggerSinkList;
        public Exception exception;
        public WebClient XrmBrowser;
        public XrmApp xrmApp;



        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            CredentialsManager.Instance.Init(new XrmManagementHelper());
            ICredentials credentials = new NetworkCredential("tmp-QA-TA-001", "DraegerQA01");
            WebRequest.DefaultWebProxy.Credentials = credentials;
            
        }

        [TestInitialize]
        public void TestInitialize()
        {
            loggerSinkList = new List<string>();
            var loggerConfig = new LoggerConfiguration()
                .Enrich.With(new QaLogEnricher(TestContext))
                .WriteTo.File(new JsonFormatter(renderMessage: true),
                    Path.Combine(Directory.GetCurrentDirectory(), "/logs/log.json"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null)
                .WriteTo.MSTestOutput(TestContext, Serilog.Events.LogEventLevel.Information)
                .WriteTo.StringList(loggerSinkList, restrictedToMinimumLevel: LogEventLevel.Debug);
            logger = new LoggerWrapper(loggerConfig.CreateLogger());


            users = CredentialsManager.Instance.GetCredentials(this,
                TestContext.TestName, logger);

            XrmBrowser = new WebClient(TestSettings.Options);
            xrmApp = new XrmApp(XrmBrowser);
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
            logger.TestResult("{TestResult}", TestContext.CurrentTestOutcome);
#if DEBUG
            if (exception != null)
            {
                
                Console.WriteLine("Creating Bug");
                if (exception.Message.Contains("Assert"))
                    logger.Fail("{InnerException} - {@Exception}", exception.InnerException, exception);
                else
                    logger.Error("{InnerException} - {@Exception}", exception.InnerException, exception);

                WorkItems.CreateOrUpdateBug(int.Parse(TestContext.Properties["TestCaseId"].ToString()),
                     loggerSinkList,
                     exception,
                     (new Screenshot()).SaveScreenshot(XrmBrowser, TestContext));
            }
#else
            (new Screenshot()).SaveScreenshot(XrmBrowser, TestContext);
#endif

            foreach (var kvp in users)
            {
                kvp.Value.Dispose();
            }

            //XrmBrowser.Browser.Driver?.Close();
            XrmBrowser.Browser.Driver?.Quit();
            //XrmBrowser.Browser.Driver?.Dispose();
            //xrmApp?.Dispose();


        }

        protected void Login(XrmApp app, string userAlias)
        {
            try
            {
                //fallback for now
                var username = users[userAlias].Username;
                var password = users[userAlias].Password; 
                logger.Step("Logged in to {@XrmUri} as {@Username}", XrmUri, Marshal.PtrToStringUni(SecureStringMarshal.SecureStringToGlobalAllocUnicode(Username)));
                app.OnlineLogin.Login(XrmUri, username, password, XrmBrowser.ADFSLoginAction);

                 xrmApp.ThinkTime(2000);
            }
            catch (Exception e)
            {
                logger.Fatal("Login failed: {@exception}", e);
                throw;
            }
        }


        [Obsolete("This method is deprecated! Use CredentialsManager instead", false)]
        protected bool ResetSecurityRoles(string UserName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AdminConnection"]?.ConnectionString;
            var connectionProvider = new CrmConnectionProvider();
            using (var organizationServiceProxy = connectionProvider.ConnectWithCredentials(connectionString))
            {
                organizationServiceProxy.EnableProxyTypes();
                var securityRoles = GetBasicSecurityRoleList();

                var context = new ServiceContext(organizationServiceProxy, null);

                var userEntity = (from sysUser in context.SystemUserSet
                                  where sysUser.DomainName.Equals(UserName)
                                  select sysUser).FirstOrDefault();

                if (userEntity != null)
                {
                    var query = from user in context.SystemUserSet
                                join userRoles in context.SystemUserRolesSet on user.SystemUserId equals userRoles.SystemUserId
                                join role in context.RoleSet on userRoles.RoleId equals role.RoleId
                                where user.DomainName == UserName
                                select role;
                    //TODO: compare list of roles with list of basic roles
                    //TODO: remove superfluous roles

                }
            }
            return true;
        }


        [Obsolete("This method is deprecated! Use CredentialsManager instead!", false)]
        private object GetBasicSecurityRoleList()
        {
            throw new NotImplementedException();
        }

        //[Obsolete("This method is deprecated! Use credentialsManager instead!", false)]
        //protected bool SetSecurityRoles(string UserName)
        //{
        //    //TODO: needs refactoring? uses deprecated credentials currently
        //    var connectionString = ConfigurationManager.ConnectionStrings["AdminConnection"]?.ConnectionString;
        //    var connectionProvider = new CrmConnectionProvider();
        //    using (var organizationServiceProxy = connectionProvider.ConnectWithCredentials(connectionString))
        //    {
        //        organizationServiceProxy.EnableProxyTypes();
        //        var securityRoles = GetSecurityRoleList();

        //        ServiceContext context = new ServiceContext(organizationServiceProxy, null);

        //        var user = (from sysUser in context.SystemUserSet
        //                    where sysUser.DomainName.Equals(UserName)
        //                    select sysUser).FirstOrDefault();

        //        //XrmService context = new XrmService(organizationServiceProxy);
        //        foreach (var securityRole in securityRoles)
        //        {
        //            // see https://docs.microsoft.com/de-de/dynamics365/customer-engagement/developer/sample-associate-security-role-user for details
        //            QueryExpression query = new QueryExpression
        //            {
        //                EntityName = Role.EntityLogicalName,
        //                ColumnSet = new ColumnSet("roleid"),
        //                Criteria = new FilterExpression
        //                {
        //                    Conditions =
        //                    {
        //                        new ConditionExpression
        //                        {
        //                            AttributeName = "name",
        //                            Operator = ConditionOperator.Equal,
        //                            Values = {securityRole}
        //                        }
        //                    }
        //                }
        //            };

        //            EntityCollection roles = organizationServiceProxy.RetrieveMultiple(query);
        //            if (roles.Entities.Count > 0)
        //            {
        //                Role currentRole = organizationServiceProxy.RetrieveMultiple(query).Entities[0].ToEntity<Role>();
        //                Guid roleId = currentRole.Id;

        //                if (roleId != Guid.Empty && user.Id != Guid.Empty)
        //                {
        //                    organizationServiceProxy.Associate(
        //                                            "systemuser",
        //                                            user.Id,
        //                                            new Relationship("systemuserroles_association"),
        //                                            new EntityReferenceCollection() { new EntityReference(Role.EntityLogicalName, roleId) });
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}


        //protected bool OpenFirstOpportunity()
        //{
        //    var driver = XrmTestBrowser.Driver;

        //    // Wait for page loaded
        //    driver.WaitUntilClickable(By.Id("TabUserInfoId")); // wait for user info icon
        //    XrmTestBrowser.GuidedHelp.CloseGuidedHelp();
        //    XrmTestBrowser.ThinkTime(500);

        //    XrmTestBrowser.Navigation.OpenSubArea("Sales", "Opportunities");
        //    XrmTestBrowser.ThinkTime(200);

        //    XrmTestBrowser.Grid.OpenRecord(0);

        //    return true;
        //}

        //[Obsolete("This method is deprecated! Use credentialsManager instead!", false)]
        //private Dictionary<string, string> GetSecurityRoleList()
        //{
        //    StackTrace trace = new StackTrace();
        //    int index = 1;
        //    Dictionary<string, string> securityroles = new Dictionary<string, string>();

        //    do
        //    {
        //        // find calling testmethod
        //        var testmethod = trace.GetFrame(index).GetMethod().GetCustomAttributes(typeof(TestMethodAttribute), false);
        //        if (testmethod.Length > 0)
        //        {
        //            var securityRoleAttributes = trace.GetFrame(index).GetMethod().GetCustomAttributes(typeof(CrmUserRoleAttribute), false).Cast<CrmUserRoleAttribute>();
        //            foreach (var securityRoleAttribute in securityRoleAttributes)
        //            {
        //                securityroles.Add(securityRoleAttribute.UserId, securityRoleAttribute.SecurityRole.GetAttribute<DescriptionAttribute>().Description);
        //            }
        //            break;
        //        }

        //        index++;
        //    } while (index < 25);

        //    return securityroles;
        //}

        protected string GetAcceptanceCriteriaMessage(MethodBase method)
        {
            AcceptanceCriteriaAttribute attr = (AcceptanceCriteriaAttribute)method.GetCustomAttributes(typeof(AcceptanceCriteriaAttribute), true)[0];
            return $"[{attr.Sort}] {attr.WorkItem}: {attr.Description}";
        }

    }
}