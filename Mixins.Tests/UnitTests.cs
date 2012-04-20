using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mixins.Tests
{
	[TestClass]
	public class UnitTests
	{
		internal Person Person;
		
		[TestInitialize]
		public void Init()
		{
			Person = new Person
			{
			    FirstName = "Bill",
				LastName = "Klingon",
				DateOfBirth = DateTime.Parse("11/11/11")
			};
		}

		[TestMethod]
		public void TestMCloneable()
		{
			var clone = Person.Clone();
			Assert.AreNotSame(Person, clone);
			Assert.AreEqual(Person.FirstName, clone.FirstName);
			Assert.AreEqual(Person.LastName, clone.LastName);
			Assert.AreEqual(Person.DateOfBirth, clone.DateOfBirth);
		}

		[TestMethod]
		public void TestMEquatable()
		{
			var clone = Person.Clone();
			Assert.IsTrue(clone.Equals(Person));
		}

		[TestMethod]
		public void TestMEditableObject()
		{
			var clone = Person.Clone();
			Person.BeginEdit();
			Person.FirstName = "New name1";
			Person.LastName = "New name2";
			Person.DateOfBirth = DateTime.Now;
			Person.CancelEdit();
			Assert.IsTrue(clone.Equals(Person));

			// idempotent
			Person.EndEdit();
			Person.CancelEdit();
			Person.BeginEdit();
			Person.BeginEdit();
			Person.FirstName = "Alice";
			Person.EndEdit();
			Person.EndEdit();
			Assert.AreEqual("Alice", Person.FirstName);
		}

		[TestMethod]
		public void TestMNotifyStateChange()
		{
			var firstNameChanged = false;
			Person.OnPropertyChanged(c => c.FirstName, s => 
			{ 
				Assert.AreEqual("New", s);
				firstNameChanged = true;
			});

			Person.FirstName = "New";
			Assert.IsTrue(firstNameChanged);
		}

		[TestMethod]
		public void FullNameDependsOnOtherProperties()
		{
			var fullNameChanged = false;
			Person.OnPropertyChanged(c => c.FullName, s =>
			{
				fullNameChanged = true;
			});
			Person.FirstName = "New";
			Assert.IsTrue(fullNameChanged);
		}

		[TestMethod]
		public void TestMChangeTracking()
		{
			var clone = Person.Clone();

			var isChangedChanged = false;
			Person.OnPropertyChanged(c => c.IsChanged, s =>
			{
				isChangedChanged = true;
			});

			Person.StartTrackingChanges();
			Assert.IsFalse(Person.IsChanged);
			Person.FirstName = "New";
			Assert.IsTrue(Person.IsChanged);
			Assert.IsTrue(isChangedChanged);
			Person.RejectChanges();
			Assert.IsFalse(Person.IsChanged);
			Assert.IsTrue(clone.Equals(Person));

			// idempotent
			Person.RejectChanges();
			Person.RejectChanges();
			Person.StartTrackingChanges();
			Person.StartTrackingChanges();
			Person.FirstName = "Foo";
			var changes = Person.GetChanges();
			Assert.IsTrue(changes.Count == 1);
			Assert.IsTrue(changes["FirstName"].OldValue.ToString() == clone.FirstName);
			Assert.IsTrue(changes["FirstName"].NewValue.ToString() == "Foo");

		}

		


	}
}
