using System.Linq;

namespace Mixins
{
    /// <summary>
    /// Transfer data between mixins using convention (same property names and compatible data types) 
    /// shapshot mode clears out all destination properties that are not in the source
    /// </summary>
    public interface MMapper : Mixin { } 
	
	public static partial class Extensions
	{
	    public static void MapTo(this MMapper self, Mixin destination, bool shapshot = false)
        {
            var state = self.GetPublicState();
            var otherState = destination.GetPublicState();
            
            var sameProps = state.Select(c => c.Key).Intersect(otherState.Select(c => c.Key)).ToList();
            if (shapshot)
            {
                var newProps = otherState.Select(c => c.Key).Except(sameProps).ToList();
                foreach (var prop in newProps)
                {
                    destination.SetProperty(prop, self.GetPropertyType(prop).GetDefaultValue());    
                }
            }
            foreach (var name in sameProps)
            {
                var sourcePropType = self.GetPropertyType(name);
                var destPropType = destination.GetPropertyType(name);
                if (destPropType.IsAssignableFrom(sourcePropType) || shapshot)
                {
                    destination.SetProperty(name, self.GetProperty(name));
                }
            }
        }
    }
}
