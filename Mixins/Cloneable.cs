using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mixins
{
    /// <summary>
    /// Type safe version of ICloneable, with deep clone ability
    /// Deep clone will work only on ICloneable properties
    /// </summary>
    public interface ICloneable : IMixin {}

    public static partial class Extensions
    {
        private static T CloneInternal<T>(this T self, bool deep, Dictionary<ICloneable, ICloneable> cloned)
            where T : ICloneable
        {
            // trace cloned objects to preserve object graph structure and deal with circular refs
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
                else if (propertyValue is IEnumerable<ICloneable> && deep)
                {
                    // todo : observable collections etc
                    var listType = typeof(List<>);
                    var elementType = propertyValue.GetType().GetGenericArguments();
                    if (!elementType.Any()) // array (we still create List for a clone)
                    {
                        elementType = new [] { propertyValue.GetType().GetElementType() };
                    }
                    var concreteType = listType.MakeGenericType(elementType);
                    var clonedList = (IList)Activator.CreateInstance(concreteType);

                    foreach (var item in (IEnumerable<ICloneable>)propertyValue)
                    {
                        ICloneable alreadyCloned;
                        if (cloned.TryGetValue(item, out alreadyCloned))
                        {
                            clonedList.Add(alreadyCloned);
                        }
                        else
                        {
                            clonedList.Add(item.CloneInternal(true, cloned));
                        }
                    }

                    clonedValue = clonedList;
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