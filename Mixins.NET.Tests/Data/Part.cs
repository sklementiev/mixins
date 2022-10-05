namespace Mixins.Tests.Data
{
    public class Part : IComposite
    {
        public string Name
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}