using System;
using System.Collections.Generic;
using System.Linq;
using Mixins.Tests.Data;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class Mapper
    {
        [Test]
        public void CanMap()
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

        [Test]
        public void CanMapInShapshotMode()
        {
            var foo = new Foo
            {
                Name = "Green Foo",
            };

            var bar = new Bar
            {
                Color = ConsoleColor.Yellow,
                Name = "Yellow Bar",
                Tag = 1,
                Date = DateTime.Today
            };

            foo.MapTo(bar, true);

            Assert.AreEqual(default(ConsoleColor), bar.Color);
            Assert.AreEqual(default(int), bar.Tag);
            Assert.AreEqual(default(DateTime?), bar.Date);
            Assert.AreEqual(foo.Name, bar.Name);
        }

        [Test]
        public void DynamicMixinCanMap()
        {
            dynamic foo = new FooDynamic();
            foo.Name = "Foo";
            foo.Age = 1;
            foo.Tag = 1;
            dynamic bar = new BarDynamic();
            bar.Name = "Bar";
            bar.Age = 2;
            bar.Tag = "tag";
            ((IMapper) foo).MapTo((IMapper) bar);
            Assert.AreEqual("Foo", bar.Name);
            Assert.AreEqual(1, bar.Age);
            Assert.AreEqual("tag", bar.Tag);
        }

        [Test]
        public void DynamicMixinCanMapInShapshotMode()
        {
            dynamic foo = new FooDynamic();
            foo.Name = "Foo";
            dynamic bar = new BarDynamic();
            bar.Name = "Bar";
            bar.Tag = 1;
            bar.Boo = "boo";
            ((IMapper) foo).MapTo((IMapper) bar, true);
            Assert.AreEqual("Foo", bar.Name);
            Assert.AreEqual(null, bar.Tag);
            Assert.AreEqual(null, bar.Boo);
        }

        [Test]
        public void ComplexMixinsCanMap()
        {
            var source = new Whole
            {
                Part = new Part { Name = "part1" },
                Parts = new List<Part> { new Part { Name = "part2" }, new Part { Name = "part3" } }
            };

            var destination = new Whole();
            source.MapTo(destination);
            Assert.IsTrue(source.EqualsByValue(destination));
        }

        [Test]
        public void ComplexMixinsWithArraysCanMap()
        {
            var source = new Whole
            {
                Part = new Part { Name = "part1" },
                Parts = new [] { new Part { Name = "part2" }, new Part { Name = "part3" } }
            };

            var destination = new Whole();
            source.MapTo(destination);
            Assert.IsTrue(source.EqualsByValue(destination));
        }

        [Test]
        public void ComplexMixinsCanMapToOtherComplexMixins()
        {
            var source = new Whole
            {
                Part = new Part { Name = "part1" },
                Parts = new[] { new Part { Name = "part2" }, new Part { Name = "part3" } }
            };

            var destination = new VievModel();
            source.MapTo(destination, deep: true);
            Assert.AreNotSame(source.Part, destination.Part);
            Assert.AreEqual(source.Part.Name, destination.Part.Name);
            Assert.AreNotSame(source.Parts, destination.Parts);
            Assert.AreEqual(source.Parts.Count(), destination.Parts.Count());
            Assert.AreEqual(source.Parts.First().Name, destination.Parts.First().Name);
            Assert.AreEqual(source.Parts.Last().Name, destination.Parts.Last().Name);
        }

        [Test]
        public void CircularRefsMixinsCanMap()
        {
            var source = new Composite() { Name = "Test" };
            source.Part = source;
            var destination = new Composite();
            destination.Part = destination;
            source.MapTo(destination, deep: true);
            Assert.AreEqual("Test", destination.Name);
        }
    }
}