namespace Mixins.Tests.Data
{
    public class Composite : IComposite
    {
        public string Name
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Composite Part
        {
            get { return (Composite)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IList<Composite> Parts
        {
            get { return (IList<Composite>)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}