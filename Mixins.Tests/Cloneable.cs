using System;
using NUnit.Framework;

namespace Mixins.Tests
{
	[TestFixture]
	public class Cloneable
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
        public void CloningClones()
        {
            var clone = person.Clone();
            Assert.AreNotSame(person, clone);
            Assert.AreEqual(person.FirstName, clone.FirstName);
            Assert.AreEqual(person.LastName, clone.LastName);
            Assert.AreEqual(person.DateOfBirth, clone.DateOfBirth);
        }

		[Test]
		public void CloneCanBeginEdit()
		{
			person.BeginEdit();
		    person.FirstName = "Kevin";
            var clone = person.Clone();
			Assert.AreNotSame(person, clone);
			Assert.AreEqual(person.FirstName, clone.FirstName);
            clone.BeginEdit();
            Assert.AreEqual(clone.IsChanged, false);
            clone.FirstName = "Bob";
            Assert.AreEqual(clone.IsChanged, true);
		}
    }
}
