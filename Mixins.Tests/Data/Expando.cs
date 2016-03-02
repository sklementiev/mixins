namespace Mixins.Tests.Data
{
    public class Expando : IMixin
    {
        public object this[string name]
        {
            get
            {
                return this.GetProperty(name);
            }
            set
            {
                this.SetProperty(name, value);
            }
        }
    }
}