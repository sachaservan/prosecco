using System;
using System.Diagnostics;
using System.Collections.Generic;
using NDesk.Options;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApplication 
{
    public class Program 
    {
        static Stopwatch batchStopwatch = Stopwatch.StartNew(); 
        static long totalTimeElapsed = 0;
        static int numSequences = 0;

        static double errorTolerance = 0.05;

        static long sumComponentTimes = 0;

        public static void Main(string[] args) {
            
            string filePath = "";
            int sampleSize = -1;
            int dbSize = -1;
            double minSupport = -1;
            int k = -1;
            bool help = false;
            bool benchmark = false;
            long killTime = int.MaxValue;

            OptionSet p = new OptionSet ();
            p.Add("f|file=", "Source data file {PATH}.", arg => filePath = arg);
            p.Add("b|benchmark", "Benchmark performance.",  arg => { if (arg != null) benchmark = true; });
            p.Add("s|support=", "Minimum pattern {SUPPORT}.", arg => { if (arg != null) minSupport = double.Parse(arg); });
            p.Add("z|size=", "Sample {SIZE}.", arg => { if (arg != null) sampleSize = int.Parse(arg); });
            p.Add("d|db-size=", "Database {SIZE}.", arg => { if (arg != null) dbSize = int.Parse(arg); });
            p.Add("k|top-k=", "Mine top-{K} sequences.", arg => { if (arg != null) k = int.Parse(arg); });
            p.Add("kill=", "Stop program after {TIME} miliseconds", arg => { if (arg != null) killTime = int.Parse(arg); });            
            p.Add("?|h|help", "Show this message and exit.", arg => { if (arg != null) help = true; });

            try
            {
                p.Parse(args);
            } 
            catch {}

            if (benchmark && filePath != "" && sampleSize >= 500 && dbSize > 0) 
            {
                List<double> minSupports = new List<double>();
                minSupports.Add(0.01);
                minSupports.Add(0.025);
                minSupports.Add(0.05);
                minSupports.Add(0.1);

                // TODO: use top-k here
                BenchmarkTest.StartBenchmark(filePath, minSupports, new List<int>(), sampleSize, dbSize);
       
            }
            else if (!help && filePath != "" && (minSupport > 0 || k > 0) && sampleSize >= 500 && dbSize > 0)
            {
                if (k > 0 && minSupport > 0)
                    printHelp(p);
                else {
                    Console.WriteLine("Kill-after: " + killTime);
                    processFile(filePath, minSupport, k, k > 0, sampleSize, dbSize, killTime).Wait();
                }
            }
            else
            {
                printHelp(p);
            }
        }

        private static void printHelp(OptionSet p) 
        {
            Console.WriteLine("Usage: ProSecCo --file: <file-path> (--support: <min-support> | --top-k: <k>) --size: <sample-size>");
            p.WriteOptionDescriptions (Console.Out);
        }

        private static async Task<long> processFile(string filepath, double minSupport,int k, bool mineTopK, int sampleSize, int dbSize, long killTime)
        {
            BatchMiner batchPatternMiner = new BatchMiner();
            batchStopwatch.Restart();
            totalTimeElapsed = 0;

            await batchPatternMiner.ProcessCSVFile(
                filepath,
                minSupport,
                k,
                mineTopK,
                sampleSize,
                dbSize,
                errorTolerance,
                async delegate(List<Sequence> frequentSequencePatterns, double error, int iteration) 
                {
                    onBatchResults(batchPatternMiner, frequentSequencePatterns, error, iteration);
                },
                killTime);

            batchStopwatch.Stop();

            // print out final result stats
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("Done with Progressive PrefixSpan results.");
            Console.WriteLine("Number of frequent sequences:     " + numSequences + " sequences");
            Console.WriteLine("Mining type:            Minimum support mining");
            if (mineTopK) 
                Console.WriteLine("Top-k:                  " + k);
            else 
                Console.WriteLine("Minimum support:        " + minSupport);
            Console.WriteLine("Total runtime:          " + totalTimeElapsed + "ms");
            Console.WriteLine("Component runtime sum:  " + sumComponentTimes + "ms");

            Console.WriteLine("********************************************************************************");

            return totalTimeElapsed;
        }
        private static async Task onBatchResults(
            BatchMiner batchMiner,
            List<Sequence> frequentSequences, 
            double error,
            int iteration) 
        {           
            numSequences = frequentSequences.Count; 
            batchStopwatch.Stop();
           
             
            totalTimeElapsed += batchStopwatch.ElapsedMilliseconds;
            
            // sort and print        
            frequentSequences.Sort(Sequence.SequenceSorter);
            frequentSequences.ForEach(Console.WriteLine);

            sumComponentTimes += batchMiner.PrevBlockFileReadingTime;
            sumComponentTimes += batchMiner.Algorithm.PrevBlockPreProcessingRuntime;
            sumComponentTimes += batchMiner.Algorithm.PrevBlockPrefixSpanRuntime;
            sumComponentTimes += batchMiner.Algorithm.PrevBlockSubsequenceMatchingRuntime;

            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Batch " + iteration);
            Console.WriteLine("Number of frequent sequences:     " + frequentSequences.Count + " sequences");
            Console.WriteLine("Error:  " + error);
            Console.WriteLine("Processing time: " + batchStopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Runtime breakdown: ");
            Console.WriteLine(" File reading:         " + batchMiner.PrevBlockFileReadingTime + "ms");
            Console.WriteLine(" Pre processing:       " + batchMiner.Algorithm.PrevBlockPreProcessingRuntime + "ms");
            Console.WriteLine(" PrefixSpan:           " + batchMiner.Algorithm.PrevBlockPrefixSpanRuntime + "ms");
            Console.WriteLine(" Subsequence matching: " + batchMiner.Algorithm.PrevBlockSubsequenceMatchingRuntime + "ms");
            Console.WriteLine("-----------------------------------------------------------\n");

               
            batchStopwatch.Restart();
        }
    }
}
