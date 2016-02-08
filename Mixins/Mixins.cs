//
// use Microsoft BCL Portability Pack if targeting .NET 4.0
// Install-Package Microsoft.Bcl
//
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mixins
{
	public interface Mixin { }

    // mixin can aggregate/extend standart interfaces
    public interface MNotifyStateChange : Mixin, INotifyPropertyChanging, INotifyPropertyChanged { }

    // another example of implementing standart interfaces
    public interface MEditableObject : MCloneable, IEditableObject { } 

    // our version of IRevertibleChangeTracking
    public interface MChangeTracking : MNotifyStateChange 
    {
        bool IsChanged { get; }
    }

    public interface MCloneable : Mixin { } // type safe version of ICloneable
	
    public interface MDisposable : Mixin { } // we can attach custom action on dispose

    public interface MEquatable : Mixin, IEquatable<MEquatable> { } // we can compare mixins by value

    public interface MMapper : Mixin { } // transfer data between mixins using convention (same property names and data types)
	
    public interface MComposite : Mixin { } //TODO define composite structure for complex object graphs

	public static class Extensions
	{
		#region	Mixin

		private static readonly ConditionalWeakTable<object, Dictionary<string, object>> State = new ConditionalWeakTable<object, Dictionary<string, object>>();

        public static object GetProperty(this Mixin self, string name)
        {
            return self.GetPropertyInternal(name);
        }

        public static dynamic GetValue(this Mixin self, [CallerMemberName] string name = null)
        {
            return self.GetPropertyInternal(name);
        }

        public static void SetValue(this Mixin self, object value, [CallerMemberName] string name = null)
        {
            self.SetProperty(name, value);
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
			var newValue = StateChanging(self, name, value);
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

	    internal const char SystemFieldPrefix = '#';

		internal static Dictionary<string, object> GetPublicState(this Mixin self)
		{
            return self.GetStateInternal().Where(c => c.Key.First() != SystemFieldPrefix).ToDictionary(c => c.Key, c => c.Value); 
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

		#endregion

		#region	MCloneable

		// simple implementation of shallow clone
        // TODO: deep clone
		public static T Clone<T>(this T self) where T : Mixin
		{
			var properties = self.GetPublicState();
			var clonedProperties = properties.Keys.ToDictionary(key => key, key => properties[key]);
			var clone = Activator.CreateInstance(self.GetType());
		    State.Remove(clone); // ctor could store some state already
            State.Add(clone, clonedProperties);
			return (T)clone;
		}

		#endregion

		#region	MEditableObject

		private const string TemporaryState = "#clone";
		
		public static void BeginEdit(this MEditableObject self)
		{
			// store current state to temporary storage
			var state = self.GetPublicState();
			object temp;
			if(state.TryGetValue(TemporaryState, out temp)) return; // idempotent
			var clone = self.Clone<Mixin>();
			State.GetOrCreateValue(self)[TemporaryState] = clone;
		}

		public static void EndEdit(this MEditableObject self)
		{
			// accept current state, discard old state
			var state = self.GetStateInternal();
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

		private const string LifetimeState = "#lifetime";

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

        //private const string IsTrackingChanges = "#isTrackingChanges";
        //private const string Changes = "#changes";
        //private const string Shapshots = "#shapshots";
        //private const string IsChanged = "IsChanged";

        //public static void StartTrackingChanges(this MChangeTracking self)
        //{
        //    if (!self.DontTrackChanges()) return;

        //    self.SetPropertyInternal(IsTrackingChanges, true);
        //    self.SetPropertyInternal(IsChanged, false);
        //    self.SetPropertyInternal(Changes, new Dictionary<string, Change>());
        //    // todo : recursion

        //    var shapshots = new Dictionary<string, ArrayList>();
        //    self.SetPropertyInternal(Shapshots, shapshots);
            
        //    // Subscribe to all props with INotifyCollectionChanged there, make shapshot of elements
        //    var state = self.GetPublicState();
        //    foreach (var prop in state)
        //    {
        //        var incc = prop.Value as INotifyCollectionChanged;
        //        if (incc == null) continue;
        //        var name = prop.Key;
        //        var list = (ICollection)prop.Value;
        //        var trackableList = list.AsQueryable().OfType<MChangeTracking>().ToList();
        //        shapshots[name] = new ArrayList(list); // snapshots elements are the same as on real list!
        //        incc.CollectionChanged += (sender, args) => OnListChanged(sender, args, self, name);
        //        trackableList.ForEach(item =>
        //        {
        //            var mnsc = item as MNotifyStateChange;
        //            if (mnsc != null)
        //            {
        //                // todo: use only MChangeTracking interception pipeline?
        //                mnsc.PropertyChanged += (sender, args) => OnItemChanged(sender, args, self, name);
        //            }
        //            item.StartTrackingChanges();
        //        });
        //    }
        //}

        //internal static void OnItemChanged(object item, PropertyChangedEventArgs eventArgs, MChangeTracking parent, string listName)
        //{
        //    if (eventArgs.PropertyName != IsChanged) return;
        //    parent.EvaluateIsChanged();
        //    // item is changed => fire list property changed on parent
        //    var mnsc = parent as MNotifyStateChange;
        //    if (mnsc != null)
        //    {
        //        mnsc.RaisePropertyChanged(listName);
        //        mnsc.RaisePropertyChanged(IsChanged);
        //    }
        //}

        //internal static void OnListChanged(object list, NotifyCollectionChangedEventArgs eventArgs, MChangeTracking self, string propertyName)
        //{
        //    // if list differs from snapshot, raise IsChanged, raise INPC with list name
        //    var collection = list as ICollection;
        //    var shapshot = ((Dictionary<string, ArrayList>)self.GetPropertyInternal(Shapshots))[propertyName];
        //    self.SetProperty(IsChanged, !collection.EqualsTo(shapshot));
        //    (self as MNotifyStateChange).RaisePropertyChanged(propertyName); 
        //    // todo: getchanges() should calculate list diffs on request
        //}

        //public static void AcceptChanges(this MChangeTracking self)
        //{
        //    if (self.DontTrackChanges()) return;
        //    ClearTrackingState(self);
        //}

        //public static void RejectChanges(this MChangeTracking self)
        //{
        //    if (self.DontTrackChanges()) return;
        //    // TODO. Lists
        //    var changes = ((Dictionary<string, Change>)self.GetPropertyInternal(Changes))
        //        .Where(c => c.Value is ValueChange)
        //        .Select(c => new { Property = c.Key, ((ValueChange)c.Value).OldValue }).ToArray();
        //    foreach (var change in changes)
        //    {
        //        self.SetProperty(change.Property, change.OldValue);
        //    }
        //    self.SetProperty(IsChanged, false);
        //    ClearTrackingState(self);
        //}

        //private static void ClearTrackingState(MChangeTracking self)
        //{
        //    var state = self.GetStateInternal();
        //    state.Remove(IsChanged);
        //    state.Remove(IsTrackingChanges);
        //    state.Remove(Changes);
        //    state.Remove(Shapshots);
        //}

        //public static Dictionary<string, Change> GetChanges(this MChangeTracking self)
        //{
        //    var changes = (Dictionary<string, Change>)self.GetProperty(Changes);
        //    var result = new Dictionary<string, Change>(changes);
        //    var snapshots = (Dictionary<string, ArrayList>)self.GetPropertyInternal(Shapshots);
        //    foreach (var shapshot in snapshots)
        //    {
        //        var list = self.GetPropertyInternal(shapshot.Key) as ICollection;
        //        var diff = list.GetDiff(shapshot.Value);
        //        if (diff.Added.Count > 0 || diff.Removed.Count > 0 || diff.Changed.Count > 0) result.Add(shapshot.Key, diff);
        //    }

        //    return result;
        //}

        //private static bool DontTrackChanges(this MChangeTracking self)
        //{
        //    var isTrackingChanges = self.GetProperty(IsTrackingChanges);
        //    return (isTrackingChanges == null || !(bool) isTrackingChanges);
        //}

        //private static void StateChanging(MChangeTracking self, string name, object value)
        //{
        //    if (self.DontTrackChanges() || name == IsChanged) return;
        //    var changes = (Dictionary<string, Change>)self.GetProperty(Changes);
        //    if(changes.ContainsKey(name)) return;
        //    changes.Add(name, new ValueChange { OldValue = self.GetProperty(name) });
        //}

        //private static void StateChanged(MChangeTracking self, string name, object value)
        //{
        //    if (self.DontTrackChanges() || name == IsChanged) return;
        //    var changes = (Dictionary<string, Change>)self.GetProperty(Changes);
        //    var change = (ValueChange)changes[name];
        //    if (Equals(change.OldValue, value))
        //    {
        //        changes.Remove(name);
        //    }
        //    else
        //    {
        //        change.NewValue = value;
        //    }
        //    self.EvaluateIsChanged();
        //}

        //internal static void EvaluateIsChanged(this MChangeTracking self)
        //{
        //    // any lists has any changes? // todo: check added, deleted
        //    var state = self.GetPublicState();

        //    var listsChanged = false;
        //    foreach (var prop in state)
        //    {
        //        var incc = prop.Value as INotifyCollectionChanged;
        //        if (incc == null) continue;
        //        var name = prop.Key;
        //        var list = (ICollection) prop.Value;
        //        // bug: use snapshots here
        //        var trackableList = list.AsQueryable().OfType<MChangeTracking>().ToList();
        //        if(trackableList.Any(c=>c.IsChanged))
        //        {
        //            listsChanged = true;
        //            break;
        //        }
        //    }

        //    var changes = (Dictionary<string, Change>)self.GetProperty(Changes);
        //    var isChanged = changes.Count > 0 || listsChanged;
        //    self.SetProperty(IsChanged, isChanged); 
        //}

		#endregion

        #region	MMapper

        public static void MapTo(this MMapper self, Mixin destination)
        {
            var state = self.GetPublicState();
            var otherState = destination.GetPublicState();
            var sameProps = state.Select(c => c.Key).Intersect(otherState.Select(c => c.Key));
            foreach (var prop in sameProps.Where(prop => otherState[prop].GetType().IsInstanceOfType(state[prop])))
            {
                destination.SetProperty(prop, self.GetProperty(prop));
            }
        }

        #endregion

        #region	MComposite

        public static void DefineComposite(this MComposite self, Mixin destination)
        {
            // TODO
        }

	    #endregion
    }

    //public abstract class Change
    //{
    //}

    //// ? ref changes ? how to deal with ? maybe only support Mixins
    //public class ValueChange : Change
    //{
    //    public object OldValue { get; set; }
    //    public object NewValue { get; set; }
    //}

    //public class CollectionChange : Change
    //{
    //    public List<object> Added { get; set; }
    //    public List<object> Removed { get; set; }
    //    public List<object> Changed { get; set; }
    //}


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

    //public static class CollectionHelper
    //{
    //    // TODO: Compare based on IEquitable not refs
    //    public static bool EqualsTo(this ICollection left, ICollection right)
    //    {
    //        if (left.Count != right.Count) return false;

    //        var dict = new Dictionary<object, int>();

    //        foreach (var member in left)
    //        {
    //            if (!dict.ContainsKey(member)) dict[member] = 1;
    //            else dict[member]++;
    //        }

    //        foreach (var member in right)
    //        {
    //            if (!dict.ContainsKey(member)) return false;
    //            dict[member]--;
    //        }

    //        return dict.All(kvp => kvp.Value == 0);
    //    }

    //    // TODO: Compare based on IEquitable not refs
    //    public static CollectionChange GetDiff(this ICollection self, ICollection other)
    //    {
    //        var added = new List<object>();
    //        var removed = new List<object>();
    //        var changed = new List<object>();
    //        var result = new CollectionChange
    //        {
    //            Added = added,
    //            Removed = removed,
    //            Changed = changed
    //        };

    //        var dictSelf = new Dictionary<object, int>(); // item, count

    //        foreach (var item in self)
    //        {
    //            if (!dictSelf.ContainsKey(item)) dictSelf[item] = 1;
    //            else dictSelf[item]++;
    //        }

    //        var dictOther = new Dictionary<object, int>(); // item, count

    //        foreach (var item in other)
    //        {
    //            if (!dictOther.ContainsKey(item)) dictOther[item] = 1;
    //            else dictOther[item]++;
    //        }

    //        // added to self
    //        GetDiff(dictSelf, dictOther, added);

    //        // deleted from self
    //        GetDiff(dictOther, dictSelf, removed);

    //        // все что не аддед проверить на IsChanged
    //        foreach (var item in self)
    //        {
    //            if (!added.Contains(item) && item is MChangeTracking)
    //            {
    //                var mct = (MChangeTracking)item;
    //                if(mct.IsChanged) changed.Add(item);
    //            }
    //        }

    //        return result;
    //    }

    //    private static void GetDiff(Dictionary<object, int> current, Dictionary<object, int> old, List<object> diff)
    //    {
    //        foreach (var pair in current)
    //        {
    //            // not there
    //            if (!old.ContainsKey(pair.Key))
    //            {
    //                for (var i = 0; i < pair.Value; i++)
    //                {
    //                    diff.Add(pair.Key);
    //                }
    //            }
    //            // partially there
    //            else
    //            {
    //                var newItems = pair.Value - old[pair.Key];
    //                if (newItems > 0)
    //                {
    //                    for (var i = 0; i < newItems; i++)
    //                    {
    //                        diff.Add(pair.Key);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}


	#endregion	
}
