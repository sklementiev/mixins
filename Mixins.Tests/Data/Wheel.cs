namespace Mixins.Tests.Data
{
    public class Wheel : IComposite
    {
        public string Brand
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}