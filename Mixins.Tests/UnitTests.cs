using System;
using System.Collections.ObjectModel;
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
				DateOfBirth = DateTime.Parse("11/11/11"),
                //Friends = new List<Person> { 
                //    new Person { FirstName = "Freda", LastName = "Friend" }, 
                //    new Person { FirstName = "Joe", LastName = "Doe" }}
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
			Assert.IsTrue(clone.Equals(Person));
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
            Assert.IsTrue(clone.Equals(Person));
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
        public void AssigningSameValueDoesntChangeState()
        {
            var firstNameChanged = false;
            Person.OnPropertyChanged(c => c.FirstName, s =>
            {
                firstNameChanged = true;
            });

            Person.FirstName = Person.FirstName;
            Assert.IsFalse(firstNameChanged);
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
            fullNameChanged = false;
            Person.LastName = "New";
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
			Assert.IsTrue(((ValueChange)changes["FirstName"]).OldValue.ToString() == clone.FirstName);
            Assert.IsTrue(((ValueChange)changes["FirstName"]).NewValue.ToString() == "Foo");
		}

        [TestMethod]
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

        [TestMethod]
        public void TestCollectionDiffAdded()
        {
            var old = new List<string>
            {
                "One",
                "Two",
                "Three"
            };

            var current = new List<string>
            {
                "One",
                "Two",
                "Three",
                "Four",
                "Five",
                "Five",
                "One"
            };

            var added = new List<string>
            {
                "One",
                "Four",
                "Five",
                "Five"
            };

            var diff = current.GetDiff(old);
            CollectionAssert.AreEqual(added, diff.Added);
        }

        [TestMethod]
        public void TestCollectionDiffDeleted()
        {
            var old = new List<string>
            {
                "One",
                "One",
                "Two",
                "Three"
            };

            var current = new List<string>
            {
                "One",
            };

            var deleted = new List<string>
            {
                "One",
                "Two",
                "Three"            
            };

            var diff = current.GetDiff(old);
            CollectionAssert.AreEqual(deleted, diff.Removed);
        }


        [TestMethod]
        public void TrackCollectionChanges()
        {
            var changedRaisedTimes = 0;
            var listChangedRaisedTimes = 0;

            Person.Friends = new ObservableCollection<Person>
            {
                new Person { FirstName = "Liz", LastName = "Tayler" },
                new Person { FirstName = "Liv", LastName = "Tayler" }
            };

            Person.OnPropertyChanged(c => c.IsChanged, s =>
            {
                changedRaisedTimes++;
            });

            Person.OnPropertyChanged(c => c.Friends, s =>
            {
                listChangedRaisedTimes++;
            });
            
            Person.StartTrackingChanges();
            Person.StartTrackingChanges();
            Assert.IsFalse(Person.IsChanged);
            Person.Friends.Add(new Person { FirstName = "Foo", LastName = "Bar" });
            Assert.IsTrue(Person.IsChanged);
            var changes = Person.GetChanges();
            var change = changes["Friends"] as CollectionChange;
            Assert.IsTrue(change.Added.Count == 1);
            Assert.IsTrue(change.Removed.Count == 0);
            Assert.IsTrue(change.Changed.Count == 0);
            
            Person.Friends.RemoveAt(Person.Friends.Count() - 1);

            changes = Person.GetChanges();
            Assert.AreEqual(0, changes.Count); 

            Assert.IsFalse(Person.IsChanged);
            Person.Friends.RemoveAt(Person.Friends.Count() - 1);
            Assert.IsTrue(Person.IsChanged);

            changes = Person.GetChanges();
            change = changes["Friends"] as CollectionChange;
            Assert.IsTrue(change.Added.Count == 0); 
            Assert.IsTrue(change.Removed.Count == 1);
            Assert.IsTrue(change.Changed.Count == 0);

            Person.Friends[Person.Friends.Count() - 1].LastName = "Murray";
            changes = Person.GetChanges();
            change = changes["Friends"] as CollectionChange;

            Assert.AreEqual(4, changedRaisedTimes);
            Assert.AreEqual(4, listChangedRaisedTimes);
        }

	}
}
