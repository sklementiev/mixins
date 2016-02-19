namespace Mixins.Tests.Data
{
    public class FooDynamic : DynamicMixin, IMapper
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}