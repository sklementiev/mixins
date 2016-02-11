using System.Linq;

namespace Mixins
{
    /// <summary>
    /// Implementation of System.IEquatable<T>
    /// </summary>
    public static partial class Extensions
	{
        public static bool ValueEquals(this Mixin self, Mixin other)
		{
			var properties = self.GetPublicState();
			var otherProperties = other.GetPublicState(); 
			return properties.SequenceEqual(otherProperties);
		}
    }
}
