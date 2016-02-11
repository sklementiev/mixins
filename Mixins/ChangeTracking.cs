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
    }
}
