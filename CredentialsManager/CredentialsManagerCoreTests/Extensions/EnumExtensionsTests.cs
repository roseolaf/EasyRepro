using Microsoft.VisualStudio.TestTools.UnitTesting;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Draeger.Testautomation.CredentialsManagerCore.Extensions.Tests
{
    [TestClass()]
    public class EnumExtensionsTests
    {
        private enum Test
        {
            Default,
            NoAttribute,
            [EnumMember(Value = "test")]
            WithAttribute
        }

        [TestMethod()]
        public void ToEnumTest()
        {
            Assert.AreEqual(Test.WithAttribute, "test".ToEnum<Test>());
        }

        [TestMethod()]
        public void ToEnumTest2()
        {
            Assert.AreEqual(Test.Default, "test2".ToEnum<Test>());
        }

        [TestMethod()]
        public void ToEnumStringTest()
        {
            Assert.AreEqual("test", Test.WithAttribute.ToEnumString());
        }

        [TestMethod()]
        public void ToEnumStringTest2()
        {
            Assert.IsNull(Test.NoAttribute.ToEnumString());
        }
    }
}