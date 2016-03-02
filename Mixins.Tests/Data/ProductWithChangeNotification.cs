using System.ComponentModel;

namespace Mixins.Tests.Data
{
    public class ProductWithChangeNotification : Product, INotifyStateChange
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}