using System;
using NUnit.Framework;

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
			Mixin clone = person.Clone();
            Assert.IsTrue(person.EqualsByValue(clone));
		}
    }
}
