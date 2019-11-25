using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using CredentialsManagerCoreTests.Mocks;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using Draeger.Testautomation.CredentialsManagerCore.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 12, Scope = ExecutionScope.MethodLevel)]
namespace CredentialsManagerCoreTests
{
    [TestClass]
    public class CredentialsManagerTests: TestBase
    {
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            CredentialsManager.Instance.Init(new XrmManagementHelperMock());
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, UserGroup = UserGroup.Sales)]
        public void GetCredentialsTest1()
        {
           Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
           Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
           Thread.Sleep(3000);
           Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!"); 
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, UserGroup = UserGroup.Sales)]
        public void GetCredentialsTest2()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, SecurityRole.AmDummyRole, UserGroup = UserGroup.Sales)]
        public void GetCredentialsTest3()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, SecurityRole.DraegerGlobalTestcardRole, UserGroup = UserGroup.Sales)]
        public void GetCredentialsTest4()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, UserGroup = UserGroup.Service)]
        public void GetCredentialsTest5()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, SecurityRole.DraegerMaintenanceWoContractRole, UserGroup = UserGroup.Service)]
        public void GetCredentialsTest6()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("admin", UserGroup = UserGroup.Admin)]
        public void GetCredentialsTest7()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["admin"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.DraegerLocalTestcardAdminRole, UserGroup = UserGroup.Service)]
        public void GetCredentialsTest8()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, UserGroup = UserGroup.Sales)]
        public void GetCredentialsTest9()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }

        [TestMethod()]
        [CrmUser("alias", SecurityRole.Base, SecurityRole.ActivityFeeds, UserGroup = UserGroup.Admin)]
        public void GetCredentialsTest10()
        {
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Got user {users["alias"].Username.ToUnsecureString()}");
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Starting long operation...");
            Thread.Sleep(3000);
            Console.WriteLine($"{DateTime.Now}: {TestContext.TestName} Done!");
        }
    }

    public class TestBase
    {
        public TestContext TestContext { get; set; }

        internal Dictionary<string, ITestUserCredentials> users = null;
        
        [TestInitialize]
        public void testInit()
        {
            if(users != null) Assert.Fail("Users were already initialized, no separate instances!");
            users = CredentialsManager.Instance.GetCredentials(this, TestContext.TestName);
        }
    }
}