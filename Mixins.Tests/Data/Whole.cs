using System.Collections.Generic;

namespace Mixins.Tests.Data
{
    public class Whole : IComposite, IEditableObject
    {
        public Part Part
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Part> Parts
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}