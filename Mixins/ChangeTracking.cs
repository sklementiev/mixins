namespace Mixins
{
    public interface IChangeTracking : INotifyStateChange, IEditableObject
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

        public static void AcceptChanges(this IChangeTracking self)
        {
            self.EndEdit();
        }

        public static void RejectChanges(this IChangeTracking self)
        {
            self.CancelEdit();
        }

        internal static void TrackChanges(this IChangeTracking self, string name, object value)
        {
            if (name != SystemFields.IsChanged && self.GetPropertyInternal(SystemFields.Shapshot) != Value.Undefined)
            {
                var shapshot = (IMixin) self.GetPropertyInternal(SystemFields.Shapshot);
                var isChanged = !self.EqualsByValue(shapshot);
                self.SetProperty(SystemFields.IsChanged, isChanged);
            }
        }
    }
}