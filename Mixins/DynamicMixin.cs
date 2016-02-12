using System.Dynamic;

namespace Mixins
{
    public class DynamicMixin : DynamicObject, Mixin
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.GetProperty(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.SetProperty(binder.Name, value);
            return true;
        }
    }
}
