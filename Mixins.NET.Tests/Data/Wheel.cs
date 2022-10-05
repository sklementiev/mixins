namespace Mixins.Tests.Data
{
    public class Wheel : IComposite
    {
        public string Brand
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}