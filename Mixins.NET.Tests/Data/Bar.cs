namespace Mixins.Tests.Data
{
    public class Bar : IMixin
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

        public int Count
        {
            get { return (int)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public ConsoleColor Color
        {
            get { return (ConsoleColor)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public string Length
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}