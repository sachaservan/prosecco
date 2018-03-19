using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemEqualityComparer : IEqualityComparer<Item>
{
    public bool Equals(Item item1, Item item2)
    {
        return item1.Equals(item2);
    }

    public int GetHashCode(Item item)
    {
        return item.GetHashCode();
    }
}

public class StrictItemEqualityComparer : IEqualityComparer<Item>
{
    public bool Equals(Item item1, Item item2)
    {
        return item1.IsSetExtention == item2.IsSetExtention && item1.Equals(item2);
    }

    public int GetHashCode(Item item)
    {
        return item.GetHashCode() + (item.IsSetExtention ? 1 : 0);
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class Item : IComparable<Item>, IEquatable<Item>  {
    /* strict comparison takes into account IsSetExtention
    which is needed in PrefixSpan */
    static public StrictItemEqualityComparer StrictEqComp = new StrictItemEqualityComparer();
    /* regular comparison simply takes into account the value of the item */
    static public ItemEqualityComparer EqComp = new ItemEqualityComparer();
    [JsonProperty]
    public readonly int Value;
    public int SortIndex;
    [JsonProperty]
    public int Frequency;

    public bool IsSetExtention 
    {   
        get { return _isSetExtention; }
        set 
        { 
            _isSetExtention = value; 
            _hashCode = 0;
        }    
    }
    protected bool _isSetExtention = false;
    protected int _hashCode;

    public Item(int value) 
    {
        this.Value = value;
        this._hashCode = 0;
    }

    /* OVERRIDES & INTERFACES */

    public int CompareTo(Item other) 
    {
        if (this.Frequency == other.Frequency)
            return Value.CompareTo(other.Value);
        
        return other.Frequency - this.Frequency;
    }

    public bool Equals(Item other)
    {

        if (GetHashCode() != ((Item) other).GetHashCode())
            return false;

        return this.Value.Equals(other.Value);
    }

    // override object.Equals
    public override bool Equals (object other)
    {
        if (other == null || !(other is Item))
           return false;

        if (GetHashCode() != ((Item) other).GetHashCode())
            return false;

       return this.Value.Equals(((Item)other).Value);
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
         if (_hashCode == 0)
            _hashCode = Value.GetHashCode();

        return _hashCode;
    }

    // override object.ToString
    public override string ToString() 
    {
        return Value + (Frequency == 0 ? "" : ":" + Frequency);
    }
}