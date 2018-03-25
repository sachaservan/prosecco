using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using Newtonsoft.Json;

public class SequenceEqualityComparer : IEqualityComparer<Sequence>
{
    public bool Equals(Sequence s1, Sequence s2)
    {
        return s1.Equals(s2);
    }

    public int GetHashCode(Sequence s)
    {
        return s.GetHashCode();
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class Sequence : IComparable<Sequence> 
{
    static public SequenceEqualityComparer EqComp = new SequenceEqualityComparer();
    protected List<Transaction> _transactions;
    protected int _hashCode = 0;   
    protected int _numItems = 0;

    public static int SequenceSorter(Sequence a, Sequence b) 
    {
        var ret = b.Support.CompareTo(a.Support);

        if (ret == 0) 
            ret = a.ToString().CompareTo(b.ToString());

        if (ret == 0)
            ret = b.GetHashCode().CompareTo(a.GetHashCode());

        return ret;    
    }


    public int NumItemsInSequence
    {
        get
        {
            if (_numItems == 0)
            {
                foreach(var trans in Transactions)
                {
                    _numItems += trans.NumItems;
                }
            }

            return _numItems;
        }
    } 
    
    [JsonProperty]
    public List<Transaction> Transactions
    {
        get { return _transactions; }
        set { _transactions = value; }
    }

    public Transaction this[int i]
    {
        get { return _transactions[i]; }
        set { _transactions[i] = value; }
    }
    
    [JsonProperty]
    public int Support;
    
    public int NumTransactions 
    {
        get { return this._transactions.Count; }
    }

    public bool IsEmpty
    {
        get { return this._transactions.Count == 0; }
    }

    public Transaction LastTransaction
    {
        get { return this._transactions[_transactions.Count - 1]; }
    }

    public Sequence() 
    {
       this._transactions = new List<Transaction>();
       Support = 0;
       _hashCode = 0;
    }

    public Sequence(Transaction transaction) 
    {
        this._transactions = new List<Transaction>(1);
        _transactions.Add(new Transaction(transaction));
       _hashCode = 0;
    }

    public Sequence(List<Transaction> transactions) 
    {
        this._transactions = new List<Transaction>(transactions);
       _hashCode = 0;
    }

    public Sequence(Sequence s) 
    {
       this._transactions = new List<Transaction>(s.NumTransactions);
       foreach(var itemSet in s._transactions) {
           this._transactions.Add(new Transaction(itemSet));
       }

       this._transactions.TrimExcess();

       _hashCode = 0;
       Support = s.Support;
    }

    public void SetTransactions(List<Transaction> transactions)
    {
        _transactions = transactions;
        _hashCode = 0;
    }

    public bool IsSubSequenceOf(Sequence supersequence) 
    {
        if (this.IsEmpty)
            return false;

        if (supersequence.IsEmpty)
            return false;

        if (supersequence.NumTransactions < this.NumTransactions)
            return false; 

        if (supersequence.NumItemsInSequence < this.NumItemsInSequence)
            return false; 

        int index = 0;
        bool match = false;

        for (var i = 0; i < supersequence.NumTransactions; i++) 
        {
            bool subset = this.Transactions[index].IsSubsetOf(supersequence[i]);
           
            if (!match && subset)
            {
                index = 1;
                match = true;
            } 
            else if (subset) 
            {
                index++;
            } 
    
            if (match && index >= this.NumTransactions)
                return true;

            if (this.NumTransactions - index > supersequence.NumTransactions - i)
                return false;
        }

        return false;
    }
    
    public bool IsPrefixOf(Dictionary<Item, int> fDict, Sequence seq, PseudoSequence pSeq, out PseudoSequence suffix) 
    {
        suffix = new PseudoSequence();

        if (seq.IsEmpty)
            return false;

        if (seq.NumTransactions - pSeq.TransactionIndex < 0)
            return false;
            
        // current transaction index in prefix
        var prefixTransactionIndex = 0;

        // current item index in current prefix transaction
        var prefixItemIndex = 0;

        // current transaction index in suffix
        var suffixTransactionIndex = pSeq.TransactionIndex;

        // current item index in current suffix transaction
        var suffixItemIndex = pSeq.ItemIndex;
        
        // whether or not only last item needs to be matched to form a prefix
        // e.g. prefix=<(abc)> suffix=<(_c)> only item c needs to be checked
        // to validate new prefix
        if (pSeq.ItemIndex != 0 || this.LastTransaction.NumItems == 1) 
        {
            // only check last item since prefix already established
            // from previous rounds
            prefixTransactionIndex = this.NumTransactions - 1;
            prefixItemIndex = this[prefixTransactionIndex].NumItems - 1;

            if (pSeq.ItemIndex != 0 && this.LastTransaction.NumItems == 1)
            {
                suffixTransactionIndex++;
                suffixItemIndex = 0;
            }
        }
        
        for (int i = suffixTransactionIndex; i < seq.NumTransactions; i++)
        {
            int k = (i == suffixTransactionIndex) ? suffixItemIndex : 0;
            for (; k < seq[i].NumItems; k++)
            {
                // if the current prefix item does not equal suffix item at [i, k]
                if (!this[prefixTransactionIndex][prefixItemIndex].Equals(seq[i][k]))
                {
                    // to form a prefix, all items in the last transaction of the prefix 
                    // must be alphabetically AFTER all previous items in the transaction.
                    // The sorting step takes care of that, however, need to check for cases
                    // where two items have the same support making their ordering arbitrary

                    int support;
                    if (pSeq.ItemIndex == 0 || (fDict.TryGetValue(seq[i][k], out support) && support == fDict[this[prefixTransactionIndex][prefixItemIndex]]))
                    {
                        // items have same support, continue
                        continue;
                    }

                    // prefix not found in this transaction
                    break;
                }

                // items matched, continue to next item in prefix (if any)
                prefixItemIndex++;

                // no more items in current prefix transaction
                // continue to next transaction (if any)
                if (prefixItemIndex >= this[prefixTransactionIndex].NumItems)
                {
                    // move to next transaction in prefix
                    prefixTransactionIndex++;
                    prefixItemIndex = 0;

                    // no more transactions, DONE!
                    if (prefixTransactionIndex >= this.NumTransactions)
                    {
                        // add the suffix pseudo sequence
                        suffix.Init(pSeq.SequenceIndex, i, k);
                        suffix.PointToNextItem(seq);

                        return suffix.IsValid;
                    }
                }
            }
            
            // reset the prefix transaction in the case that
            // the transaction contains more than 1 item
            // making a full transaction match necessary
            if (this[prefixTransactionIndex].NumItems != 1)
            {
                prefixTransactionIndex = 0;
                prefixItemIndex = 0;
            }
        }

        return false;
    }

    // removes items that are not in fList (frequent items list)
    // and sorts remaining items according to their order in fList
    public void SortAndPrune(Dictionary<Item, int> fDict) 
    {
        for (int i = NumTransactions - 1; i >= 0; i--)
        {
            this[i].SortAndPrune(fDict);
            if (this[i].IsEmpty)
                this._transactions.RemoveAt(i);
        }
    }

    public void JustPrune(Dictionary<Item, int> fDict) 
    {
        for (int i = NumTransactions - 1; i >= 0; i--)
        {
            this[i].JustPrune(fDict);
            if (this[i].IsEmpty)
                this._transactions.RemoveAt(i);
        }
    }


    /* OVERRIDES & INTERFACES */
    public int CompareTo(Sequence other) 
    {
        return other.NumTransactions.CompareTo(NumTransactions); 
    }

    public override bool Equals (object obj)
    {
        if (obj == null || !(obj is Sequence))
            return false;
            
        if (NumTransactions != ((Sequence) obj).NumTransactions)
            return false;

        if (NumItemsInSequence != ((Sequence) obj).NumItemsInSequence)
            return false;

  
        int i = 0;
        foreach(var transactionOther in ((Sequence) obj)._transactions)
        {
            if (!transactionOther.Equals(this.Transactions[i]))
            {
                return false;
            }
        
            i++;
        }

        return true;
    }

    public override int GetHashCode()
    {
        if (_hashCode != 0)
            return _hashCode;

        _hashCode = 0;
        foreach(var itemSet in _transactions)
        {
            _hashCode = _hashCode ^ itemSet.GetHashCode();
        }
        
        return _hashCode;
    }

    public override string ToString()
    {
        string description = "<";
        foreach(var itemSet in _transactions) {
            if (itemSet.NumItems == 1)
                description += itemSet.ToString() + " ";
            else
                description +="{" + itemSet.ToString() + "} ";
        }

        description = description.Substring(0, description.Length - 1);
        
        return description + ">" + (Support == 0 ? "" : " (" + Support + ")");
    }
}
