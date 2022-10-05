namespace Mixins.Tests
{
    using Mixins;

    [TestFixture]
    public class Equatable
    {
        private Person person;

        [SetUp]
        public void Init()
        {
            person = new Person
            {
                FirstName = "Bob",
                LastName = "Minion",
                DateOfBirth = DateTime.Parse("11/11/11"),
            };
        }

        [Test]
        public void TestMEquatable()
        {
            IMixin clone = person.Clone();
            Assert.IsTrue(person.EqualsByValue(clone));
        }

        [Test]
        public void NullMixinsAreEqualByValue()
        {
            var mixin = (IMixin)null;
            var mixin2 = (IMixin)null;
            Assert.IsTrue(mixin.EqualsByValue(mixin2));
        }

        [Test]
        public void NullMixinAreNotEqualByValueToNonNullMixin()
        {
            Assert.IsFalse(person.EqualsByValue(null));
        }
    }
}