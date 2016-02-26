using System.Collections.Generic;
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
                var compositeList = value as IEnumerable<IComposite>;
                var otherCompositeList = otherValue as IEnumerable<IComposite>;
                if (compositeList != null && otherCompositeList != null)
                {
                    for (var i = 0; i < compositeList.ToList().Count(); i++)
                    {
                        if (!compositeList.ElementAt(i).EqualsByValue(otherCompositeList.ElementAt(i))) return false;
                    }
                }
                if (composite == null && compositeList == null && !Equals(value, otherValue))
                {
                    return false;
                }
            }
            return true;
        }
    }
}