using System.Collections.Generic;

namespace Mixins.Tests.Data
{
    public class VievModel : IMixin
    {
        public Item Part
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Item> Parts
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}