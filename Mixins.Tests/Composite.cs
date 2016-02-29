using System.Collections.Generic;
using System.Linq;
using Mixins.Tests.Data;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class Composite
    {
        [Test]
        public void EmptyCompositesAreEqual()
        {
            var bike = new MultyCycle();
            var clone = bike.Clone(deep: true); 
            Assert.IsTrue(bike.EqualsByValue(clone));
        }

        [Test]
        public void CompositesWithListsOfDifferentLenghtsAreNotEqual()
        {
            var tricycle = new MultyCycle
            {
                Name = "Lightning",
                Wheels = new List<Wheel> { new Wheel { Brand = "Dunlop" }, new Wheel { Brand = "Dunlop" }, new Wheel { Brand = "Noname" } }
            };

            var clone = tricycle.Clone(deep: true); // deep clone!

            Assert.AreNotSame(tricycle, clone);
            Assert.AreNotSame(tricycle.Wheels, clone.Wheels);
            Assert.AreEqual(tricycle.Wheels.Count(), clone.Wheels.Count());

            // compare the whole bike with the clone including wheels
            Assert.IsTrue(tricycle.EqualsByValue(clone));

            ((IList<Wheel>)clone.Wheels).RemoveAt(2);
            Assert.IsFalse(tricycle.EqualsByValue(clone));
        }

        [Test]
        public void CompositesWithListsOfDifferentOrderAreNotEqual()
        {
            var tricycle = new MultyCycle
            {
                Name = "Lightning",
                Wheels = new List<Wheel> { new Wheel { Brand = "Dunlop" }, new Wheel { Brand = "Dunlop" }, new Wheel { Brand = "Noname" } }
            };

            var clone = tricycle.Clone(deep: true); // deep clone!

            Assert.AreNotSame(tricycle, clone);
            Assert.AreNotSame(tricycle.Wheels, clone.Wheels);
            Assert.AreEqual(tricycle.Wheels.Count(), clone.Wheels.Count());

            // compare the whole bike with the clone including wheels
            Assert.IsTrue(tricycle.EqualsByValue(clone));

            var list = ((IList<Wheel>) clone.Wheels);
            var first = list.First();
            list.Remove(first);
            list.Add(first);
            Assert.AreEqual(tricycle.Wheels.Count(), clone.Wheels.Count());

            Assert.IsFalse(tricycle.EqualsByValue(clone));
        }

        [Test]
        public void CompositesWithArraysCanBeClonedAndCompared()
        {
            var tricycle = new MultyCycle
            {
                Name = "Lightning",
                Wheels = new [] { new Wheel { Brand = "Dunlop" }, new Wheel { Brand = "Dunlop" }, new Wheel { Brand = "Noname" } }
            };

            var clone = tricycle.Clone(deep: true); 
            Assert.IsTrue(tricycle.EqualsByValue(clone));
        }

        [Test]
        public void DynamicCompositesCanBeClonedAndCompared()
        {
            dynamic bike = new DynamicBicycle();
            bike.Wheels = new List<Wheel> { new Wheel { Brand = "Noname" }, new Wheel { Brand = "Noname" } };
            bike.Frame = new Frame();
            bike.Frame.Type = "BMX";
            var mixin = (IComposite) bike;

            dynamic clone = mixin.Clone(deep: true);
            Assert.AreNotSame(bike, clone);
            Assert.AreNotSame(bike.Frame, clone.Frame);
            Assert.AreNotSame(bike.Wheels, clone.Wheels);

            Assert.IsTrue(mixin.EqualsByValue((IComposite)clone));

            clone.Frame.Type = "Hybrid";
            Assert.IsFalse(mixin.EqualsByValue((IComposite)clone));
        }
   }
}