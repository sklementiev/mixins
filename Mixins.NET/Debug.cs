using System;
using System.Linq;

namespace Mixins
{
    public interface IDebug : IMixin {}

    public static partial class Extensions
    {
        public static void PrintState(this IDebug self)
        {
            var properties = self.GetInternalState();
            Console.WriteLine("=========================================================================================================");
            foreach (var propertyName in properties.Keys.OrderBy(c => c))
            {
                Console.WriteLine("{0,-50}{1}", propertyName, properties[propertyName]);
            }
            Console.WriteLine("=========================================================================================================");
        }
    }
}