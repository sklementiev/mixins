using System;

namespace Mixins.Tests.Data
{
    public class Foo : IMapper
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public int Tag
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public DateTime? Date
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