﻿using System.ComponentModel;

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
}