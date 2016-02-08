using System;
using System.ComponentModel;
using Mixins;

namespace WpfConsole
{
	public class Person : MNotifyStateChange, MChangeTracking
	{
		public Person()
		{
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

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsChanged
		{
			get { return this.GetValue(); }
		}
	}
}
