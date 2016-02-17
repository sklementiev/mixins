using System.Collections.Generic;
using System.Dynamic;

namespace Mixins
{
    public class DynamicMixin : DynamicObject, IMixin
    {
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.GetMembers();
        }

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