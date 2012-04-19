using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Mixins;

namespace WpfConsole
{
	public class Person : Mixin, MNotifyStateChange, MChangeTracking
	{
		public Person()
		{
			this.OnPropertyChanged(() => FirstName, UpdateFullName);
			this.OnPropertyChanged(() => LastName, UpdateFullName);
		}
		
		private void UpdateFullName(string s)
		{
			this.RaisePropertyChanged(() => FullName);
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
