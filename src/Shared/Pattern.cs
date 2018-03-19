using System.Collections.Generic;
using System;

public class Pattern : Transaction, IComparable<Pattern>
{    
    // support of the given pattern
    public int Support;
    
    public Pattern() 
        : base()
    {}
    
    public Pattern(Item item)
        : base() 
    {
        _items = new List<Item>(1);
        _items.Add(item);
    }

    public Pattern(List<Item> items)
        : base(items) 
    {}

    public Pattern(string [] items)
        : base(items) 
    {}

    public Pattern(Pattern p) 
        : base(p)
    {
        this._hashCode = 0;
        this.Support = p.Support;
    }

    public override bool Equals (object obj)
    {
        return base.Equals(obj);
    }

    public void Prepend(Item item)
    {
        Items.Insert(0, item);
        _hashCode = 0;
    }

    public int CompareTo(Pattern other) 
    {
        return other.NumItems.CompareTo(NumItems); 
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        string description = base.ToString();
        return description + " (" + Support + ")";
    }
}