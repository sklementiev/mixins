using System.ComponentModel;

namespace Mixins
{
    // our version of IRevertibleChangeTracking
    public interface MChangeTracking : MNotifyStateChange, IRevertibleChangeTracking, MEditableObject, MEquatable { }

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
    }
}
