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
        public void ClonedCanBeginEdit()
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

        [Test]
        public void DeepCloneClones()
        {
            person.LeftHand = new Hand {Length = 12};
            person.RightHand = new Hand {Length = 13};
            var clone = person.Clone(true);
            Assert.AreEqual(12, clone.LeftHand.Length);
            Assert.AreEqual(13, clone.RightHand.Length);
            Assert.AreNotSame(person.LeftHand, clone.LeftHand);
            Assert.AreNotSame(person.RightHand, clone.RightHand);
        }

        [Test]
        public void DeepClonePreservesObjectGraphShape()
        {
            person.LeftHand = new Hand {Length = 12};
            person.RightHand = person.LeftHand;
            var clone = person.Clone(true);
            Assert.AreEqual(12, clone.LeftHand.Length);
            Assert.AreSame(clone.LeftHand, clone.RightHand);
        }

        [Test]
        public void DeepCloneClonesWithCircularRefs()
        {
            person.LeftHand = new Hand {Holds = person};
            person.RightHand = new Hand {Holds = person};
            var clone = person.Clone(true); // another self-embraced person )
            Assert.AreNotSame(person.LeftHand, clone.LeftHand);
            Assert.AreNotSame(person.RightHand, clone.RightHand);
            Assert.AreNotSame(clone.RightHand, clone.LeftHand);
            Assert.AreSame(clone, clone.RightHand.Holds);
            Assert.AreSame(clone, clone.LeftHand.Holds);
        }
    }
}