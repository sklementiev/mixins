using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mixins
{
	public interface Mixin { } 
	
	public interface MCloneable : Mixin { } // type safe version of ICloneable
	public interface MDisposable : Mixin { } // we can attach custom action on dispose
	public interface MNotifyStateChange : Mixin, INotifyPropertyChanging, INotifyPropertyChanged { } // mixin can aggregate/extend standart interfaces
	public interface MEditableObject : Mixin, IEditableObject { } // another example of implementing standart interfaces
	public interface MEquatable : Mixin, IEquatable<MEquatable> { } // we can compare mixins by value
	public interface MChangeTracking : Mixin // our version of IRevertibleChangeTracking
	{
		bool IsChanged { get; }
	}
	public interface MComposite : Mixin { } //TODO define composite structure for complex object graphs
	public interface MMapper : Mixin { } //TODO transfer data between mixins using conventions

	public static partial class Extensions
	{
		#region	Mixin

		private static readonly ConditionalWeakTable<object, Dictionary<string, object>> State = new ConditionalWeakTable<object, Dictionary<string, object>>();

		public static object GetProperty(this Mixin self, string name)
		{
			return self.GetPropertyInternal(name);
		}

		public static T GetProperty<T>(this Mixin self, Expression<Func<T>> property)
		{
			var name = PropertyName.For(property);
			var value = self.GetProperty(name);
			return value == null ? default(T) : (T) value;
		}

		public static void SetProperty(this Mixin self, string name, object value)
		{
			if(Equals(value, self.GetProperty(name))) return;
			StateChanging(self, name, value);
			self.SetPropertyInternal(name, value);
			StateChanged(self, name, value);
		}

		public static void SetProperty<T>(this Mixin self, Expression<Func<T>> property, object value)
		{
			var name = PropertyName.For(property);
			self.SetProperty(name, value);
		}

		internal static Dictionary<string, object> GetStateInternal(this Mixin self)
		{
			return State.GetOrCreateValue(self);
		}

		internal static Dictionary<string, object> GetPublicState(this Mixin self)
		{
			return self.GetStateInternal().Where(c => c.Key.First() != '!').ToDictionary(c => c.Key, c => c.Value); 
		}

		internal static void SetPropertyInternal(this Mixin self, string name, object value)
		{
			self.GetStateInternal()[name] = value;
		}

		internal static object GetPropertyInternal(this Mixin self, string name)
		{
			object value;
			self.GetStateInternal().TryGetValue(name, out value);
			return value;
		}

		// generic interceptors
		private static void StateChanging(object self, string name, object value)
		{
			var notifyStateChange = self as MNotifyStateChange;
			if (notifyStateChange != null)
			{
				StateChanging(notifyStateChange, name, value);
			}
			var changeTracking = self as MChangeTracking;
			if (changeTracking != null)
			{
				StateChanging(changeTracking, name, value);
			}
		}

		private static void StateChanged(object self, string name, object value)
		{
			var notifyStateChange = self as MNotifyStateChange;
			if (notifyStateChange != null)
			{
				StateChanged(notifyStateChange, name, value);
			}
			var changeTracking = self as MChangeTracking;
			if (changeTracking != null)
			{
				StateChanged(changeTracking, name, value);
			}
		}

		public static void DumpState(this Mixin self)
		{
			var properties = State.GetOrCreateValue(self);
			Console.WriteLine("=========================================================================================================");
			foreach (var propertyName in properties.Keys)
			{
				Console.WriteLine(string.Format("{0,-50}{1}", propertyName, properties[propertyName]));
			}
			Console.WriteLine("=========================================================================================================");
		}

		#endregion

		#region	MNotifyStateChange
		private static void StateChanging(MNotifyStateChange self, string name, object value)
		{
			self.RaisePropertyChanging(name);
		}

		private static void StateChanged(MNotifyStateChange self, string name, object value)
		{
			self.RaisePropertyChanged(name);
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

		#endregion

		#region	MCloneable

		// simple implementation of shallow clone
		public static T Clone<T>(this T self) where T : Mixin
		{
			var properties = State.GetOrCreateValue(self);
			var clonedProperties = properties.Keys.ToDictionary(key => key, key => properties[key]);
			var clone = Activator.CreateInstance(self.GetType());
			State.Add(clone, clonedProperties);
			return (T)clone;
		}

		#endregion

		#region	MEditableObject

		private const string TemporaryState = "!temp";
		
		public static void BeginEdit(this MEditableObject self)
		{
			// store current state to temporary storage
			var state = State.GetOrCreateValue(self);
			object temp;
			if(state.TryGetValue(TemporaryState, out temp)) return; // idempotent
			var clone = self.Clone<Mixin>();
			State.GetOrCreateValue(self)[TemporaryState] = clone;
		}

		public static void EndEdit(this MEditableObject self)
		{
			// accept current state, discard old state
			var state = State.GetOrCreateValue(self);
			state.Remove(TemporaryState);
		}

		public static void CancelEdit(this MEditableObject self)
		{
			// restore state from temporary storage, discard temporary state
			var properties = State.GetOrCreateValue(self);
			object clone;
			if (!properties.TryGetValue(TemporaryState, out clone)) return; // idempotent
			lock (self)
			{
				State.Remove(self);
				State.Add(self, State.GetOrCreateValue(clone));
				State.Remove(clone);
			}
		}

		#endregion

		#region	MEquatable

		public static bool Equals<T>(this MEquatable self, T other) where T : MEquatable
		{
			var properties = self.GetPublicState();
			var otherProperties = other.GetPublicState(); 
			return properties.SequenceEqual(otherProperties);
		}

		#endregion

		#region	MDisposable

		internal class Lifetime<T>
		{
			private Action<T> _action;
			private readonly T _self;

			public Lifetime(Action<T> action, T self)
			{
				_action = action;
				_self = self;
			}

			public void Reset(Action<T> action)
			{
				_action = action;
			}

			~Lifetime()
			{
				if (_action != null) _action(_self);
			}
		}

		private const string LifetimeState = "!lifetime";

		public static void OnDispose<T>(this T self, Action<T> action) where T : MDisposable 
		{
			var state = State.GetOrCreateValue(self);
			object old;
			if(state.TryGetValue(LifetimeState, out old))
			{
				((Lifetime<T>)old).Reset(action); // reset old action if any
			}
			else
			{
				state[LifetimeState] = new Lifetime<T>(action, self);	
			}
		}

		#endregion

		#region	MChangeTracking

		private const string IsTrackingChanges = "!isTrackingChanges";
		private const string Changes = "!changes";
		private const string IsChanged = "IsChanged";

		public static void StartTrackingChanges(this MChangeTracking self)
		{
			self.SetPropertyInternal(IsTrackingChanges, true);
			self.SetProperty(IsChanged, false);
			self.SetPropertyInternal(Changes, new Dictionary<string, Change>());
		}

		public static void AcceptChanges(this MChangeTracking self)
		{
			if (self.DontTrackChanges()) return;
			ClearTrackingState(self);
		}

		public static void RejectChanges(this MChangeTracking self)
		{
			if (self.DontTrackChanges()) return;
			var changes = ((Dictionary<string, Change>)self.GetPropertyInternal(Changes))
				.Select(c => new { Property = c.Key, c.Value.OldValue }).ToArray();
			foreach (var change in changes)
			{
				self.SetProperty(change.Property, change.OldValue);
			}
			self.SetProperty(IsChanged, false);
			ClearTrackingState(self);
		}

		private static void ClearTrackingState(MChangeTracking self)
		{
			var state = self.GetStateInternal();
			state.Remove(IsChanged);
			state.Remove(IsTrackingChanges);
			state.Remove(Changes);
		}

		public static Dictionary<string, Change> GetChanges(this MChangeTracking self)
		{
			return (Dictionary<string, Change>)self.GetProperty(Changes);
		}

		private static bool DontTrackChanges(this MChangeTracking self)
		{
			var isTrackingChanges = self.GetProperty(IsTrackingChanges);
			return (isTrackingChanges == null || !(bool) isTrackingChanges);
		}

		private static void StateChanging(MChangeTracking self, string name, object value)
		{
			if (self.DontTrackChanges() || name == IsChanged) return;
			var changes = (Dictionary<string, Change>)self.GetProperty(Changes);
			if(changes.ContainsKey(name)) return;
			changes.Add(name, new Change { OldValue = self.GetProperty(name) });
		}

		private static void StateChanged(MChangeTracking self, string name, object value)
		{
			if (self.DontTrackChanges() || name == IsChanged) return;
			var changes = (Dictionary<string, Change>)self.GetProperty(Changes);
			var change = changes[name];
			if (Equals(change.OldValue, value))
			{
				changes.Remove(name);
				if (changes.Count == 0) self.SetProperty(IsChanged, false); // reset if all changes are reverted
				return;
			}
			change.NewValue = value;
			self.SetProperty(IsChanged, true); 
		}

		#endregion
	}

	public class Change
	{
		public object OldValue { get; set; }
		public object NewValue { get; set; }
	}

	#region	Utils
	
	/// <summary>
	/// Gets property name using lambda expressions.
	/// </summary>
	public static class PropertyName
	{
		public static string For<T, T1>(Expression<Func<T, T1>> expression)
		{
			var body = expression.Body;
			return GetMemberName(body);
		}
		
		public static string For<T>(Expression<Func<T>> expression)
		{
			var body = expression.Body;
			return GetMemberName(body);
		}

		public static string GetMemberName(Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
				{
					return GetMemberName(memberExpression.Expression) + "." + memberExpression.Member.Name;
				}
				return memberExpression.Member.Name;
			}

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				if (unaryExpression.NodeType != ExpressionType.Convert)
					throw new Exception(string.Format("Cannot interpret member from {0}", expression));

				return GetMemberName(unaryExpression.Operand);
			}

			throw new Exception(string.Format("Could not determine member from {0}", expression));
		}
	}

	#endregion	
}
