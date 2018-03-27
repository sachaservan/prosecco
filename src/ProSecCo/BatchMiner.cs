using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

public delegate Task FrequentSequencesBatchResult(
    List<Sequence> frequentSequencePatterns, double error, int iteration
);

public class BatchMiner 
{
    private ProSecCo _algorithm;
    public int NumTransactionsProcessed;
    public int CurrentIteration;
    private double _minSupport;
    private bool _mineTopK;
    private bool _didGetTopKSupport;
    private int _k;

    public ProSecCo Algorithm {
        get {
            return _algorithm;
        }
    }

    Stopwatch _blockProcessingTimer = Stopwatch.StartNew();
    public long PrevBlockFileReadingTime = 0;

    public async Task ProcessCSVFile(
        string filepath, 
        double minSup, 
        int k,
        bool mineTopK,
        int sampleSize,
        int dbSize,
        double errorTolerance,
        FrequentSequencesBatchResult resultDelegate,
        long killAfter)
    {


        Stopwatch killTimer = Stopwatch.StartNew();


        this._minSupport = minSup;
        this._k = k;
        this._mineTopK = mineTopK;

        _algorithm = new ProSecCo(minSup, mineTopK, errorTolerance, sampleSize, dbSize);

        // current batch to process
        List<Sequence> batch = new List<Sequence>();
        
        var didProcessSampleBlocks = false;

        // open the CSV file 
        using (var reader = new StreamReader(File.OpenRead(filepath))) 
        {
            _blockProcessingTimer.Restart();

            while(!reader.EndOfStream) {
                // read the current line
                string line = reader.ReadLine();

                // split the line by comma (assuming CSV file)
                string [] transactions = line.Split(new string[] { " -1 " }, StringSplitOptions.None);

                List<Transaction> tr = new List<Transaction>(transactions.Length);
                for (int i = 0; i < transactions.Length - 1; i++)
                {
                    string [] items = transactions[i].Split(' ');
                    Transaction trans = new Transaction(items);
                    tr.Add(trans);
                }

                Sequence sequence = new Sequence(tr);
                batch.Add(sequence);

                if (!didProcessSampleBlocks) 
                {
                    // check if there are enough transactions 
                    if (batch.Count % sampleSize == 0) {

                        NumTransactionsProcessed += batch.Count;

                        // update the sample and get back
                        // the error for the sample
                        _blockProcessingTimer.Stop();
                        var sampleError = _algorithm.AddSequenceBatchToSampleWindow(batch);
                        _blockProcessingTimer.Start();

                        if (!_didGetTopKSupport && _mineTopK) {
                            _minSupport = _algorithm.GetTopKMinSupport(_k);
                            _didGetTopKSupport = true;
                        }

                        batch.Clear();

                        Console.WriteLine("[DEBUG]: Min support=" + _minSupport +" Error=" +  sampleError);

                        // make sure we have enough transaction so that the error > minsup 
                        if (sampleError <= _minSupport*0.75) {
                            Console.WriteLine("[DEBUG]: Starting progressive algorithm.");
                            didProcessSampleBlocks = true;
                            _blockProcessingTimer.Stop();
                            _algorithm.ProcessSampleWindow();
                            _blockProcessingTimer.Start();
                            await processBatch(batch, resultDelegate, _minSupport, false);
                        } 

                    }   
                } 
                else if (didProcessSampleBlocks && batch.Count > 0 && batch.Count % sampleSize == 0) 
                {
                    if (killAfter <= killTimer.ElapsedMilliseconds)
                        return; // kill the mining

                    NumTransactionsProcessed += batch.Count;

                    await processBatch(batch, resultDelegate, _minSupport, false);
                }
            }
        }

        NumTransactionsProcessed += batch.Count;
        if (!didProcessSampleBlocks)
        {
            _algorithm.AddSequenceBatchToSampleWindow(batch);
            batch.Clear();

            // handle small data set case 
            _algorithm.ProcessSampleWindow();
            didProcessSampleBlocks = true;

            await processBatch(batch, resultDelegate, _minSupport, true);
        } else {        
            await processBatch(batch, resultDelegate, _minSupport, true);
        }
    }

    private async Task processBatch(
        List<Sequence> batch, 
        FrequentSequencesBatchResult resultDelegate, 
        double minSupport,
        bool purge) 
    {       

        PrevBlockFileReadingTime = _blockProcessingTimer.ElapsedMilliseconds;

        CurrentIteration++;

        if (batch.Count > 0)
        {
            _algorithm.SetNextSequenceBatch(batch);
            batch.Clear();
        }

        Tuple<List<Sequence>, double> results = _algorithm.FrequentSequences(_minSupport);

        List<Sequence> sequences = results.Item1;

        if (_mineTopK && sequences.Count >= _k && purge) {
            sequences.Sort(Sequence.SequenceSorter);
            _minSupport = sequences[_k].Support / (double)NumTransactionsProcessed;
        }

        if (purge) {
            for (var i = sequences.Count - 1; i >= 0; i--) {
                if (sequences[i].Support < Math.Floor(NumTransactionsProcessed * minSupport)) {
                    sequences.RemoveAt(i);
                }
            }
        }

        await resultDelegate(sequences, results.Item2, CurrentIteration);

        _blockProcessingTimer.Restart();
    }
}
