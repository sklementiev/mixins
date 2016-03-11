using System;
using System.Collections;
using System.Collections.Generic;
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
        public static void MapTo(this IMapper self, IMixin destination, bool shapshot = false, bool deep = false)
        {
            MapToInternal(self, destination, new Dictionary<IMixin, object>(), shapshot, deep);
        }

        public static void MapToInternal(IMapper self, IMixin destination, Dictionary<IMixin, object> path, bool shapshot = false, bool deep = false)
        {
            path.Add(self, null);

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
                var sourceProperty = self.GetProperty(name);
                var destinationPropety = destination.GetProperty(name);
                // property
                if (sourceProperty is IMapper && deep)
                {
                    if (path.ContainsKey((IMixin)sourceProperty)) return;

                    var mapper = (IMapper)sourceProperty;
                    if (destinationPropety == null && destPropType != null && typeof(IMixin).IsAssignableFrom(destPropType))
                    {
                        destinationPropety = Activator.CreateInstance(destPropType);
                        destination.SetProperty(name, destinationPropety);
                    }
                    if (destinationPropety is IMixin)
                    {
                        mapper.MapTo((IMixin)destinationPropety, shapshot, deep: true);
                    }
                    continue;
                }
                // list
                if (sourceProperty is IEnumerable<IMapper> && deep)
                {
                    IList destinationList;
                    if (destinationPropety == null && destPropType != null && typeof(IEnumerable<IMixin>).IsAssignableFrom(destPropType))
                    {
                        destinationList = destPropType.CloneTypedList();
                        destination.SetProperty(name, destinationList);
                    }
                    else if (destinationPropety is IEnumerable<IMixin>)
                    {
                        destinationList = (IList) destinationPropety;
                        destinationList.Clear();
                    }
                    else continue;

                    var type = destinationList.GetType().GetGenericArguments().First();
                    foreach (var item in (IEnumerable<IMapper>)sourceProperty)
                    {
                        if (path.ContainsKey(item)) return;
                        var destItem = (IMixin)Activator.CreateInstance(type);
                        item.MapTo(destItem, shapshot, deep: true);
                        destinationList.Add(destItem);
                    }
                    continue;
                }
                // simple property
                if (destPropType == null || sourcePropType == null || destPropType.IsAssignableFrom(sourcePropType)) // dynamic or compatible 
                {
                    destination.SetProperty(name, self.GetProperty(name));
                }
            }
        }
    }
}