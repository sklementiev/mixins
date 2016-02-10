using System;
using System.ComponentModel;
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

namespace MixinsConsole
{
    public class Creature : 
        IHasName,
        MCanScratch, 
        MCanBark, 
        MChangeTracking,
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

        void IChangeTracking.AcceptChanges()
        {
            this.AcceptChanges();
        }

        void IRevertibleChangeTracking.RejectChanges()
        {
            this.RejectChanges();
        }
    }
}