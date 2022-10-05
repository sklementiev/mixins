﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mixins
{
    /// <summary>
    /// NotifyStateChange mixin
    /// mixins can aggregate/extend standart interfaces
    /// </summary>
    public interface INotifyStateChange : IMixin, INotifyPropertyChanging, INotifyPropertyChanged {}

    public static partial class Extensions
    {
        private static void StateChanging(INotifyStateChange self, string name, object value)
        {
            self.RaisePropertyChanging(name);
        }

        private static void StateChanged(INotifyStateChange self, string name, object value)
        {
            self.RaisePropertyChanged(name);
            var tracking = (self as IChangeTracking);
            if (tracking != null) tracking.TrackChanges(name, value);
        }

        public static void OnPropertyChanged<T, T1>(this T self, Expression<Func<T, T1>> property, Action<T1> action)
            where T : INotifyStateChange
        {
            var propertyName = PropertyName.For(property);
            self.HookToPropertyChangedEvent(propertyName, action);
        }

        public static void OnPropertyChanged<T>(this INotifyStateChange self, Expression<Func<T>> property,
            Action<T> action)
        {
            var propertyName = PropertyName.For(property);
            self.HookToPropertyChangedEvent(propertyName, action);
        }

        private static void HookToPropertyChangedEvent<T>(this INotifyStateChange self, string propertyName,
            Action<T> action)
        {
            self.PropertyChanged +=
                (sender, args) =>
                {
                    if (string.CompareOrdinal(args.PropertyName, propertyName) == 0)
                        action((T) self.GetProperty(propertyName));
                };
        }

        public static void NotifyOnChange<T>(this INotifyStateChange self, string dependentProperty,
            params string[] dependsOn)
        {
            foreach (var dependsOnPropName in dependsOn)
            {
                self.HookToPropertyChangedEvent<T>(dependsOnPropName, c => self.RaisePropertyChanged(dependentProperty));
            }
        }

        public static void NotifyOnChange<T>(this INotifyStateChange self, Expression<Func<T>> dependentProperty,
            params Expression<Func<object>>[] dependsOn)
        {
            var propertyName = PropertyName.For(dependentProperty);
            var dependsOnProps = dependsOn.Select(PropertyName.For).ToArray();
            self.NotifyOnChange<T>(propertyName, dependsOnProps);
        }

        public static void RaisePropertyChanged(this INotifyStateChange self, Expression<Func<object>> property)
        {
            var name = PropertyName.For(property);
            RaisePropertyChangedEventInternal(self, new PropertyChangedEventArgs(name), "PropertyChanged");
        }

        public static void RaisePropertyChanged(this INotifyStateChange self, string propertyName)
        {
            RaisePropertyChangedEventInternal(self, new PropertyChangedEventArgs(propertyName), "PropertyChanged");
        }

        public static void RaisePropertyChanging(this INotifyStateChange self, string propertyName)
        {
            RaisePropertyChangedEventInternal(self, new PropertyChangingEventArgs(propertyName), "PropertyChanging");
        }

        private static void RaisePropertyChangedEventInternal(INotifyPropertyChanged bindableObject, EventArgs eventArgs,
            string eventName)
        {
            if (bindableObject == null) return;

            // get the internal eventDelegate
            var bindableObjectType = bindableObject.GetType();

            // search the base type, which contains the PropertyChanged event field.
            FieldInfo propChangedFieldInfo = null;
            while (bindableObjectType != null)
            {
                propChangedFieldInfo = bindableObjectType.GetField(eventName,
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (propChangedFieldInfo != null) break;
                bindableObjectType = bindableObjectType.BaseType;
            }
            if (propChangedFieldInfo == null) return;

            // get prop changed event field value
            var fieldValue = propChangedFieldInfo.GetValue(bindableObject);
            if (fieldValue == null) return;

            var eventDelegate = fieldValue as MulticastDelegate;
            if (eventDelegate == null) return;

            // get invocation list
            var delegates = eventDelegate.GetInvocationList();

            // invoke each delegate
            foreach (var propertyChangedDelegate in delegates)
            {
                propertyChangedDelegate.Method.Invoke(propertyChangedDelegate.Target,
                    new object[] {bindableObject, eventArgs});
            }
        }
    }
}