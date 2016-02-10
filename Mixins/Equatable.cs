using System;
using System.Linq;

namespace Mixins
{
    public interface MEquatable : Mixin, IEquatable<MEquatable> { } // we can compare mixins by value

    public static partial class Extensions
	{
		public static bool Equals<T>(this MEquatable self, T other) where T : MEquatable
		{
			var properties = self.GetPublicState();
			var otherProperties = other.GetPublicState(); 
			return properties.SequenceEqual(otherProperties);
		}
    }
}
