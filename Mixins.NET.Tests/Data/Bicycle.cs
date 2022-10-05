namespace Mixins.Tests.Data
{
    public class Bicycle : IComposite
    {
        public string Name
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Wheel FrontWheel
        {
            get { return (Wheel)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Wheel RearWheel
        {
            get { return (Wheel)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}