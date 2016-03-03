using System.Collections.Generic;

namespace Mixins.Tests.Data
{
    public class Composite : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Composite Part
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IList<Composite> Parts
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}