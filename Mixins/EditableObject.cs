using System.ComponentModel;

namespace Mixins
{
    // another example of implementing standart interfaces
    public interface MEditableObject : MCloneable, MMapper, IEditableObject { } 

	public static partial class Extensions
	{
		private static partial class SystemFields
        {
            public const string Shapshot = "#shapshot";
        }

		public static void BeginEdit(this MEditableObject self)
		{
            // store current state to temporary storage
			var state = self.GetPublicState();
			object temp;
			if(state.TryGetValue(SystemFields.Shapshot, out temp)) return; // idempotent
			var clone = self.Clone<Mixin>();
			self.SetPropertyInternal(SystemFields.Shapshot, clone);
		}

		public static void EndEdit(this MEditableObject self)
		{
            // accept current state, discard old state
			var state = self.GetStateInternal();
            object clone;
            if (!state.TryGetValue(SystemFields.Shapshot, out clone)) return; // idempotent
            State.Remove(clone);
			state.Remove(SystemFields.Shapshot);

            self.SetProperty(SystemFields.IsChanged, false);
		}

		public static void CancelEdit(this MEditableObject self)
		{
            // self.SetProperty(SystemFields.IsChanged, false);
            // restore state from temporary storage, discard temporary state
			var state = self.GetStateInternal();
			object clone;
			if (!state.TryGetValue(SystemFields.Shapshot, out clone)) return; // idempotent
            ((MMapper)clone).MapTo(self);
            State.Remove(clone);
            state.Remove(SystemFields.Shapshot);
		}
    }
}
