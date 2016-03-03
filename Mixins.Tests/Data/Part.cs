namespace Mixins.Tests.Data
{
    public class Part : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}