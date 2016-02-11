using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Mixins
{
	public interface Mixin { }

	public static partial class Extensions
	{
		private static readonly ConditionalWeakTable<object, Dictionary<string, object>> State = new ConditionalWeakTable<object, Dictionary<string, object>>();

        public static dynamic GetValue(this Mixin self, [CallerMemberName] string name = null)
        {
            return self.GetPropertyInternal(name);
        }

        public static void SetValue(this Mixin self, object value, [CallerMemberName] string name = null)
        {
            self.SetProperty(name, value);
        }

        public static object GetProperty(this Mixin self, string name)
        {
            return self.GetPropertyInternal(name);
        }

        public static void DumpState(this Mixin self)
        {
            var properties = self.GetStateInternal();
            Console.WriteLine("=========================================================================================================");
            foreach (var propertyName in properties.Keys.OrderBy(c => c))
            {
                Console.WriteLine("{0,-50}{1}", propertyName, properties[propertyName]);
            }
            Console.WriteLine("=========================================================================================================");
        }

		internal static void SetProperty(this Mixin self, string name, object value)
		{
			if(Equals(value, self.GetProperty(name))) return;
			var newValue = StateChanging(self, name, value);
			self.SetPropertyInternal(name, value);
			StateChanged(self, name, value);
		}

		internal static Dictionary<string, object> GetStateInternal(this Mixin self)
		{
			return State.GetOrCreateValue(self);
		}

	    internal const char SystemFieldPrefix = '#';

		internal static Dictionary<string, object> GetPublicState(this Mixin self)
		{
            return self.GetStateInternal()
                .Where(c => c.Key.First() != SystemFieldPrefix && c.Key != SystemFields.IsChanged)
                .ToDictionary(c => c.Key, c => c.Value); 
		}

		internal static void SetPropertyInternal(this Mixin self, string name, object value)
		{
			self.GetStateInternal()[name] = value;
		}

		internal static object GetPropertyInternal(this Mixin self, string name)
		{
			object value;
			var success = self.GetStateInternal().TryGetValue(name, out value);
			return value;
		}

		private static object StateChanging(object self, string name, object value)
		{
			var notifyStateChange = self as MNotifyStateChange;
			if (notifyStateChange != null)
			{
				StateChanging(notifyStateChange, name, value);
			}
            return value;
		}

		private static void StateChanged(object self, string name, object value)
		{
			var notifyStateChange = self as MNotifyStateChange;
			if (notifyStateChange != null)
			{
				StateChanged(notifyStateChange, name, value);
			}
		}
    }
}
