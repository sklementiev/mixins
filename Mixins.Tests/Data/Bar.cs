using System;

namespace Mixins.Tests.Data
{
    public class Bar : Mixin
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

        public int Count
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public ConsoleColor Color
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public string Length
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}
