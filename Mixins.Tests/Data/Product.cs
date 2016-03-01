using System.Collections.Generic;
using System.ComponentModel;

namespace Mixins.Tests.Data
{
    public class Product : IMixin
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal Price
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class CloneableProduct : Product, ICloneable
    {
    }

    public class ProductWithChangeNotification : Product, INotifyStateChange
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ProductDto : IMapper
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public decimal Price
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public string ProducedBy
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class EditableProduct : Product, IEditableObject
    {
    }

    public class ProductWithChangeTracking : Product, IChangeTracking
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsChanged
        {
            get { return this.GetValue(); }
        }
    }

    public class ProductDynamic : DynamicMixin, ICloneable
    {
    }

    public class ReadOnlyProduct : Product, IReadOnly
    {
        public bool IsReadOnly
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class Bicycle : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Wheel FrontWheel
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public Wheel RearWheel
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class MultyCycle : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Wheel> Wheels
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class Wheel : IComposite
    {
        public string Brand
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class Frame : DynamicMixin, IComposite
    {
    }

    public class DynamicBicycle : DynamicMixin, IComposite
    {
    }

    public class Whole : IComposite, IEditableObject
    {
        public Part Part
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Part> Parts
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class Part : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class VievModel : IMixin
    {
        public Item Part
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        public IEnumerable<Item> Parts
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    public class Item : IComposite
    {
        public string Name
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }
}