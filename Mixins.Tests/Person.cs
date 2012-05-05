using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mixins.Tests
{
	public class Person : Mixin, MNotifyStateChange, MChangeTracking, MEquatable, MCloneable, MEditableObject, MComposite
	{
		public Person()
		{
            Friends = new ObservableCollection<Person>();
            this.NotifyOnChange(() => FullName, 
				() => FirstName, () => LastName);
        }


		public string FirstName
		{
			get { return this.GetProperty(() => FirstName); }
			set { this.SetProperty(() => FirstName, value); }
		}

		public string LastName
		{
			get { return this.GetProperty(() => LastName); }
			set { this.SetProperty(() => LastName, value); }
		}

		public string FullName
		{
			get { return FirstName + " " + LastName; }
		}

		public DateTime? DateOfBirth
		{
			get { return this.GetProperty(() => DateOfBirth); }
			set { this.SetProperty(() => DateOfBirth, value); }
		}

        public ObservableCollection<Person> Friends
        {
            get { return this.GetProperty(() => Friends); }
            private set { this.SetProperty(() => Friends, value); }
        }

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsChanged
		{
			get { return this.GetProperty(() => IsChanged); }
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
