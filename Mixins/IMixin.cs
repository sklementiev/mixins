using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Mixins
{
    public interface IMixin
    {
    }

    public static partial class Extensions
    {
        private static readonly ConditionalWeakTable<object, Dictionary<string, object>> State =
            new ConditionalWeakTable<object, Dictionary<string, object>>();

        public static dynamic GetValue(this IMixin self, [CallerMemberName] string name = null)
        {
            return self.GetPropertyInternal(name);
        }

        public static void SetValue(this IMixin self, object value, [CallerMemberName] string name = null)
        {
            self.SetProperty(name, value);
        }

        public static object GetProperty(this IMixin self, string name)
        {
            return self.GetPropertyInternal(name);
        }

        public static IEnumerable<string> GetMembers(this IMixin self)
        {
            return self.GetPublicState().Keys;
        }

        public static Type GetPropertyType(this IMixin self, string name)
        {
            var property = self.GetType().GetProperty(name);
            if (property != null) return self.GetType().GetProperty(name).PropertyType;
            var value = self.GetProperty(name);
            return value == null ? typeof (object) : value.GetType();
        }

        private static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        internal static void SetProperty(this IMixin self, string name, object value)
        {
            if (Equals(value, self.GetProperty(name))) return;
            var newValue = StateChanging(self, name, value);
            self.SetPropertyInternal(name, value);
            StateChanged(self, name, value);
        }

        internal static Dictionary<string, object> GetStateInternal(this IMixin self)
        {
            return State.GetOrCreateValue(self);
        }

        private static partial class SystemFields
        {
            public const char Prefix = '#';
        }

        internal static Dictionary<string, object> GetPublicState(this IMixin self)
        {
            return self.GetStateInternal()
                .Where(c => c.Key.First() != SystemFields.Prefix && c.Key != SystemFields.IsChanged)
                .ToDictionary(c => c.Key, c => c.Value);
        }

        internal static void SetPropertyInternal(this IMixin self, string name, object value)
        {
            self.GetStateInternal()[name] = value;
        }

        internal static object GetPropertyInternal(this IMixin self, string name)
        {
            object value;
            var success = self.GetStateInternal().TryGetValue(name, out value);
            return value;
        }

        private static object StateChanging(object self, string name, object value)
        {
            var notifyStateChange = self as INotifyStateChange;
            if (notifyStateChange != null)
            {
                StateChanging(notifyStateChange, name, value);
            }
            return value;
        }

        private static void StateChanged(object self, string name, object value)
        {
            var notifyStateChange = self as INotifyStateChange;
            if (notifyStateChange != null)
            {
                StateChanged(notifyStateChange, name, value);
            }
        }
    }
}