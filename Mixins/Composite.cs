using System.Linq;

namespace Mixins
{
    /// <summary>
    /// Marker interface to specify a "Composite" object or a part of a "Composite" object
    /// </summary>
    public interface IComposite : ICloneable { }

    /// <summary>
    /// We can compare composite mixins by value
    /// </summary>
    public static partial class Extensions
    {
        public static bool EqualsByValue(this IComposite self, IComposite other)
        {
            if (self == null && other == null) return true;
            if (self == null || other == null) return false;

            if (self.GetMembers().Count() != other.GetMembers().Count())
            {
                return false;
            }
            
            foreach (var member in self.GetMembers())
            {
                var value = self.GetProperty(member);
                var otherValue = other.GetProperty(member);

                var composite = value as IComposite;
                var otherComposite = otherValue as IComposite;
                if (composite != null && otherComposite != null && !composite.EqualsByValue(otherComposite))
                {
                    return false;
                } 
                if (composite == null && !Equals(value, otherValue))
                {
                    return false;
                }
            }
            return true;
        }
    }
}