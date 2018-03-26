/*************************************************************************************************
* Implementation of the ProSecCo algorithm                                                    *
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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

public class ProSecCo : PrefixSpan 
{
    
    // stores information on the transactions
    // processed so far (e.g. number of transactions, errors, etc)
    protected Metadata _metadata;

    // list of frequent patterns 
    protected Dictionary<Sequence, int> _frequentSequences;
    protected HashSet<Sequence> _misses;
    protected Dictionary<Item, int> _fDictLastBlock;
    protected double _errorTolerance;
    protected int _dbSize;
    protected int _blockSize;
    protected int _currentBatchSize;

    // performance measurements
    Stopwatch runtimeTimer = Stopwatch.StartNew();

    public long LastBlockPrefixSpanRuntime = 0;
    public long LastBlockSubsequenceMatchingRuntime = 0;
    public long LastBlockPruningRuntime = 0;


    public ProSecCo(double minSupport, bool topK, double errorTolerance, int blockSize, int dbSize) 
        : base()
    {
        _minSupport = minSupport;
        _errorTolerance = errorTolerance;
        _blockSize = blockSize;
        _dbSize = dbSize;
        _metadata = new Metadata(errorTolerance, topK, blockSize, dbSize);
        _frequentSequences = new Dictionary<Sequence, int>(Sequence.EqComp);
        _itemSupportMap = new Dictionary<Item, int>(Item.EqComp);
    }

    public double AddSequenceBatchToSampleWindow(List<Sequence> batch) 
    {
        for (int i = 0; i < batch.Count; i++)
        {
            _sequences.Add(batch[i]);   
            updateSupportMapWithSequence(batch[i]);
            _metadata.UpdateWithSequence(batch[i]);
        }

                
        return _metadata.GetError(); 
    }
    
    public void ProcessSampleWindow() 
    {
        Debug.Assert(_metadata.NumTransactionsProcessed != 0);

        var sampleError = _metadata.GetError();
        _minCount = (int) Math.Floor(_metadata.NumTransactionsProcessed * (_minSupport - sampleError));
        _currentBatchSize = _metadata.NumTransactionsProcessed;
        _misses = new HashSet<Sequence>();
    }
    
    public void SetNextSequenceBatch(List<Sequence> batch)  
    {
        _itemSupportMap = new Dictionary<Item, int>(Item.EqComp);
        _currentBatchSize = batch.Count;

        _sequences = new List<Sequence>(batch);

        for (int i = 0; i < batch.Count; i ++)
        {
            // update metadata on data (before pruning)
            var sequence = batch[i];
            _metadata.UpdateWithSequence(sequence);
            updateSupportMapWithSequenceAndPrune(sequence, _fDictLastBlock);
        }
    }
    
    public Tuple<List<Sequence>, double> FrequentSequences(double minSupport) 
    {  
        runtimeTimer.Restart();

        _minSupport = minSupport;
        
        if (_metadata.NumTransactionsProcessed == 0) 
            return new Tuple<List<Sequence>, double>(new List<Sequence>(), 0.0);

        double error = _metadata.GetError();        
        double adjustedMinSupport = (_minSupport - error);

        // overall min support for all sequences seen so far
        var minCountGlobal = (int) Math.Floor(_metadata.NumTransactionsProcessed * adjustedMinSupport);

        // min support for the current block
        _minCount = (int) Math.Floor(_currentBatchSize * adjustedMinSupport);

        // everything which was frequent up until this point but not frequent 
        // in the current block
       _misses = new HashSet<Sequence>(_frequentSequences.Keys, Sequence.EqComp);

        bool firstBatch = _frequentSequences.Count == 0;
        
        var blockSequences = getFrequentSequences(!firstBatch);

        LastBlockPrefixSpanRuntime = runtimeTimer.ElapsedMilliseconds;
        runtimeTimer.Restart();

        foreach(var sequence in blockSequences)
        {
            _misses.Remove(sequence);

            int support;
            bool exists = _frequentSequences.TryGetValue(sequence, out support);
            if (exists || firstBatch) {
                _frequentSequences[sequence] = sequence.Support + support;
            }
        }

        // find the exact support of sequences which did not appear frequent
        // in the current batch
        foreach(var candidate in _misses) 
        {               
            foreach(var sequence in _sequences)
            {
                if (candidate.IsSubSequenceOf(sequence))
                    _frequentSequences[candidate]++;  
            }
        }

        LastBlockSubsequenceMatchingRuntime = runtimeTimer.ElapsedMilliseconds;
        runtimeTimer.Restart();

        _fDictLastBlock = _fDict;

        // if using the accumulate algorithm, prune based on global error, otherwise, prune based on batch error
        List<Sequence> sequences = extractAndPruneFrequentSequences(minCountGlobal, firstBatch);

        // reset the PrefixSpan projections
        reset();
   
        LastBlockPruningRuntime = runtimeTimer.ElapsedMilliseconds;

        return new Tuple<List<Sequence>, double>(sequences, error);
    }

     public double GetTopKMinSupport(int k) {

        // the dictionary of items to their count
        Dictionary<Item, int> supportMap = new Dictionary<Item, int>(Item.EqComp);

        // calculate support for each item in the batch
        // and insert it into the dictionary. 
        foreach(var sequence in _sequences)
        {
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
                    if (supportMap.TryGetValue(item, out frequency)) 
                        supportMap[item] = frequency + 1;
                    else 
                        supportMap.Add(item, 1);
                }
            }
        }
        
        var fList = new List<Item>(supportMap.Keys);
        for (var i = fList.Count - 1; i >= 0; i--) 
        {
            fList[i].Frequency = supportMap[fList[i]];
        }

        fList.Sort((a, b) => a.CompareTo(b));
    
        return fList[k].Frequency / (double)_metadata.NumTransactionsProcessed;
    }

    private List<Sequence> extractAndPruneFrequentSequences(int minCount, bool firstBatch) 
    {
        List<Sequence> frequentSequences = new List<Sequence>(_frequentSequences.Count);

        _maxSequenceLength = 0;
        foreach (var sequence in new List<Sequence>(_frequentSequences.Keys))
        {
            int support;
            if ((_frequentSequences.TryGetValue(sequence, out support) && support >= minCount))
            {
                if (_maxSequenceLength < sequence.NumTransactions) 
                    _maxSequenceLength = sequence.NumTransactions;

                sequence.Support = support;
                frequentSequences.Add(sequence);
            }
            else
            {
                _frequentSequences.Remove(sequence);
            }
        }

        return frequentSequences;
    }
    
}
