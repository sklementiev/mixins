using System;
using Mixins.Tests.Data;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class Demo
    {
        [Test]
        public void BasicFeatures()
        {
            var banana = new Product
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            var members = banana.GetMembers();
            foreach (var member in members)
            {
                var value = banana.GetProperty(member);
                var type = banana.GetPropertyType(member);
                Console.WriteLine("{0} = {1} of type {2}", member, value, type);
            }

            var banana2 = new Product
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            Assert.IsTrue(banana.EqualsByValue(banana2));
        }

        [Test]
        public void Clone()
        {
            var banana = new CloneableProduct
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            var banana2 = banana.Clone();

            Assert.IsTrue(banana.EqualsByValue(banana2));
        }

        [Test]
        public void Notification()
        {
            var banana = new ProductWithChangeNotification
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            banana.PropertyChanged += (sender, args) =>
            {
                Console.WriteLine("{0} changed to {1}", args.PropertyName, banana.GetProperty(args.PropertyName));
            };

            banana.Price = 3; // prints "Price changed to 3"
        }

        [Test]
        public void Mapper()
        {
            var bananaDto = new ProductDto
            {
                Name = "Banana",
                Price = new decimal(2.5),
                ProducedBy = "Banana tree"
            };

            var banana = new Product();

            bananaDto.MapTo(banana);

            Assert.AreEqual(bananaDto.Name, banana.Name);
            Assert.AreEqual(bananaDto.Price, banana.Price);
        }

        [Test]
        public void Editable()
        {
            var banana = new EditableProduct
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            banana.BeginEdit();
            banana.Name = "Apple";
            banana.Price = 7;
            banana.CancelEdit();

            Assert.AreEqual("Banana", banana.Name);
            Assert.AreEqual(2.5, banana.Price);
        }

        [Test]
        public void ChangeTracking()
        {
            var banana = new ProductWithChangeTracking
            {
                Name = "Banana",
                Price = new decimal(2.5)
            };

            banana.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != "IsChanged") return;
                Console.WriteLine("{0} = {1}", args.PropertyName, banana.GetProperty(args.PropertyName));
            };

            banana.BeginEdit();
            banana.Name = "Apple";  // prints IsChanged = true
            banana.Name = "Banana"; // prints IsChanged = false
            banana.Price = 5;       // prints IsChanged = true
            banana.CancelEdit();    // prints IsChanged = false

            Assert.IsFalse(banana.IsChanged);
            Assert.AreEqual("Banana", banana.Name);
            Assert.AreEqual(2.5, banana.Price);
        }


        [Test]
        public void DynamicClone()
        {
            dynamic banana = new ProductDynamic();
            banana.Name = "Banana";
            banana.Price = new decimal(2.5);
            
            ICloneable mixin = banana;
            dynamic clone = mixin.Clone();
            
            Assert.AreNotSame(banana, clone);
            Assert.AreEqual(banana.Name, clone.Name);
            Assert.AreEqual(banana.Price, clone.Price);
        }
    }
}