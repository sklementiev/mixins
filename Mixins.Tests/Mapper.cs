using System;
using NUnit.Framework;

namespace Mixins.Tests
{
    [TestFixture]
    public class Mapper
    {
        [Test]
        public void MapperMaps()
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
