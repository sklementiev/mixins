using System;
using System.Linq;

namespace Mixins
{
    // type safe version of ICloneable
    public interface MCloneable : Mixin { } 
	
	public static partial class Extensions
	{
        // deep clone will work only on MCloneable properties
        public static T Clone<T>(this T self, bool deep = false) where T : Mixin
		{
			var properties = self.GetPublicState();
            // TODO: deep clone, lists
            //var properties = self.GetStateInternal();
			var clonedProperties = properties.Keys.ToDictionary(key => key, key => properties[key]);
			var clone = Activator.CreateInstance(self.GetType());
            State.Remove(clone); // ctor could store some state already
            State.Add(clone, clonedProperties);
			return (T)clone;
		}
    }
}
