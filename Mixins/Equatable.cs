using System.Linq;

namespace Mixins
{
    /// <summary>
    /// We can compare mixins by value
    /// </summary>
    public static partial class Extensions
	{
        public static bool EqualsByValue(this Mixin self, Mixin other)
        {
            return self.GetMembers().All(name => Equals(self.GetProperty(name), other.GetProperty(name)));
        }
	}
}
