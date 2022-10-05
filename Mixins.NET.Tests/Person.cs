using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mixins.Tests
{
    public class Person : IChangeTracking
    {
        public Person()
        {
            Friends = new ObservableCollection<Person>();
            this.NotifyOnChange(() => FullName,
                () => FirstName, () => LastName);
        }

        public string FirstName
        {
            get => this.GetValue() as string;
            set => this.SetValue(value);
        }

        public string LastName
        {
            get { return (string)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public DateTime? DateOfBirth
        {
            get { return (DateTime?)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Person> Friends
        {
            get { return (IEnumerable<Person>)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public Hand LeftHand
        {
            get { return (Hand)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Hand RightHand
        {
            get { return (Hand)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public bool IsChanged
        {
            get { return (bool)this.GetValue(); }
        }
    }

    public class Hand : ICloneable
    {
        public int Length
        {
            get { return (int)this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Person Holds
        {
            get { return (Person)this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}