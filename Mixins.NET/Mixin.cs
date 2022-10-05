using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Mixins
{
    /// <summary>
    /// Base Mixin interface
    /// </summary>
    public interface IMixin {}

    public static partial class Extensions
    {
        private static readonly ConditionalWeakTable<object, Dictionary<string, object>> State = new();

        public static object GetValue(this IMixin self, [CallerMemberName] string name = null)
        {
            return self.GetProperty(name);
        }

        public static void SetValue(this IMixin self, object value, [CallerMemberName] string name = null)
        {
            self.SetProperty(name, value);
        }

        public static object GetProperty(this IMixin self, string name)
        {
            EnsurePropertyName(ref name);
            var type = self.GetPropertyType(name);
            var value = self.GetPropertyInternal(name);
            if (type != null && value == Value.Undefined)
            {
                return type.GetDefaultValue();
            }
            return value == Value.Undefined ? null : value;
        }

        public static void SetProperty(this IMixin self, string name, object value)
        {
            EnsurePropertyName(ref name); 
            if (Equals(value, self.GetProperty(name))) return;
            if (!StateChanging(self, name, value)) return; // we can cancel state change
            self.SetPropertyInternal(name, value);
            StateChanged(self, name, value);
        }

        public static IEnumerable<string> GetMembers(this IMixin self)
        {
            return self.GetPublicState().Keys;
        }

        public static Type GetPropertyType(this IMixin self, string name)
        {
            var property = self.GetType().GetProperty(name);
            if (property != null) return self.GetType().GetProperty(name).PropertyType;
            var value = self.GetPropertyInternal(name);
            return value == null || value == Value.Undefined ? null : value.GetType();
        }

        private static void EnsurePropertyName(ref string name)
        {
            name = name.Trim();
            if (name.StartsWith(SystemFields.Prefix))
            {
                throw new Exception("Property name cannot start with #");
            }
        }

        private static object GetDefaultValue(this Type type)
        {
            if (type == null) return null;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private static IList CloneTypedList(this object propertyValue)
        {
            return propertyValue.GetType().CloneTypedList();
        }

        private static IList CloneTypedList(this Type propertyType)
        {
            var listType = typeof(List<>);
            var elementType = propertyType.GetGenericArguments();
            if (!elementType.Any()) // array (we still create List for a clone)
            {
                elementType = new[] { propertyType.GetElementType() };
            }
            var concreteType = listType.MakeGenericType(elementType);
            return (IList)Activator.CreateInstance(concreteType);
        }

        internal static Dictionary<string, object> GetInternalState(this IMixin self)
        {
            return State.GetOrCreateValue(self);
        }

        internal static Dictionary<string, object> GetPublicState(this IMixin self)
        {
            return self.GetInternalState()
                .Where(c => !c.Key.StartsWith(SystemFields.Prefix) && c.Key != SystemFields.IsChanged)
                .ToDictionary(c => c.Key, c => c.Value);
        }

        internal static void SetPropertyInternal(this IMixin self, string name, object value)
        {
            self.GetInternalState()[name] = value;
        }

        [DebuggerDisplay("Undefined")]
        internal class UndefinedValue {}

        internal static class Value
        {
            public static readonly object Undefined = new UndefinedValue();
        }

        internal static object GetPropertyInternal(this IMixin self, string name)
        {
            object value;
            return self.GetInternalState().TryGetValue(name, out value) ? value : Value.Undefined;
        }

        private static bool StateChanging(object self, string name, object value)
        {
            var notifyStateChange = self as INotifyStateChange;
            if (notifyStateChange != null)
            {
                StateChanging(notifyStateChange, name, value);
            }
            var readOnly = self as IReadOnly;
            if (readOnly != null && readOnly.IsReadOnly)
            {
                return false;
            }
            return true;
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