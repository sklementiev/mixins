using System;
using NUnit.Framework;

namespace Mixins.Tests
{
    using Mixins;

	[TestFixture]
	public class UnitTests
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
			Mixin clone = person.Clone();
            Assert.IsTrue(person.ValueEquals(clone));
		}

		[Test]
		public void TestMEditableObject()
		{
			var clone = person.Clone();
			person.BeginEdit();
			Assert.IsTrue(clone.ValueEquals(person));
			person.FirstName = "New name1";
			person.LastName = "New name2";
			person.DateOfBirth = DateTime.Now;
			person.CancelEdit();
			Assert.IsTrue(clone.ValueEquals(person));

			// idempotent
			person.EndEdit();
			person.CancelEdit();
			person.BeginEdit();
			person.BeginEdit();
            Assert.IsTrue(clone.ValueEquals(person));
			person.FirstName = "Alice";
			person.EndEdit();
			person.EndEdit();
			Assert.AreEqual("Alice", person.FirstName);
		}

		[Test]
		public void TestMNotifyStateChange()
		{
			var firstNameChanged = false;
			person.OnPropertyChanged(c => c.FirstName, s => 
			{ 
				Assert.AreEqual("New", s);
				firstNameChanged = true;
			});

			person.FirstName = "New";
			Assert.IsTrue(firstNameChanged);
		}

        [Test]
        public void AssigningSameValueDoesntChangeState()
        {
            var firstNameChanged = false;
            person.OnPropertyChanged(c => c.FirstName, s =>
            {
                firstNameChanged = true;
            });

            person.FirstName = person.FirstName;
            Assert.IsFalse(firstNameChanged);
        }

		[Test]
		public void FullNameDependsOnOtherProperties()
		{
			var fullNameChanged = false;
			person.OnPropertyChanged(c => c.FullName, s =>
			{
				fullNameChanged = true;
			});
			person.FirstName = "New";
			Assert.IsTrue(fullNameChanged);
            fullNameChanged = false;
            person.LastName = "New";
            Assert.IsTrue(fullNameChanged);
		}

        [Test]
        public void TestMMapper()
        {
            var foo = new Foo
            {
                Color = ConsoleColor.Green,
                Name = "Green Foo",
                SomeProperty = 12,
                Length = 100
            };

            var bar = new Bar
            {
                Color = ConsoleColor.Yellow,
                Name = "Yellow Bar",
                Count = 13,
                Length = "LONG"
            };

            foo.MapTo(bar);

            Assert.AreEqual(foo.Color, bar.Color);
            Assert.AreEqual(foo.Name, bar.Name);
            Assert.AreEqual(12, foo.SomeProperty);
            Assert.AreEqual(13, bar.Count);
            Assert.AreEqual(100, foo.Length);
            Assert.AreEqual("LONG", bar.Length);
        }
    }
}
