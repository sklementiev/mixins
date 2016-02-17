using System;
using Mixins.Tests.Data;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class Demo
    {
        [Test]
        public void BasicFeatures()
        {
            var banana = new Product
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            var members = banana.GetMembers();
            foreach (var member in members)
            {
                var value = banana.GetProperty(member);
                var type = banana.GetPropertyType(member);
                Console.WriteLine("{0} = {1} of type {2}", member, value, type);
            }

            var banana2 = new Product
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            Assert.IsTrue(banana.EqualsByValue(banana2));
        }

        [Test]
        public void Clone()
        {
            var banana = new CloneableProduct
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            var banana2 = banana.Clone();

            Assert.IsTrue(banana.EqualsByValue(banana2));
        }
    }
}