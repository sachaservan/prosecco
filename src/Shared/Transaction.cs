using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using Newtonsoft.Json;

public class TransactionEqualityComparer : IEqualityComparer<Transaction>
{
    public bool Equals(Transaction t1, Transaction t2)
    {
        if(t1.NumItems == t2.NumItems)
            return t1.IsSubsetOf(t2);
        else
            return false;
    }

    public int GetHashCode(Transaction t)
    {
        return t.GetHashCode();
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class Transaction : IComparable<Transaction> 
{
    static public TransactionEqualityComparer EqComp = new TransactionEqualityComparer();
    protected List<Item> _items;

    protected int _hashCode = 0;    

    public Item this[int i]
    {
        get { return _items[i]; }
        set { _items[i] = value; }
    }

    [JsonProperty]
    public List<Item> Items 
    {
        get { return _items; }
        set { _items = value; }
    }

    public bool IsEmpty 
    {
        get { return this._items.Count == 0; }
    }
    public int NumItems 
    {
        get { return this._items.Count; }
    }

    public Item LastItem
    {
        get { return _items.Count > 0 ? _items[_items.Count - 1] : null; }
    }

    public Transaction() 
    {
       this._items = new List<Item>();
    }
    public Transaction(Item item) 
    {
        this._items = new List<Item>(1);
        _items.Add(item);
    }

    public Transaction(List<Item> items) 
    {
       this._items = new List<Item>(items);     
    }

    public Transaction(string [] items) 
    {
       this._items = new List<Item>(items.Length);
       foreach (string item in items) 
            this._items.Add(new Item(int.Parse(item)));   
    }

    public Transaction(Transaction t) 
    {
       this._items = new List<Item>(t.NumItems);
       foreach(var item in t._items) {
           this._items.Add(item);
       }
    }

    public void SetItems(List<Item> items)
    {
        _items = new List<Item>(items);
        _hashCode = 0;
    }

    public bool IsSubsetOf(Transaction superset) 
    {
        if (superset.IsEmpty)
            return false;

        if (superset.NumItems < NumItems)
            return false;
        
        var found = false;
        foreach(var item in _items)
        {
            found = false;
            foreach(var superItem in superset._items)
            {
                if (item.Equals(superItem))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                break;
        }

        return found;
    }


    // removes items that are not in fList (frequent items list)
    // and sorts remaining items according to their order in fList
    public void SortAndPrune(Dictionary<Item, int> fDict) 
    {
        for (int index = NumItems - 1; index >= 0; index--)
        {
            if (!fDict.ContainsKey(_items[index]))
            {
                _items.RemoveAt(index);
                continue;
            }

            var item = _items[index];
            item.SortIndex = fDict[item];
        }

        _items.TrimExcess();
        _items.Sort((a, b) => a.SortIndex.CompareTo(b.SortIndex));
    }

    public void JustPrune(Dictionary<Item, int> fDict) 
    {
        for (int index = NumItems - 1; index >= 0; index--)
        {
            if (!fDict.ContainsKey(_items[index]))
            {
                _items.RemoveAt(index);
                continue;
            }
        }

        _items.TrimExcess();
    }

    /* OVERRIDES & INTERFACES */
    
    public int CompareTo(Transaction other) 
    {
        return other.NumItems.CompareTo(NumItems); 
    }

    public override bool Equals (object obj)
    {
        if (obj == null || !(obj is Transaction))
            return false;
            
        if (NumItems != ((Transaction) obj).NumItems)
            return false;

        return this.IsSubsetOf(((Transaction) obj));
    }

    public override int GetHashCode()
    {
        if (_hashCode != 0)
            return _hashCode;

        _hashCode = 0;
        foreach(var item in _items)
        {
            _hashCode = _hashCode ^ item.Value.GetHashCode();
        }
        
        return _hashCode;
    }

    public override string ToString()
    {
        string description = "";
        foreach(var item in _items) {
            description += item.Value.ToString() + " ";
        }
        return description.Substring(0, description.Length - 1);
    }
}
