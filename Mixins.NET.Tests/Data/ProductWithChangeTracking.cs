using System.ComponentModel;

namespace Mixins.Tests.Data
{
    public class ProductWithChangeTracking : Product, IChangeTracking
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsChanged
        {
            get { return (bool)this.GetValue(); }
        }
    }
}