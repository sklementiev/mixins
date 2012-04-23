using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mixins.Tests
{
    public class Bar : Mixin
    {
        public string Name
        {
            get { return this.GetProperty(() => Name); }
            set { this.SetProperty(() => Name, value); }
        }

        public int Count
        {
            get { return this.GetProperty(() => Count); }
            set { this.SetProperty(() => Count, value); }
        }

        public ConsoleColor Color
        {
            get { return this.GetProperty(() => Color); }
            set { this.SetProperty(() => Color, value); }
        }

        public string Length
        {
            get { return this.GetProperty(() => Length); }
            set { this.SetProperty(() => Length, value); }
        }

    }
}
