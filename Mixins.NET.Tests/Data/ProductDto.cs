namespace Mixins.Tests.Data
{
    public class ProductDto : IMapper
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

        public string ProducedBy
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}