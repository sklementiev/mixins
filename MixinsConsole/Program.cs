using System;
using System.Diagnostics;
using Mixins;

namespace MixinsConsole
{
    internal class Program
    {
        private static void Main()
        {
            var creature = new Creature {Name = "Rusty", DateOfBirth = DateTime.Now};
            creature.PrintState();
            creature.Bark();
            creature.Scratch();

            Console.WriteLine("# State changes - notification");
            creature.PropertyChanging += (sender, eventArgs)
                =>
                Console.Write("{0} : {1} -> ", eventArgs.PropertyName,
                    ((IMixin) sender).GetProperty(eventArgs.PropertyName));
            creature.PropertyChanged += (sender, eventArgs)
                => Console.WriteLine("{0}", ((IMixin) sender).GetProperty(eventArgs.PropertyName));
            creature.Name = "Bob";
            creature.PrintState();

            Console.WriteLine("# MEditableObject");
            creature.BeginEdit();
            creature.Name = "Kevin";
            creature.Name = "Bob"; // IsChanged == false again!
            creature.Name = "Stuart";
            creature.DateOfBirth = DateTime.Parse("11/11/11");
            creature.PrintState();
            Console.WriteLine("# CancelEdit");
            creature.CancelEdit();
            creature.PrintState();

            creature.BeginEdit();
            creature.Name = "Kevin";
            Console.WriteLine("# AcceptChanges");
            creature.AcceptChanges();
            creature.PrintState();

            Console.WriteLine("# MCloneable, MEquatable");
            Console.WriteLine("Clone the creature!");
            var clone = creature.Clone();
            Trace.Assert(clone.EqualsByValue(creature));
            clone.PrintState();
            Console.WriteLine("Clone is the same as the original!");

            clone.PropertyChanging += (sender, eventArgs)
                =>
                Console.Write("{0} : {1} -> ", eventArgs.PropertyName,
                    ((IMixin) sender).GetProperty(eventArgs.PropertyName));
            clone.PropertyChanged += (sender, eventArgs)
                => Console.WriteLine("{0}", ((IMixin) sender).GetProperty(eventArgs.PropertyName));

            Console.WriteLine("# BeginEdit on clone");
            clone.BeginEdit();
            clone.Name = "Clone";
            clone.PrintState();

            Console.WriteLine("# OnDispose attached");
            clone.OnDispose(c => Console.WriteLine("{0} is disposed!", c.Name));
            clone.PrintState();

            // ReSharper disable RedundantAssignment
            clone = null;
            // ReSharper restore RedundantAssignment

            GC.Collect(); // OnDispose action fires

            Console.ReadLine();
        }
    }
}