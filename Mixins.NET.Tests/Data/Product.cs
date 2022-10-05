namespace Mixins.Tests.Data
{
    public class Product : IMixin
    {
        public string Name
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal Price
        {
            get { return (decimal)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}