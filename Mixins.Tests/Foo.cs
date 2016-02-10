using System;

namespace Mixins.Tests
{
    public class Foo : MMapper
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal SomeProperty
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public ConsoleColor Color
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal Length
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}
