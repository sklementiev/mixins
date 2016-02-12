using System;
using System.Linq;

namespace Mixins
{
	public interface MDebug : Mixin { }

	public static partial class Extensions
	{
        public static void PrintState(this MDebug self)
        {
            var properties = self.GetStateInternal();
            Console.WriteLine("=========================================================================================================");
            foreach (var propertyName in properties.Keys.OrderBy(c => c))
            {
                Console.WriteLine("{0,-50}{1}", propertyName, properties[propertyName]);
            }
            Console.WriteLine("=========================================================================================================");
        }
    }
}
