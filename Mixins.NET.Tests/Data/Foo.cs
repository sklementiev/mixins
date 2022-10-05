namespace Mixins.Tests.Data
{
    public class Foo : IMapper
    {
        public string Name
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public int Tag
        {
            get { return (int)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public DateTime? Date
        {
            get { return (DateTime?)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal SomeProperty
        {
            get { return (decimal)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public ConsoleColor Color
        {
            get { return (ConsoleColor)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal Length
        {
            get { return (decimal)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}