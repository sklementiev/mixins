using System;
using System.Linq;
using Mixins.Tests.Data;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class Mixin
    {
        [Test]
        public void CanSetArbitratyProperty()
        {
            var banana = new Product();
            banana.SetProperty("The place where it grows", "Africa");
            Assert.AreEqual("Africa", banana.GetProperty("The place where it grows"));
        }

        [Test]
        public void PropertyNameCannotStartWithHash()
        {
            var banana = new Product();
            Assert.Throws<Exception>(() => banana.SetProperty("#don't do it", "never"));
        }

        [Test]
        public void PropertyNameIsTrimmed()
        {
            var banana = new Product();
            banana.SetProperty("  My property     ", "test");
            Assert.AreEqual("test", banana.GetProperty(" My property "));
            Assert.AreEqual(banana.GetProperty("     My property  "), banana.GetProperty("My property"));
        }

        [Test]
        public void CanDoExpando()
        {
            var expando = new Expando();
            for (int i = 0; i < 10; i++)
            {
                expando[i.ToString()] = i;
                Assert.AreEqual(i, expando[i.ToString()]);
            }
            Console.WriteLine(expando.GetMembers().Sum(name => (int) expando[name])); // 45
        }
   }
}