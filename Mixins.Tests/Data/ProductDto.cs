namespace Mixins.Tests.Data
{
    public class ProductDto : IMapper
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal Price
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public string ProducedBy
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}