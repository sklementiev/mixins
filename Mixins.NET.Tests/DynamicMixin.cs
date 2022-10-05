using Mixins.Tests.Data;

namespace Mixins.Tests
{
    [TestFixture]
    public class DynamicMixinTests
    {
        public class Person : DynamicMixin, IEditableObject
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
            dynamic clone = ((ICloneable) person).Clone();
            var cloneMixin = (ICloneable) clone;
            Assert.AreNotSame(clone, person);
            Assert.AreEqual(clone.Name, person.Name);
            Assert.IsTrue(cloneMixin.EqualsByValue((IMixin) person));
        }

        [Test]
        public void DynamicMixinCanGetPropertyType()
        {
            dynamic foo = new FooDynamic();
            foo.Name = "Foo";
            foo.Boo = 1;
            var fooMixin = (IMixin) foo;
            Assert.AreEqual(typeof (string), fooMixin.GetPropertyType("Name"));
            Assert.AreEqual(typeof (int), fooMixin.GetPropertyType("Boo"));
            foo.Boo = DateTime.Now;
            Assert.AreEqual(typeof (DateTime), fooMixin.GetPropertyType("Boo"));
            foo.Boo = null;
            Assert.AreEqual(null, fooMixin.GetPropertyType("Boo"));
        }

        [Test]
        public void CanCancelChanges()
        {
            dynamic foo = new Person();
            var fooMixin = (IEditableObject)foo;
            fooMixin.BeginEdit();
            foo.Name = "Foo";
            foo.Boo = 1;
            fooMixin.CancelEdit();
            Assert.AreEqual(null, foo.Name);
            Assert.AreEqual(null, foo.Boo);
            fooMixin.BeginEdit();
            foo.Name = "Foo2";
            foo.Boo = 2;
            fooMixin.CancelEdit();
            Assert.AreEqual(null, foo.Name);
            Assert.AreEqual(null, foo.Boo);
        }

        [Test]
        public void EqualsByValueWorks()
        {
            dynamic foo = new Person();
            var fooMixin = (IMixin)foo;
            foo.Name = "Bob";
            dynamic bar = new Person();
            bar.Name = "Bob";
            Assert.IsTrue(fooMixin.EqualsByValue((IMixin)bar));

            bar.Tag = 1;
            Assert.IsFalse(fooMixin.EqualsByValue((IMixin)bar));
        }
    }
}