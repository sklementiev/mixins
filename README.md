# Mixins [![Build status](https://ci.appveyor.com/api/projects/status/2dmaheiwr8s9phnl?svg=true)](https://ci.appveyor.com/project/sklementiev/mixins)

Mixins implementation in C#

NuGet **Install-Package Mixins.NET**

How often have you imagined to be able to extend class functionality without actually writing any code?

Now you can! You can add required functionality to any class just by adding an interface definition to it!

**No external libraries, no proxy magic, no compile-time MSIL weaving required!**

You can freely add and mix any generic functionality/behaviours from mixins into your class.

Example time!

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

As we can see this class is just marked with IMixin interface, there is no real implementation! Even interface itself is empty!

    public interface IMixin {}

You do not require to inherit from any base class! But you can, of course! The only prerequisite for mixins to work is getters and setters. They can contain any logic but in the end must call IMixin's extension methods GetValue() and SetValue(value).

Let's see what minimum functionality mixin provides

    var banana = new Product
    {
        Name = "Banana",
        Price = new decimal(2.5)
    };

    var members = banana.GetMembers();
    foreach (var member in members)
    {
        var value = banana.GetProperty(member);
        var type = banana.GetPropertyType(member);
        Console.WriteLine("{0} = {1} of type {2}", member, value, type);
    }

    var banana2 = new Product
    {
        Name = "Banana",
        Price = new decimal(2.5)
    };

    Assert.IsTrue(banana.EqualsByValue(banana2));

So every mixin can get set of properties it has, get or set a value and type of property by its name and can be compared with any other mixin by value.

Ok, it looks useful, but I want more! For example, let's say I want to clone mixins! Easy!

    public class CloneableProduct : Product, ICloneable
    {
    }

All we need to do is to mark our class with the mixin we need - ICloneable

    var banana = new CloneableProduct
    {
        Name = "Banana",
        Price = new decimal(2.5)
    };

    var banana2 = banana.Clone();

    Assert.IsTrue(banana.EqualsByValue(banana2));

How easy is that? As we can see we can add any functionality we need just by adding an interface to a class! (Mixins even support deep cloning by the way)

But I still want more! Something exciting! ) Ok, what if we suddenly want to get notifications on our object changes? Easy again!


    public class ProductWithChangeNotification : Product, INotifyStateChange
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }

Prove it!

    var banana = new ProductWithChangeNotification
    {
        Name = "Banana",
        Price = new decimal(2.5)
    };

    banana.PropertyChanged += (sender, args) =>
    {
        Console.WriteLine("{0} changed to {1}", args.PropertyName, banana.GetProperty(args.PropertyName));
    };

    banana.Price = 3; // prints "Price changed to 3"

How cool is that?

There are so many options to create and reuse generic algorithms/behaviours when use mixins. For example - mapping. It is usual and mundane task to copy data from DTO object to ViewModel (espesially if they share the same property names) and with mixins help it's a breeze!

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

Let's transfer data to the other mixin based on the same property names and compatible types.

    var bananaDto = new ProductDto
    {
        Name = "Banana",
        Price = new decimal(2.5),
        ProducedBy = "Banana tree"
    };

    var banana = new Product();

    bananaDto.MapTo(banana);

    Assert.AreEqual(bananaDto.Name, banana.Name);
    Assert.AreEqual(bananaDto.Price, banana.Price);

Amazing, isn't it?

The other common behaviour that we can get for free with mixins is implementation of System.ComponentModel.IEditableObject interface. It allows object to accept or reject changes to its state.

    public class EditableProduct : Product, IEditableObject
    {
    }

Note that IEditableObject is a mixin that composed from two other mixins!

    public interface IEditableObject : ICloneable, IMapper {}

Let's see it in action

    var banana = new EditableProduct
    {
        Name = "Banana",
        Price = new decimal(2.5)
    };

    banana.BeginEdit();
    banana.Name = "Apple";
    banana.Price = 7;
    banana.CancelEdit();

    Assert.AreEqual("Banana", banana.Name);
    Assert.AreEqual(2.5, banana.Price);

Pretty neat, huh?
Let's look at something more complex but useful. How about a mixin that can track changes to the object state and notify consumers (usually UI controls) on the fact that it has changes to be saved. Enters IChangeTracking!

    public interface IChangeTracking : INotifyStateChange, IEditableObject
    {
        bool IsChanged { get; }
    }

Our product class will become

    public class ProductWithChangeTracking : Product, IChangeTracking
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsChanged
        {
            get { return this.GetValue(); }
        }
    }


Let's demo that

    var banana = new ProductWithChangeTracking
    {
        Name = "Banana",
        Price = new decimal(2.5)
    };

    banana.PropertyChanged += (sender, args) =>
    {
        if (args.PropertyName != "IsChanged") return;
        Console.WriteLine("{0} = {1}", args.PropertyName, banana.GetProperty(args.PropertyName));
    };

    banana.BeginEdit();
    banana.Name = "Apple";  // prints IsChanged = true
    banana.Name = "Banana"; // prints IsChanged = false
    banana.Price = 5;       // prints IsChanged = true
    banana.CancelEdit();    // prints IsChanged = false

You can run and explore WPF example project (WpfConsole) to see how it all works in real life.

There is another mixin we can showcase - IReadOnly. As name suggests we can make our instance read only when we want to!

    public class ReadOnlyProduct : Product, IReadOnly
    {
        public bool IsReadOnly
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }
    }

    var banana = new ReadOnlyProduct
    {
        Name = "Banana",
        Price = new decimal(2.5)
    };

    banana.Name = "Apple";
    Assert.AreNotEqual("Banana", banana.Name);
    
    banana.IsReadOnly = true;
    banana.Name = "Mango";
    Assert.AreEqual("Apple", banana.Name);



**The other interesting aspect is that mixins actually fully support dynamic objects as well!**

In that case we don't really care about property definitions and our code became something as trivial as this

    public class ProductDynamic : DynamicMixin, ICloneable
    {
    }

    dynamic banana = new ProductDynamic();
    banana.Name = "Banana";
    banana.Price = new decimal(2.5);
    
    ICloneable mixin = banana;
    dynamic clone = mixin.Clone();
    
    Assert.AreNotSame(banana, clone);
    Assert.AreEqual(banana.Name, clone.Name);
    Assert.AreEqual(banana.Price, clone.Price);


##The current list of Mixins

**ICloneable** -Type safe version of ICloneable, with deep clone ability. Deep clone will work only on ICloneable properties

**IReadOnly** - Makes your instance read only

**IMapper** - Transfer data between mixins using convention (same property names and compatible data types) 

**IEditableObject** - Implementation of System.ComponentModel.IEditableObject

**INotifyStateChange** - Implements System.ComponentModel.INotifyPropertyChanging and System.ComponentModel.INotifyPropertyChanged

**IChangeTracking** - Implementation of System.ComponentModel.IRevertibleChangeTracking

**IDisposable** - We can dynamically attach any custom action on object's dispose 

----------

##I would love to hear your ideas on how we can improve it, which new mixins should we create. Feel free to contribute! 
