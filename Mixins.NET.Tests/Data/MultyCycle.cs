namespace Mixins.Tests.Data
{
    public class MultyCycle : IComposite
    {
        public string Name
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Wheel> Wheels
        {
            get { return (IEnumerable<Wheel>)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}