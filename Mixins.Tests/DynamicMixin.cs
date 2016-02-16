using System;
using Mixins.Tests.Data;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class DynamicMixinTests
    {
        public class Person : DynamicMixin, MCloneable
        {
            public Person()
            {
                dynamic self = this;
                self.Age = 1;
            }
        }

        [Test]
        public void CanStoreAnyPropertyData()
        {
            dynamic person = new Person();
            Assert.AreEqual(1, person.Age);
            Assert.IsNull(person.Name);
            person.Name = "Bob";
            Assert.AreEqual("Bob", person.Name);
        }

        [Test]
        public void CanBeCloned()
        {
            dynamic person = new Person();
            person.Name = "Bob";
            dynamic clone = ((MCloneable)person).Clone();
            var cloneMixin = (MCloneable) clone;
            Assert.AreNotSame(clone, person);
            Assert.AreEqual(clone.Name, person.Name);
            Assert.IsTrue(cloneMixin.EqualsByValue((Mixin)person));
        }

        [Test]
        public void DynamicMixinCanGetPropertyType()
        {
            dynamic foo = new FooDynamic();
            foo.Name = "Foo";
            foo.Boo = 1;
            var fooMixin = (Mixin) foo;
            Assert.AreEqual(typeof(string), fooMixin.GetPropertyType("Name"));
            Assert.AreEqual(typeof(int), fooMixin.GetPropertyType("Boo"));
            foo.Boo = DateTime.Now;
            Assert.AreEqual(typeof(DateTime), fooMixin.GetPropertyType("Boo"));
            foo.Boo = null;
            Assert.AreEqual(typeof(object), fooMixin.GetPropertyType("Boo"));
        }
    }
}
