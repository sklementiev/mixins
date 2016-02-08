using System;
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
	public static class Extensions
	{
		// mixin specific behaviour
		public static void Bark(this MCanBark self)
		{
			var name = "unknown thing";
		    var hasName = self as IHasName;
		    if (hasName != null) name = hasName.Name;
			Console.WriteLine(name + " Barked");
		}

		public static void Scratch(this MCanScratch self)
		{
			var name = "unknown thing";
		    var hasName = self as IHasName;
		    if (hasName != null) name = hasName.Name;
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
		MDisposable
	{
		public string Name
		{
		    get { return this.GetValue(); }
			set { this.SetValue(value); }
		}

		public DateTime? DateOfBirth
		{
            get { return this.GetValue(); }
            set { this.SetValue(value); }
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
			get { return this.GetValue(); }
		}
	}

	class Program
	{
		static void Main()
		{
			var creature = new Creature { Name = "Rusty", DateOfBirth = DateTime.Now };
            creature.DumpState();
			creature.Bark();
			creature.Scratch();

            Console.WriteLine("# State changes - notification");
            creature.PropertyChanging += (sender, eventArgs)
                => Console.Write("{0} : {1} -> ", eventArgs.PropertyName, ((Mixin)sender).GetProperty(eventArgs.PropertyName));
            creature.PropertyChanged += (sender, eventArgs)
                => Console.WriteLine("{0}", ((Mixin)sender).GetProperty(eventArgs.PropertyName));
            creature.Name = "Bob";
            creature.DumpState();

            Console.WriteLine("# MEditableObject");
            creature.BeginEdit();
            creature.Name = "Kevin";
            creature.DateOfBirth = DateTime.Parse("11/11/11");
            creature.DumpState();
            creature.CancelEdit();
            Console.WriteLine("# CancelEdit");
            creature.DumpState();

			Console.WriteLine("# MCloneable, MEquatable");
			Console.WriteLine("Clone the creature!");
			var clone = creature.Clone();
			Trace.Assert(clone.Equals(creature));
			Console.WriteLine("Clone is the same as the original!");

            clone.PropertyChanging += (sender, eventArgs)
                => Console.Write("{0} : {1} -> ", eventArgs.PropertyName, ((Mixin)sender).GetProperty(eventArgs.PropertyName));
            clone.PropertyChanged += (sender, eventArgs)
                => Console.WriteLine("{0}", ((Mixin)sender).GetProperty(eventArgs.PropertyName));

			clone.Name = "Stuart";
			clone.DumpState();

			Console.WriteLine("# OnDispose attached");
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
