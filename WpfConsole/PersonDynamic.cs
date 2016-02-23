using System.ComponentModel;
using Mixins;
using IChangeTracking = Mixins.IChangeTracking;

namespace WpfConsole
{
    public class PersonDynamic : DynamicMixin, IChangeTracking
    {
        public PersonDynamic()
        {
            dynamic self = this;
            self.FirstName = "Bob";
            self.LastName = "Dynamic";
            this.NotifyOnChange<string>("FullName", "FirstName", "LastName");
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public string FullName
        {
            get
            {
                dynamic self = this;
                return self.FirstName + " " + self.LastName;
            }
        }

        public bool IsChanged
        {
            get { return this.GetValue(); }
        }
    }
}