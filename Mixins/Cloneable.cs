using System;
using System.Collections.Generic;

namespace Mixins
{
    // type safe version of ICloneable
    public interface MCloneable : Mixin { } 
	
	public static partial class Extensions
	{
        private static T CloneInternal<T>(this T self, bool deep, Dictionary<MCloneable, MCloneable> cloned) where T : MCloneable
	    {
            var clone = Activator.CreateInstance(self.GetType());
            cloned.Add(self, (MCloneable)clone);

            var properties = self.GetPublicState();
            //var clonedProperties = properties.Keys.ToDictionary(key => key, key => properties[key]);
            var clonedProperties = new Dictionary<string, object>();
            foreach (var propertyName in properties.Keys)
            {
                var propertyValue = properties[propertyName];
                object clonedValue;
                if (propertyValue is MCloneable && deep)
                {
                    MCloneable alreadyCloned;
                    if (cloned.TryGetValue((MCloneable)propertyValue, out alreadyCloned))
                    {
                        clonedValue = alreadyCloned;
                    }
                    else
                    {
                        clonedValue = ((MCloneable) propertyValue).CloneInternal(true, cloned);
                    }
                }
                else
                {
                    clonedValue = propertyValue;
                }
                clonedProperties.Add(propertyName, clonedValue);
            }
            
            // get and update?
            State.Remove(clone); // ctor might have stored some state already
            State.Add(clone, clonedProperties);
            return (T)clone;
	    }
        
        // deep clone will work only on MCloneable properties
        public static T Clone<T>(this T self, bool deep = false) where T : MCloneable
        {
            return self.CloneInternal(deep, new Dictionary<MCloneable, MCloneable>());
		}
    }
}
