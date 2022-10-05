namespace Mixins.Tests.Data
{
    public class Item : IComposite
    {
        public string Name
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}