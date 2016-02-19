using System;
using System.Collections.Generic;

namespace Mixins
{
    /// <summary>
    /// Type safe version of ICloneable, with deep clone ability
    /// Deep clone will work only on MCloneable properties
    /// </summary>
    public interface ICloneable : IMixin {}

    public static partial class Extensions
    {
        private static T CloneInternal<T>(this T self, bool deep, Dictionary<ICloneable, ICloneable> cloned)
            where T : ICloneable
        {
            // trace cloned objects to preserve object grapth structure and deal with circular refs
            var clone = Activator.CreateInstance(self.GetType());
            cloned.Add(self, (ICloneable) clone);

            var properties = self.GetPublicState();
            var clonedProperties = new Dictionary<string, object>();
            foreach (var propertyName in properties.Keys)
            {
                var propertyValue = properties[propertyName];
                object clonedValue;
                if (propertyValue is ICloneable && deep)
                {
                    ICloneable alreadyCloned;
                    if (cloned.TryGetValue((ICloneable) propertyValue, out alreadyCloned))
                    {
                        clonedValue = alreadyCloned;
                    }
                    else
                    {
                        clonedValue = ((ICloneable) propertyValue).CloneInternal(true, cloned);
                    }
                }
                else
                {
                    clonedValue = propertyValue;
                }
                clonedProperties.Add(propertyName, clonedValue);
            }

            State.Remove(clone); // ctor might have stored some state already
            State.Add(clone, clonedProperties);
            return (T) clone;
        }

        public static T Clone<T>(this T self, bool deep = false) where T : ICloneable
        {
            return self.CloneInternal(deep, new Dictionary<ICloneable, ICloneable>());
        }
    }
}