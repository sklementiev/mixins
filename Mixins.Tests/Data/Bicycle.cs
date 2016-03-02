namespace Mixins.Tests.Data
{
    public class Bicycle : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Wheel FrontWheel
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Wheel RearWheel
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}