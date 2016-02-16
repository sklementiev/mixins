using System;
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
            ((MMapper)foo).MapTo((MMapper)bar);
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
            ((MMapper)foo).MapTo((MMapper)bar, true);
            Assert.AreEqual("Foo", bar.Name);
            Assert.AreEqual(null, bar.Tag);
            Assert.AreEqual(null, bar.Boo);
        }
    }
}
