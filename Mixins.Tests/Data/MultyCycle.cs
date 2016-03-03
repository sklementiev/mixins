using System.Collections.Generic;

namespace Mixins.Tests.Data
{
    public class MultyCycle : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Wheel> Wheels
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}