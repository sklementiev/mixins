namespace Mixins.Tests.Data
{
    public class ReadOnlyProduct : Product, IReadOnly
    {
        public bool IsReadOnly
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}