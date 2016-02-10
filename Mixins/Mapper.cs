using System.Linq;

namespace Mixins
{
    // transfer data between mixins using convention (same property names and data types)
    public interface MMapper : Mixin { } 
	
	public static partial class Extensions
	{
        public static void MapTo(this MMapper self, Mixin destination)
        {
            var state = self.GetPublicState();
            var otherState = destination.GetPublicState();
            var sameProps = state.Select(c => c.Key).Intersect(otherState.Select(c => c.Key));
            foreach (var prop in sameProps.Where(prop => otherState[prop].GetType().IsInstanceOfType(state[prop])))
            {
                destination.SetProperty(prop, self.GetProperty(prop));
            }
        }
    }
}
