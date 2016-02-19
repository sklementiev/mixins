using System.Linq;

namespace Mixins
{
    /// <summary>
    /// Transfer data between mixins using convention (same property names and compatible data types) 
    /// shapshot mode clears out all destination properties that are not in the source
    /// </summary>
    public interface IMapper : IMixin {}

    public static partial class Extensions
    {
        public static void MapTo(this IMapper self, IMixin destination, bool shapshot = false)
        {
            if (shapshot)
            {
                var newProps = destination.GetMembers().Except(self.GetMembers()).ToList();
                foreach (var prop in newProps)
                {
                    destination.SetProperty(prop, self.GetPropertyType(prop).GetDefaultValue());
                }
            }
            foreach (var name in self.GetMembers())
            {
                var sourcePropType = self.GetPropertyType(name);
                var destPropType = destination.GetPropertyType(name);
                if (destPropType == null || sourcePropType == null || destPropType.IsAssignableFrom(sourcePropType)) // dynamic or compatible 
                {
                    destination.SetProperty(name, self.GetProperty(name));
                }
            }
        }
    }
}