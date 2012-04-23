using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mixins.Tests
{
    public class Foo : Mixin, MMapper
    {
        public string Name
        {
            get { return this.GetProperty(() => Name); }
            set { this.SetProperty(() => Name, value); }
        }

        public decimal SomeProperty
        {
            get { return this.GetProperty(() => SomeProperty); }
            set { this.SetProperty(() => SomeProperty, value); }
        }

        public ConsoleColor Color
        {
            get { return this.GetProperty(() => Color); }
            set { this.SetProperty(() => Color, value); }
        }

        public decimal Length
        {
            get { return this.GetProperty(() => Length); }
            set { this.SetProperty(() => Length, value); }
        }


    }
}
