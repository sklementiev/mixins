namespace Mixins.Tests.Data
{
    public class VievModel : IMixin
    {
        public Item Part
        {
            get { return (Item)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Item> Parts
        {
            get { return (IEnumerable<Item>)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}