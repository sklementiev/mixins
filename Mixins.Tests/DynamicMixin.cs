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
            Assert.AreNotSame(clone, person);
            Assert.AreEqual(clone.Name, person.Name);
        }
    }
}
