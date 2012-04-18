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
		public string FirstName
		{
			get { return this.GetProperty(() => FirstName); }
			set
			{
				this.SetProperty(() => FirstName, value);
				this.RaisePropertyChanged("FullName");
			}
		}

		public string LastName
		{
			get { return this.GetProperty(() => LastName); }
			set
			{
				this.SetProperty(() => LastName, value);
				this.RaisePropertyChanged("FullName");
			}
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
