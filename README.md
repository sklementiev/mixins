# Mixins
Mixins implementation in C#

How often have you wanted to be able to extend class functionality without actually writing any code?
Now you can! You can add required functionality to any class just by adding an interface definition to it!
You can freely mix any generic functionality from mixins into your class.

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

As we can see this class is just marked with IMixin interface, there is no real implementation! You do not reqire to inherit from any base class! But you can, of course! The only prerequisite for mixins to work is getters and setters.
They can contain any logic but in the end must call IMixin's extension methods GetValue() and SetValue(value).



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

How easy is that? As we can see we can add any functionality we need just by adding an interface to a class!
