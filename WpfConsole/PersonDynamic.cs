using System.ComponentModel;
using Mixins;

namespace WpfConsole
{
    public class PersonDynamic : DynamicMixin, MChangeTracking
    {
        public PersonDynamic()
        {
            dynamic self = this;
            self.FirstName = "Bob";
            self.LastName = "Dynamic";
            //this.NotifyOnChange(
            //    () => FullName,
            //                // depends on
            //    () => FirstName, () => LastName);
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
            get { return this.GetValue() ?? false; }
        }
    }
}