using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mixins
{
    // NotifyStateChange mixin
    // mixins can aggregate/extend standart interfaces
    public interface MNotifyStateChange : Mixin, INotifyPropertyChanging, INotifyPropertyChanged { }

	public static partial class Extensions
	{
		private static void StateChanging(MNotifyStateChange self, string name, object value)
		{
			self.RaisePropertyChanging(name);
		}

		private static void StateChanged(MNotifyStateChange self, string name, object value)
		{
            self.RaisePropertyChanged(name);
            if (self is MChangeTracking && name != SystemFields.IsChanged && self.GetPropertyInternal(SystemFields.Shapshot) != null)
            {
                var wasChanged = self.GetPropertyInternal(SystemFields.IsChanged) as bool?;
                if (wasChanged == null)
                {
                    self.SetPropertyInternal(SystemFields.IsChanged, false); // first hit, define IsChanged
                }
                wasChanged = wasChanged ?? false;
                var shapshot = (MEquatable)self.GetPropertyInternal(SystemFields.Shapshot);
                var isChanged = !shapshot.Equals((MEquatable)self);
                
                if (wasChanged != isChanged)
                {
                    self.SetProperty(SystemFields.IsChanged, !wasChanged);
                }
            }
		}

		public static void OnPropertyChanged<T, T1>(this T self, Expression<Func<T, T1>> property, Action<T1> action) where T : MNotifyStateChange
		{
			var propertyName = PropertyName.For(property);
			self.HookToPropertyChangedEvent(propertyName, action);
		}

		public static void OnPropertyChanged<T>(this MNotifyStateChange self, Expression<Func<T>> property, Action<T> action)
		{
			var propertyName = PropertyName.For(property);
			self.HookToPropertyChangedEvent(propertyName, action);
		}

		private static void HookToPropertyChangedEvent<T>(this MNotifyStateChange self, string propertyName, Action<T> action)
		{
			self.PropertyChanged += (sender, args) => { if (string.CompareOrdinal(args.PropertyName, propertyName) == 0) action((T)self.GetProperty(propertyName)); };
		}

		public static void NotifyOnChange<T>(this MNotifyStateChange self, Expression<Func<T>> dependentProperty, params Expression<Func<object>>[] dependsOn)
		{
			var propertyName = PropertyName.For(dependentProperty);
			var dependsOnProps = dependsOn.Select(PropertyName.For);
			foreach (var dependsOnPropName in dependsOnProps)
			{
				self.HookToPropertyChangedEvent<T>(dependsOnPropName, c => self.RaisePropertyChanged(propertyName));
			}
		}

		public static void RaisePropertyChanged(this MNotifyStateChange self, Expression<Func<object>> property)
		{
			var name = PropertyName.For(property);
			RaisePropertyChangedEventInternal(self, new PropertyChangedEventArgs(name), "PropertyChanged");
		}

		public static void RaisePropertyChanged(this MNotifyStateChange self, string propertyName)
		{
            RaisePropertyChangedEventInternal(self, new PropertyChangedEventArgs(propertyName), "PropertyChanged");
		}

		public static void RaisePropertyChanging(this MNotifyStateChange self, string propertyName)
		{
			RaisePropertyChangedEventInternal(self, new PropertyChangingEventArgs(propertyName), "PropertyChanging");
		}

		private static void RaisePropertyChangedEventInternal(INotifyPropertyChanged bindableObject, EventArgs eventArgs, string eventName)
		{
			if(bindableObject == null) return;

            // get the internal eventDelegate
            var bindableObjectType = bindableObject.GetType();

			// search the base type, which contains the PropertyChanged event field.
			FieldInfo propChangedFieldInfo = null;
			while (bindableObjectType != null)
			{
				propChangedFieldInfo = bindableObjectType.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
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
				propertyChangedDelegate.Method.Invoke(propertyChangedDelegate.Target, new object[] {bindableObject, eventArgs});
			}
		}
    }
}
