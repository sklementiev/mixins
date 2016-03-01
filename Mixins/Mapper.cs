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
                var sourceProperty = self.GetProperty(name);
                var destinationPropety = destination.GetProperty(name);
                // composite property
                if (sourceProperty is IComposite)
                {
                    if (destinationPropety != null)
                    {
                        var mapper = (IMapper) sourceProperty;
                        mapper.MapTo((IMixin)destinationPropety, shapshot);
                    }
                    else
                    {
                        var cloner = (ICloneable) sourceProperty;
                        destination.SetProperty(name, cloner.Clone(true));
                    }
                    continue;
                }
                // composite list
                if (sourceProperty is IEnumerable<IComposite>)
                {
                    var sourceList = sourceProperty as IEnumerable<IComposite>;
                    IList destinationList;
                    if (destinationPropety == null)
                    {
                        destinationList = sourceProperty.CloneTypedList();
                        destination.SetProperty(name, destinationList);
                    }
                    else
                    {
                        destinationList = (IList)destinationPropety;
                        destinationList.Clear();
                    }

                    foreach (var item in sourceList)
                    {
                        destinationList.Add(item.Clone(true));
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