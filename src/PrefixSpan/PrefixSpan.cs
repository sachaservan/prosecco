/*************************************************************************************************
* Implementation of the PrefixSpan algorithm                                                     *
*                                                                                                *
*   Algorithm conference paper:                                                                  *
*   http://hanj.cs.illinois.edu/pdf/tkde04_spgjn.pdf                                             *
*                                                                                                *
*   Useful slides:                                                                               *
*   http://webdocs.cs.ualberta.ca/~zaiane/courses/cmput695-04/slides/PrefixSpan-Wojciech.pdf     *
*                                                                                                *
*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public class PrefixSpan {
    
    // database of sequences
    protected List<Sequence> _sequences;

    // mapping of items to their frequency (support)
    protected Dictionary<Item, int> _fDict;

    // mapping of items to sequences
    protected Dictionary<Item, List<PseudoSequence>> _projections;
    
    // mapping of items to their support
    protected Dictionary<Item, int> _itemSupportMap;

    // minimum number of occurrences that an 
    // item set must have in order to have minSupport
    protected int _minCount;
    protected double _minSupport;

    // longest sequence length allowed
    protected int _maxSequenceLength;
    protected int _nextPseudoSequenceIndex;

    protected void reset()
    {
        //_fDict = null;
        _nextPseudoSequenceIndex = 0;
        _sequences = new List<Sequence>();
        _projections = new Dictionary<Item, List<PseudoSequence>>(Item.EqComp);    
    }

    public PrefixSpan() 
    {
        _fDict = null;
        _sequences = new List<Sequence>();
        _projections = new Dictionary<Item, List<PseudoSequence>>(Item.EqComp);
    }

    public PrefixSpan(List<Sequence> sequences) 
    {
        _fDict = null;
        _sequences = sequences;
        _projections = new Dictionary<Item, List<PseudoSequence>>(Item.EqComp);
    }

    /* Extracts the frequent patterns from the transactions */
    public List<Sequence> MinSupportFrequentSequences(double minSupport) 
    {
        return MinSupportFrequentSequences(minSupport, 0);
    }
    
    public List<Sequence> MinSupportFrequentSequences(double minSupport, int maxLength) 
    {    
        this._minSupport = minSupport;
        this._minCount = (int)Math.Floor(minSupport * _sequences.Count);
        this._maxSequenceLength = maxLength;
        return getFrequentSequences(false);
    }

    protected List<Sequence> getFrequentSequences(bool sorted)
    {
        buildProjections(sorted);
 
        List<Sequence> frequentSequences = new List<Sequence>();
        foreach(var item in _fDict.Keys)
        {
            // add single item sequence to frequent sequences
            Sequence sequence = new Sequence(new Transaction(item));
            sequence.Support = item.Frequency;
            frequentSequences.Add(sequence);

            // find all frequent sequences containing current item
            extractFrequentSequences(frequentSequences, sequence, 1, _projections[item]);
        }

        return frequentSequences;
    }

    protected void buildProjections(bool sorted)
    {
        // contains all items in DB mapped to a pseudo projection for the item
        // or (single item sequence)
        _projections = new Dictionary<Item, List<PseudoSequence>>(Item.EqComp);
        
        // extract the individual frequent items from the list of sequences
        _fDict = getGloballyFrequentItems(_sequences, _minCount);

        if (!sorted) {
            // sort sequence alphabetically while pruning
            // infrequent items from the sequence
            foreach(var sequence in _sequences)
                sequence.SortAndPrune(_fDict);
        }

        // process sequences
        _sequences.ForEach(addSequenceToProjection);
    }

    protected void addSequenceToProjection(Sequence sequence)
    {     
        // keeps record of encountered items in the sequence
        // to adding sequence more than once
        HashSet<Item> seen = new HashSet<Item>(Item.EqComp);

        for (int i = 0; i < sequence.NumTransactions; i++)
        {
            for(int k = 0; k < sequence[i].NumItems; k++)
            {
                Item item = sequence[i][k];

                // already processed
                if (seen.Contains(item)) 
                    continue;
                else 
                    seen.Add(item);

                var pSeq = new PseudoSequence();
                pSeq.Init(_nextPseudoSequenceIndex, i, k);

                // point to the next item
                // in the sequence
                pSeq.PointToNextItem(sequence);

                // add pseudo sequence to the list of projections
                // for the current item
                List<PseudoSequence> itemProjection;
                if (!_projections.TryGetValue(item, out itemProjection))
                {
                    itemProjection = new List<PseudoSequence>();
                    _projections.Add(item, itemProjection);
                }

                if (pSeq.IsValid)
                        itemProjection.Add(pSeq);
            }
        }

        _nextPseudoSequenceIndex++;
    }

    protected void extractFrequentSequences(List<Sequence> frequentSequences, Sequence prefix, int length, List<PseudoSequence> suffixes) 
    {
        if (_maxSequenceLength > 0 && length > _maxSequenceLength)
            return;

        // find frequent items present in suffixes
        List<Item> items = getLocallyFrequentItems(prefix, suffixes, _minCount);

        foreach(var item in items)
        {
            // build a new sequence with the item
            Sequence sequence = new Sequence(prefix);
            sequence.Support = item.Frequency;
            if (item.IsSetExtention)
            {
                var it = sequence.LastTransaction.Items;
                it.Add(item);
                sequence.LastTransaction.SetItems(it);
            }
            else
            {
                var tr = sequence.Transactions;
                tr.Add(new Transaction(item));
                sequence.SetTransactions(tr);
            }

            // add the new sequence to frequent sequences
            frequentSequences.Add(sequence);

            // generate new projection 
            List<PseudoSequence> projection = new List<PseudoSequence>();
            foreach(var pSeq in suffixes)
            {
                PseudoSequence suffix;
                if (sequence.IsPrefixOf(_fDict, _sequences[pSeq.SequenceIndex], pSeq, out suffix))
                    projection.Add(suffix);
            }
        
            // recursively find all sequences
            if (projection.Count >= _minCount)
                extractFrequentSequences(frequentSequences, sequence, length + 1, projection);
        }
    }

    protected void updateSupportMapWithSequence(Sequence sequence) {
        // keeps record of encountered items in the sequence
        // to avoid double counting the support
        HashSet<Item> seen = new HashSet<Item>(Item.EqComp);
        
        // calculate frequency of individual items
        foreach(var transaction in sequence.Transactions) 
        {
            foreach(var item in transaction.Items)
            {
                // skip
                if (seen.Contains(item))
                    continue;
                
                // only calculate count once per sequence
                seen.Add(item);

                int frequency = 0;
                if (_itemSupportMap.TryGetValue(item, out frequency)) 
                    _itemSupportMap[item] = frequency + 1;
                else 
                    _itemSupportMap.Add(item, 1);
            }
        }
    }

    protected Dictionary<Item, int> getGloballyFrequentItems(List<Sequence> sequences, int minCount) 
    {
        // item support map can be updated on the fly by the progressive algorithm
        // in which case there is no need to recompute it
        if (_itemSupportMap == null) {
            // the dictionary of items to their count
            _itemSupportMap = new Dictionary<Item, int>(Item.EqComp);

            // calculate support for each item in the batch
            // and insert it into the dictionary. 
            _sequences.ForEach(updateSupportMapWithSequence);
        }
        
        Dictionary<Item, int> fDict = new Dictionary<Item, int>(Item.EqComp);
        var fList = new List<Item>(_itemSupportMap.Keys.Count);

        // 1. prune based on item support
        foreach(Item item in _itemSupportMap.Keys) 
        {
            var frequency = _itemSupportMap[item];
            if (frequency >= minCount) {
                item.Frequency = frequency;
                fList.Add(item);
            }
        }

        // 2. sort based on support
        fList.Sort((a, b) => a.CompareTo(b));

        // 3. create a dictionary with sort index of item
        _itemSupportMap = new Dictionary<Item, int>(Item.EqComp);
        int index = 0;
        foreach(Item item in fList) 
        {
            // create pruned support map
            _itemSupportMap[item] = item.Frequency;

            // update sort index
            fDict[item] = index;
            index++;
        }
        
        return fDict;
    }

    protected List<Item> getLocallyFrequentItems(
        Sequence prefix, 
        List<PseudoSequence> suffixes, 
        int minCount) 
    {
        // the dictionary of items to their count
        Dictionary<Item, int> supportCount = new Dictionary<Item, int>(Item.StrictEqComp);
       
        // calculate support for each item in the batch
        // and insert it into the dictionary. 
        foreach(var suffix in new List<PseudoSequence>(suffixes))
        {    
            // keeps record of encountered items in the sequence
            // to avoid double counting the support
            HashSet<Item> seen = new HashSet<Item>(Item.StrictEqComp);

            var sequence = _sequences[suffix.SequenceIndex];

            // set k according to the transaction index
            int k = suffix.ItemIndex;

            // calculate frequency of individual items in the sequence
            for (int i = suffix.TransactionIndex; i < sequence.NumTransactions; i++)
            {
                // remember index of first item in suffix
                int firstItemIndex = k;

                for (; k < sequence[i].NumItems; k++)
                {
                    Item item = sequence[i][k];

                    /* if index of first item is > 0, then the item is in the same set as the 
                    last item in the prefix making it a set extention */
                    item.IsSetExtention = firstItemIndex != 0;
                    updateItemSupport(supportCount, seen, item);

                    /* handle cases where separate transaction contains both the item
                    and the last item of the prefix together forming a set extention.
                    In this case, both the item (above) and _item should be updated */
                    if ((k - 1 == firstItemIndex) && (prefix.LastTransaction.LastItem.Equals(sequence[i][k-1])) 
                        && item.IsSetExtention == false)
                    {
                        item.IsSetExtention = true;
                        updateItemSupport(supportCount, seen, item);
                    }
                }

                k = 0;
            }
        }

        var fList = new List<Item>(supportCount.Keys.Count);
        
        // prune dictionary based on item support
        foreach(var keyValuePair in supportCount) 
        {
            // prune
            if (keyValuePair.Value >= minCount) 
            {
                Item item = new Item(keyValuePair.Key.Value);
                item.Frequency = keyValuePair.Value;
                item.IsSetExtention = keyValuePair.Key.IsSetExtention;
                fList.Add(item);
            }
        }

        return fList;
    }

    private void updateItemSupport(Dictionary<Item, int> supportCount, HashSet<Item> seen, Item item)
    {
        if (!seen.Contains(item))
        {
            int support = 0;
            if (supportCount.TryGetValue(item, out support))
                supportCount[item] = support + 1;
            else 
                supportCount.Add(item, 1);

            seen.Add(item);
        }
    }
}
