namespace Mixins
{
    public interface IEditableObject : ICloneable, IMapper {}

    /// <summary>
    /// Implementation of System.ComponentModel.IEditableObject
    /// </summary>
    public static partial class Extensions
    {
        public static void BeginEdit(this IEditableObject self)
        {
            // store current state to temporary storage
            var state = self.GetPublicState();
            object temp;
            if (state.TryGetValue(SystemFields.Shapshot, out temp)) return; // idempotent
            var deepClone = self is IComposite;
            var clone = self.Clone(deepClone);
            self.SetPropertyInternal(SystemFields.Shapshot, clone);
            self.SetPropertyInternal(SystemFields.IsChanged, false);
        }

        public static void EndEdit(this IEditableObject self)
        {
            // accept current state, discard old state
            var state = self.GetInternalState();
            object clone;
            if (!state.TryGetValue(SystemFields.Shapshot, out clone)) return; // idempotent
            State.Remove(clone);
            state.Remove(SystemFields.Shapshot);

            self.SetProperty(SystemFields.IsChanged, false);
        }

        public static void CancelEdit(this IEditableObject self)
        {
            // restore state from temporary storage, discard temporary state
            var state = self.GetInternalState();
            object clone;
            if (!state.TryGetValue(SystemFields.Shapshot, out clone)) return; // idempotent
            ((IMapper) clone).MapTo(self, true, true);
            State.Remove(clone);
            state.Remove(SystemFields.Shapshot);
        }
    }
}