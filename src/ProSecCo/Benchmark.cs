
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

public static class BenchmarkTest
{

    // **************************************************
    private const int NUM_TRIALS = 5;
    private const int STAT_SAMPLE_INTERVAL = 20;
    // **************************************************

    private static string FILE_PATH;

    private static double ERROR_TOLERANCE = 0.05;

    private static int SAMPLE_SIZE = 12000;

    static Stopwatch batchStopwatch = Stopwatch.StartNew(); 
    static long totalTimeElapsed = 0;

    static List<double> batchProcessingTimes;

    static List<double> batchErrors;

    static int currentTrialNumber = 0;
    static int currentBatchNumber = 0;


    public static void StartBenchmark (string filePath, List<double> minSupports, List<int> topKs, int sampleSize, int dbSize){

        var numBlocks = (int)Math.Ceiling(dbSize/(double)sampleSize);

        shuffleDataset(filePath);
        FILE_PATH = filePath;
        SAMPLE_SIZE = sampleSize;


        
        // List<Sequence> expectedResults;   
        // Tuple<List<Sequence>, int> results = getPrefixSpanResults(filePath, 0.01, false);
        // expectedResults = results.Item1;
        // expectedResults.Sort((a,b) => b.Support.CompareTo(a.Support));
        // Console.WriteLine("[DEBUG]: Finished getting expected results");

      //  testSupportDifference(expectedResults, minSupports, SAMPLE_SIZE, results.Item2);        
        benchmarkTest(minSupports, topKs, false, SAMPLE_SIZE, numBlocks); // test with min support
      //  benchmarkTest(SAMPLE_SIZE, minSupports, topKs, true); // test with top k

        benchmarkTestPrefixSpan(minSupports); 
    }

    private static void shuffleDataset(string filepath) 
    {
        var cmdShuf = "shuf " + filepath + " > " + filepath + "1";
        var cmdShufBack = "shuf " + filepath +"1" + " > " + filepath;

        Bash(cmdShuf);
        Bash(cmdShufBack);
    }

    public static string Bash(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
        
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return result;
    }

    private static long processCSVFile(string filepath, double minSupport) 
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch

        // current batch to process
        List<Sequence> sequenceList = new List<Sequence>();

        // open the CSV file 
        var reader = new StreamReader(File.OpenRead(filepath));

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
            sequenceList.Add(sequence);
        }

        PrefixSpan algorithm = new PrefixSpan(sequenceList);
        
        List<Sequence> frequentSequences = algorithm.MinSupportFrequentSequences(minSupport);
        
        // stop the stopwatch after frequent patterns are 
        // returned
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }


    private static async Task<long> processCSVFileProgressive(string filepath, double minSupport, int k, bool mineTopK, int sampleSize, int numBlocks)
    {
        BatchMiner miner = new BatchMiner();
        batchStopwatch.Restart();
        totalTimeElapsed = 0;

        await miner.ProcessCSVFile(
            filepath,
            minSupport,
            k,
            mineTopK,
            sampleSize,
            numBlocks,
            ERROR_TOLERANCE,
            new FrequentSequencesBatchResult(onBatchResultsNoOutput),
            long.MaxValue
        );

        batchStopwatch.Stop();

        return totalTimeElapsed;
    }


    private static async Task onBatchResultsNoOutput(
        List<Sequence> frequentSequences, 
        double error,
        int iteration) 
    {          
        batchStopwatch.Stop();
        totalTimeElapsed += batchStopwatch.ElapsedMilliseconds;
       
        if (batchProcessingTimes != null && currentTrialNumber == 0)
            batchProcessingTimes.Add(batchStopwatch.ElapsedMilliseconds);
        else if (batchProcessingTimes != null) 
            batchProcessingTimes[currentBatchNumber] = batchProcessingTimes[currentBatchNumber] + batchStopwatch.ElapsedMilliseconds;

        batchStopwatch.Restart();
        await Task.Delay(0);
    }




    // TESTS
    private static async Task testSupportDifference(List<Sequence> expectedResults, double minSupport, int k, bool mineTopK, int sampleSize, int datasetSize)
    {
        batchProcessingTimes = new List<double>();
        batchErrors = new List<double>();

        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine("MIN SUPPORT: " + minSupport);
        Console.WriteLine("----------------------------------------------------");
        
        BatchMiner miner = new BatchMiner();
        List<Sequence> results = new List<Sequence>();
        List<double> supportDiffMax = new List<double>();
        List<double> supportDiffMedian = new List<double>();
        List<double> falseNegatives = new List<double>();
        List<double> falsePositives = new List<double>();

        int minCount = 0;

        for (var i = 0; i < NUM_TRIALS; i++) 
        {
            currentTrialNumber = i;
            currentBatchNumber = 0;

            batchStopwatch.Restart();

            // test difference between PrefixSpan and PrefixSpan supports
            miner = new BatchMiner();
            await miner.ProcessCSVFile(
                FILE_PATH,
                minSupport,
                k,
                mineTopK,
                sampleSize,
                (int)Math.Ceiling(datasetSize/(double) sampleSize),
                ERROR_TOLERANCE,
                async (List<Sequence> frequentSequences, double error, int iteration) => {
                    await onBatchResultsNoOutput(frequentSequences, error, iteration);
                    results = frequentSequences;
                    
                        minCount = (int)Math.Floor(datasetSize * minSupport);
                        Tuple<double, double, int, int> supportDiffs = getSupportDifference(expectedResults, results, datasetSize, miner.NumTransactionsProcessed, minCount);

                    if (i == 0)
                    {
                        batchErrors.Add(error);
                        supportDiffMedian.Add(supportDiffs.Item1);
                        supportDiffMax.Add(supportDiffs.Item2);
                        falsePositives.Add(supportDiffs.Item3);
                        falseNegatives.Add(supportDiffs.Item4);
                    }
                    else
                    {
                        batchErrors[currentBatchNumber]       += (error);
                        supportDiffMedian[currentBatchNumber] += (supportDiffs.Item1);
                        supportDiffMax[currentBatchNumber]    += (supportDiffs.Item2);
                        falsePositives[currentBatchNumber]    += (supportDiffs.Item3);
                        falseNegatives[currentBatchNumber]    += (supportDiffs.Item4);    
                    }

                    currentBatchNumber++;

                },
                killAfter: long.MaxValue
            );
        }

        minCount = (int)Math.Floor(miner.NumTransactionsProcessed * minSupport);


        Console.WriteLine("Min support:               " + minSupport);

        Tuple<double, double, int, int> result = getSupportDifference(expectedResults, results, datasetSize, miner.NumTransactionsProcessed, minCount);
        Console.WriteLine("Median support diff:       " + result.Item1 / (double)miner.NumTransactionsProcessed);
        Console.WriteLine("Max support diff:          " + result.Item2 / (double)miner.NumTransactionsProcessed);
        Console.WriteLine("Total Results:             " + results.Count);
        Console.WriteLine("False positives:           " + result.Item3);
        Console.WriteLine("False negatives:           " + result.Item4);
        Console.WriteLine("Sample size:     " + sampleSize);
        Console.WriteLine("----------------------------------------------------");


        var divBy = (double)(NUM_TRIALS*STAT_SAMPLE_INTERVAL);

        Console.Write("PROCESSING TIMES: \n[");
        int index = 0;
        int batch = 0;
        double currentVal = 0.0;
        foreach (var time in batchProcessingTimes) 
        {
            currentVal += time;

            if (batch == STAT_SAMPLE_INTERVAL)
            { 
                Console.Write("(" + index + ", " + Math.Round(currentVal / divBy) + ")"); 
                currentVal = 0;
                index++;
                batch = 0;
            }
            
            batch++;
        }

        Console.WriteLine("]");

        Console.Write("ERRORS (median): \n[");
        index = 0;
        batch = 0;
        currentVal = 0.0;
        foreach (var median in supportDiffMedian) 
        {
            currentVal += median;

            if (batch == STAT_SAMPLE_INTERVAL)
            { 
                Console.Write("(" + index + ", " + currentVal/divBy + ")"); 
                index ++;
                currentVal = 0;
                batch = 0;
            }

            batch++;

        }
        Console.WriteLine("]\n");

        Console.Write("ERRORS (max): \n[");
        index = 0;
        batch = 0;
        currentVal = 0.0;
        foreach (var max in supportDiffMax) 
        {
            currentVal += max;

            if (batch == STAT_SAMPLE_INTERVAL)
            { 
                Console.Write("(" + index + ", " + currentVal/divBy + ")"); 
                index ++;
                batch = 0;
                currentVal = 0;
            }

            batch++;
        }
        Console.WriteLine("]\n\n");

        Console.Write("FALSE-POSITIVES: \n[");
        index = 0;
        batch = 0;
        currentVal = 0.0;
        foreach (var pos in falsePositives) 
        {
            currentVal += pos;

            if (batch == STAT_SAMPLE_INTERVAL)
            { 
                Console.Write("(" + index + ", " + currentVal/divBy + ")"); 
                index ++;
                batch = 0;
                currentVal = 0;
            }

            batch++;
        }
        Console.WriteLine("]\n\n");

        Console.Write("FALSE-NEGATIVES: \n[");
        index = 0;
        batch = 0;
        currentVal = 0.0;
        foreach (var neg in falseNegatives) 
        {
            currentVal += neg;

            if (batch == STAT_SAMPLE_INTERVAL)
            { 
                Console.Write("(" + index + ", " + currentVal/divBy + ")"); 
                index ++;
                batch = 0;
                currentVal = 0;
            }

            batch++;
        }
        Console.WriteLine("]\n\n");
    }

    private static async void benchmarkTest(double minSupport, bool minSupportTest, int k, bool topKTest, int sampleSize, int numBlocks)
    {
        long computationTime = 0;
        
        if (minSupportTest) {
            // test with min support
            Console.WriteLine("****************************************************");
            for (var i = 0; i < NUM_TRIALS; i++) 
            {
                computationTime += await processCSVFileProgressive(FILE_PATH, minSupport, 0, false, sampleSize, numBlocks);
            }


            Console.WriteLine("Min support:               " + minSupport);
            Console.WriteLine("Sample size:     " + sampleSize);
            Console.WriteLine("Average computation time: " + computationTime / (double)NUM_TRIALS);
        }

        if (topKTest) {
            // test with top k
            Console.WriteLine("****************************************************");
            for (var i = 0; i < NUM_TRIALS; i++) 
            {
                computationTime += await processCSVFileProgressive(FILE_PATH, 0, k, true, sampleSize, numBlocks);
            }

            Console.WriteLine("top-k:               " + k);
            Console.WriteLine("Sample size:     " + sampleSize);
            Console.WriteLine("Average computation time: " + computationTime / (double)NUM_TRIALS);
        }

    }

    private static async void testSupportDifference(List<Sequence> expectedResults, List<double>minSupports, List<int>topKs, bool mineTopK, int sampleSize, int datasetSize)
    {
        Console.WriteLine("****************************************************");
        Console.WriteLine("           ITEMSET SUPPORT MEASUREMENTS");
        Console.WriteLine("****************************************************");
        Console.WriteLine("Dataset:         " + FILE_PATH);
        Console.WriteLine("****************************************************");

        if (!mineTopK) {
            // min support
            foreach(var minSup in minSupports)
            {
                shuffleDataset(FILE_PATH);
                await testSupportDifference(expectedResults, minSup, 0, false, sampleSize, datasetSize);
            }
        } else {
            foreach(var k in topKs)
            {
                shuffleDataset(FILE_PATH);
                await testSupportDifference(expectedResults, 0, k, true, sampleSize, datasetSize);
            }
        }
        Console.WriteLine("\n\n");
     
        Console.WriteLine("****************************************************\n\n");
    }

    private static void benchmarkTestPrefixSpan(List<double>minSupports)
    {
        Console.WriteLine("****************************************************");
        Console.WriteLine("             BENCHMARKING PERFORMANCE");
        Console.WriteLine("****************************************************");
        Console.WriteLine("Dataset: " + FILE_PATH);
        
        foreach(var minSup in minSupports)
        {
            benchmarkTestPrefixSpan(minSup);
        }
    }

    private static void benchmarkTestPrefixSpan(double minSupport)
    {
        long computationTime = 0;
        
        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine("MIN SUPPORT: " + minSupport);
        Console.WriteLine("----------------------------------------------------");
        for (var i = 0; i < NUM_TRIALS; i++) 
        {
            computationTime += processCSVFile(FILE_PATH, minSupport);
        }
        Console.WriteLine("Min support:              " + minSupport);
        Console.WriteLine("Average computation time: " + computationTime / (double)NUM_TRIALS);
        Console.WriteLine("----------------------------------------------------");
    }

    private static void benchmarkTest(List<double>minSupports, List<int>topKs, bool topK, int sampleSize, int numBlocks) 
    {
        // don't count batch times
        batchProcessingTimes = null;

        Console.WriteLine("****************************************************");
        Console.WriteLine("             BENCHMARKING PERFORMANCE");
        Console.WriteLine("****************************************************");

        Console.WriteLine("Testing Performance of PROGRESSIVE PrefixSpan algorithm.");
        Console.WriteLine("Dataset:         " + FILE_PATH);
        Console.WriteLine("Sample size:     " + sampleSize);

        if (!topK)
            // min support
            foreach(var minSup in minSupports)
            {
                shuffleDataset(FILE_PATH);
                benchmarkTest(minSup, true, 0, false, sampleSize, numBlocks);
            }
        else {
            foreach(var k in topKs)
            {
                shuffleDataset(FILE_PATH);
                benchmarkTest(0, false, k, true, sampleSize, numBlocks);
            }
        }
    }
    
    private static Tuple<double, double,  int, int> getSupportDifference(List<Sequence> expectedSequences, List<Sequence> foundSequences, int sizeTotal, int numProcessed, int minCount)
    {            
        double max = double.MinValue;
        double median = 0;
        int falsePositives = 0;
        int falseNegatives = 0;
        int numActual = 0;

        List<double> diffs = new List<double>();
        
        Dictionary<Sequence, int> expectedSequencesDict = new Dictionary<Sequence, int>(Sequence.EqComp);
        foreach(var seq in expectedSequences) 
        {
            expectedSequencesDict.Add(new Sequence(seq), seq.Support);
        }
        
        int countOfDiffs = 0;
        foreach(var foundSequence in foundSequences) 
        {
            var fp = foundSequence;
            if (!expectedSequencesDict.ContainsKey(fp))
            {
                falsePositives++;
                continue;
            }

            if (expectedSequencesDict[fp] < minCount)
            {
                falsePositives++;
            }
            else
            {
                numActual++;
            }

            double diff = Math.Abs((expectedSequencesDict[fp]/(double)sizeTotal) - (fp.Support / (double) numProcessed));

            if (diff > max)
                max = diff;

            countOfDiffs++;
            diffs.Add(diff);
        }

        falseNegatives = numActual - foundSequences.Count + falsePositives;

        if (max == double.MinValue)
            max = 0;

        if (diffs.Count != 0)
        {
            diffs.Sort((a,b) => a.CompareTo(b));
            if (countOfDiffs % 2 == 0) {
                median = diffs[((int) (countOfDiffs / 2.0))];
            } 
            else if (diffs.Count > 2)
            {
                median = diffs[(int)Math.Floor(countOfDiffs / 2.0)];
                median += diffs[(int)Math.Ceiling(countOfDiffs / 2.0)];
                median /= 2.0;
            } 
            else 
            {
                median = diffs[0];
            }
        }

        return Tuple.Create<double, double, int, int> (median, max, falsePositives, falseNegatives);
    }

    private static Tuple<List<Sequence>, int> getPrefixSpanResults(string filepath, double minSupport, bool print)
    {
        // current batch to process
        List<Sequence> sequenceList = new List<Sequence>();

        // open the CSV file 
        var reader = new StreamReader(File.OpenRead(filepath));

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
            sequenceList.Add(sequence);
        }

        PrefixSpan algorithm = new PrefixSpan(sequenceList);

        // extract the frequent patterns
        List<Sequence> frequentSequences =  algorithm.MinSupportFrequentSequences(minSupport);

        return new Tuple<List<Sequence>, int>(frequentSequences, sequenceList.Count);
    }
}
