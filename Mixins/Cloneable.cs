using System;
using System.Linq;

namespace Mixins
{
    public interface MCloneable : Mixin { } // type safe version of ICloneable
	
	public static partial class Extensions
	{
		// simple implementation of shallow clone
        // TODO: deep clone
		public static T Clone<T>(this T self) where T : Mixin
		{
			var properties = self.GetPublicState();
			var clonedProperties = properties.Keys.ToDictionary(key => key, key => properties[key]);
			var clone = Activator.CreateInstance(self.GetType());
		    // todo: check if exists, update
            State.Remove(clone); // ctor could store some state already
            State.Add(clone, clonedProperties);
			return (T)clone;
		}
    }
}
