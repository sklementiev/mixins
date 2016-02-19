using System;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class Editable
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
        public void TestMEditableObject()
        {
            var clone = person.Clone();
            person.BeginEdit();
            Assert.IsTrue(clone.EqualsByValue(person));
            person.FirstName = "New name1";
            person.LastName = "New name2";
            person.DateOfBirth = DateTime.Now;
            person.CancelEdit();
            Assert.IsTrue(clone.EqualsByValue(person));

            // idempotent
            person.EndEdit();
            person.CancelEdit();
            person.BeginEdit();
            person.BeginEdit();
            Assert.IsTrue(clone.EqualsByValue(person));
            person.FirstName = "Alice";
            person.EndEdit();
            person.EndEdit();
            Assert.AreEqual("Alice", person.FirstName);
        }
    }
}