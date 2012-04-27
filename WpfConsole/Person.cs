using System;
using System.ComponentModel;
using Mixins;

namespace WpfConsole
{
	public class Person : Mixin, MNotifyStateChange, MChangeTracking
	{
		public Person()
		{
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

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsChanged
		{
			get { return this.GetProperty(() => IsChanged); }
		}
	}
}
