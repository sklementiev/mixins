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
            return self.GetMembers().All(name => Equals(self.GetProperty(name), other.GetProperty(name)));
        }
	}
}
