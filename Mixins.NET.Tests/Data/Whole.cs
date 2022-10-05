namespace Mixins.Tests.Data
{
    public class Whole : IComposite, IEditableObject
    {
        public Part Part
        {
            get { return (Part)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Part> Parts
        {
            get { return (IEnumerable<Part>)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}