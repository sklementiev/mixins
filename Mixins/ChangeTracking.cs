namespace Mixins
{
    public interface MChangeTracking : MNotifyStateChange, MEditableObject
    {
        bool IsChanged { get; }
    }

    /// <summary>
    /// Implementation of System.ComponentModel.IRevertibleChangeTracking
    /// </summary>
	public static partial class Extensions
	{
        private static partial class SystemFields
        {
            public const string IsChanged = "IsChanged";
        }

        public static void AcceptChanges(this MChangeTracking self)
        {
            self.EndEdit();
        }

        public static void RejectChanges(this MChangeTracking self)
        {
            self.CancelEdit();
        }

        internal static void TrackChanges(this MChangeTracking self, string name, object value)
        {
            if (name != SystemFields.IsChanged && self.GetPropertyInternal(SystemFields.Shapshot) != null)
            {
                var wasChanged = self.GetPropertyInternal(SystemFields.IsChanged) as bool?;
                if (wasChanged == null)
                {
                    self.SetPropertyInternal(SystemFields.IsChanged, false); // first hit, define IsChanged
                }
                var shapshot = (Mixin)self.GetPropertyInternal(SystemFields.Shapshot);
                var isChanged = !self.ValueEquals(shapshot);
                self.SetProperty(SystemFields.IsChanged, isChanged);
            }
        }
    }
}
