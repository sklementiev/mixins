using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mixins.Tests
{
	public class Person : MChangeTracking, MEquatable
	{
		public Person()
		{
            Friends = new ObservableCollection<Person>();
            this.NotifyOnChange(() => FullName, 
				() => FirstName, () => LastName);
        }

		public string FirstName
		{
			get { return this.GetValue(); }
			set { this.SetValue(value); }
		}

		public string LastName
		{
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

		public string FullName
		{
			get { return FirstName + " " + LastName; }
		}

		public DateTime? DateOfBirth
		{
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public ObservableCollection<Person> Friends
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

	    public bool IsChanged
		{
			get { return this.GetValue(); }
		}

        void IChangeTracking.AcceptChanges()
        {
            this.AcceptChanges();
        }

        void IRevertibleChangeTracking.RejectChanges()
        {
            this.RejectChanges();
        }

	    public bool Equals(MEquatable other)
		{
			return this.Equals<MEquatable>(other);
		}

		void IEditableObject.BeginEdit()
		{
			this.BeginEdit();
		}

		void IEditableObject.EndEdit()
		{
			this.EndEdit();
		}

		void IEditableObject.CancelEdit()
		{
			this.CancelEdit();
		}
	}
}
