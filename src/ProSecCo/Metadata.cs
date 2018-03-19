using System;
using System.Collections.Generic;
using System.Diagnostics;
public class Metadata {

    private double _errorTolerance;

    private int _dbSize; // ceiling of total db size over sample size

    private int _blockSize; 

    private int _numBlocks;

    private bool _topK;


    // current number of iterations (batches)
    // processed
    private int _numSequencesProcessed = 0;
    public int Iteration = 1;

    // keep a sorted set in accending order (longest transactions first)
    private List<Sequence> _longestSequences;
    
    // d-index of the transactions processed
    int _dIndex = 1;

    public int DIndex
    {
        get 
        {
            return _dIndex;
        }
    }

    public int NumTransactionsProcessed 
    {
        get 
        {
            return _numSequencesProcessed;
        }
    }
    
    public Metadata(double errorTolerance, bool topK, int blockSize, int dbSize) 
    {
        this._errorTolerance = errorTolerance;
        this._dbSize = dbSize;
        this._blockSize = blockSize;
        this._numBlocks = (int)Math.Ceiling(_dbSize/(float)_blockSize);
        this._topK = topK;
        
        _longestSequences = new List<Sequence>();
    }


    /**
     * Explanation for calculating d-index bounds and pseudo code 
     * for the algorithm found here:
     * dx.doi.org/10.1145/2629586
     */
    public void UpdateWithSequence(Sequence seq) 
    {
        _numSequencesProcessed++;

        // check if the transaction should be considered
        if (seq.NumItemsInSequence > _dIndex && !_longestSequences.Contains(seq))  
        {   
            _longestSequences.Add(new Sequence(seq));
            _longestSequences.Sort((a, b) => b.NumItemsInSequence.CompareTo(a.NumItemsInSequence));

            _dIndex = 1;
            foreach(var t in _longestSequences)
            {
                if (_dIndex > t.NumItemsInSequence) 
                    break;     
                
                 _dIndex++;
            }
            
            _dIndex--;

            // update the set of transactions
            for(var i = _longestSequences.Count - 1; i >= 0; i--) 
            {
                if (i + 1 > _dIndex)
                    _longestSequences.RemoveAt(i);
                else
                    break;
            }
        } 
    }
    public double GetError() 
    {
        // iteration starts at 1
        Debug.Assert(Iteration > 0);

        if (_numSequencesProcessed >= _dbSize) {
            return 0.0;
        }

        var epsilon = Math.Sqrt(

            (_dIndex - Math.Log(_errorTolerance) + Math.Log(_numBlocks)) / (double)(2*_numSequencesProcessed)
        );

        if (double.IsInfinity(epsilon))
            return 0.0;

            
        return _topK ? 2*epsilon : epsilon;
    }
}
