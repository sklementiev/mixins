namespace Mixins.Tests.Data
{
    public class Item : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}