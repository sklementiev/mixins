using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Mixins;

namespace Mixins
{
	public interface IHasName
	{
		string Name { get; set; }
	}

	// can contain required behaviour/data
	public interface MCanBark : Mixin { }

	public interface MCanScratch : Mixin { }
	
	// extensible behaviour/state
	public static partial class Extensions
	{
		// mixin specific behaviour
		public static void Bark(this MCanBark self)
		{
			var name = "unknown thing";
			if (self is IHasName) name = (self as IHasName).Name;
			Console.WriteLine(name + " Barked");
		}

		public static void Scratch(this MCanScratch self)
		{
			var name = "unknown thing";
			if (self is IHasName) name = (self as IHasName).Name;
			Console.WriteLine(name + " Scratched ");
		}
	}
}

namespace MixinsExample
{
	public class Creature : 
		IHasName,
		MCanScratch, 
		MCanBark, 
		MNotifyStateChange, 
		MEditableObject,
		MEquatable,
		MDisposable,
		MChangeTracking
	{
		public string Name
		{
			get { return this.GetProperty(() => Name); }
			set { this.SetProperty(() => Name, value); }
		}

		public DateTime? DateOfBirth
		{
			get { return this.GetProperty(() => DateOfBirth); }
			set { this.SetProperty(() => DateOfBirth, value); }
		}

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		//public void DoBar()
		//{
		//    this.RaisePropertyChanged("Bar");
		//}

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

		public bool Equals(MEquatable other)
		{
			return this.Equals<MEquatable>(other);
		}

		public bool IsChanged
		{
			get { return this.GetProperty(() => IsChanged); }
		}
	}

	class Program
	{
		static void Main()
		{
			var creature = new Creature { Name = "Rusty", DateOfBirth = DateTime.Now };
			creature.Bark();
			creature.Scratch();
			creature.DumpState();

			Console.WriteLine("=> Start Tracking Changes");
			creature.StartTrackingChanges();
			creature.PropertyChanging += (sender, eventArgs)
				=> Console.Write("Property [{0}] is changing from [{1}] ", eventArgs.PropertyName, ((Mixin)sender).GetProperty(eventArgs.PropertyName));
			creature.PropertyChanged += (sender, eventArgs)
				=> Console.WriteLine("to [{0}]", ((Mixin)sender).GetProperty(eventArgs.PropertyName));

			creature.Name = "Any other name";
			creature.Name = "Rusty"; // should reset IsChanges
			creature.Name = "Dusty";
			creature.DateOfBirth = creature.DateOfBirth; // not an actual "change"
			creature.DumpState();
			
			var changes = creature.GetChanges();
			Debug.Assert(changes.Count == 1);
			Debug.Assert(changes["Name"].OldValue.ToString() == "Rusty");
			Debug.Assert(changes["Name"].NewValue.ToString() == "Dusty");
			
			Console.WriteLine("=> Reject Changes");
			creature.RejectChanges();
			creature.DumpState();

			Console.WriteLine("=> MCloneable, MEquatable");
			Console.WriteLine("Clone the creature!");
			var clone = creature.Clone();
			Trace.Assert(clone.Equals(creature));
			Console.WriteLine("Clone is the same as the original!");

			clone.PropertyChanging += (sender, eventArgs)
				=> Console.Write("Property [{0}] is changing from [{1}] ", eventArgs.PropertyName, ((Mixin) sender).GetProperty(eventArgs.PropertyName));

			clone.PropertyChanged += (sender, eventArgs)
				=> Console.WriteLine("to [{0}]", ((Mixin) sender).GetProperty(eventArgs.PropertyName));

			clone.Name = "Cloned Rusty";
			clone.DumpState();

			Console.WriteLine("=> BeginEdit");
			clone.BeginEdit();
			clone.Name = "Rusty 2";
			clone.DateOfBirth = DateTime.Now;
			clone.DumpState();
			clone.CancelEdit();
			Console.WriteLine("=> CancelEdit");

			clone.DumpState();

			Console.WriteLine("=> OnDispose attached");
			clone.OnDispose(c => Console.WriteLine("{0} is disposed!", c.Name));
			clone.DumpState();

			// ReSharper disable RedundantAssignment
			clone = null;
			// ReSharper restore RedundantAssignment
			
			GC.Collect(); // OnDispose action fires

			Console.ReadLine();
		}
	}
}
