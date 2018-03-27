using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Benchmark;
using Newtonsoft.Json;
using NDesk.Options;

namespace server {
    
    public class Program 
    {    
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        private static int _port = -1;
        private static int _sampleSize = -1;
        private static int _numberOfRuns = -1;
        private static double _errorTolerance = -1;
        private static double _support = -1;
        private static string _algorithm = "";
        private static string _file = "";
        private static string _id = "";
        private static int _k = -1;
        private static bool _useTopK = false;
        private static int _dbSize = -1;

        private static bool _terminateServer = false;


        public static void Main(string[] args) 
        {
            bool help = false;

            OptionSet p = new OptionSet ();
            p.Add("i|id=", "Name of this run.", arg => _id = arg);
            p.Add("p|port=", "Port of server.", arg => { if (arg != null) _port = int.Parse(arg); });
            p.Add("r|runs=", "Number of runs.", arg => { if (arg != null) _numberOfRuns = int.Parse(arg); });
            p.Add("f|file=", "Source data file {PATH}.", arg => _file = arg);
            p.Add("u|usetopk=", "Use top-K mining.", arg => { if (arg != null) _useTopK = true; });
            p.Add("k|top-k=", "Mine top-{K} sequences.", arg => { if (arg != null) _k = int.Parse(arg); });
            p.Add("s|support=", "Minimum pattern {SUPPORT}.", arg => { if (arg != null) _support = double.Parse(arg); });
            p.Add("d|db-size=", "Database {SIZE}.", arg => { if (arg != null) _dbSize = int.Parse(arg); });
            p.Add("z|size=", "Sample {SIZE}.", arg => { if (arg != null) _sampleSize = int.Parse(arg); });
            p.Add("e|error=", "Error Tolerance.", arg => { if (arg != null) _errorTolerance = double.Parse(arg); });
            p.Add("a|algorithm=", "Algorithm to run (ProSecCo, PrefixSpan)", arg => _algorithm = arg);     
            p.Add("t|terminate", "Terminate server after run.", arg => { if (arg != null) _terminateServer = true; });
            p.Add("?|h|help", "Show this message and exit.", arg => { if (arg != null) help = true; });

            try
            {
                p.Parse(args);
            } 
            catch 
            {
                printHelp(p);
            }

           if (!help)
            {
                 run().Wait();
            }
            else
            {
                printHelp(p);
            }
        }

        private static void printHelp(OptionSet p) 
        {
            Console.WriteLine("Usage: BenchmarkClient");
            p.WriteOptionDescriptions (Console.Out);
        }

        public async static Task run() 
        {
            var channel = new Channel("127.0.0.1:" + _port, ChannelCredentials.Insecure);
            var client = new Benchmarker.BenchmarkerClient(channel);

            Report report = new Report() 
            {
                Id = _id,
                Algorithm = _algorithm,
                NumberOfRuns = _numberOfRuns,
                SampleSize = _sampleSize,
                ErrorTolerance = _errorTolerance,
                Support = _support,
                File = _file
            };

            Console.Write("computing or reading expected results...");          
            List<Sequence> expectedResults;   
            var cacheFilename = "../results/groundtruths/" + Path.GetFileNameWithoutExtension(_file) + "_" + _support + "_gt";
            if (File.Exists(cacheFilename)) 
            {
                var ser = File.ReadAllText(cacheFilename);
                expectedResults = JsonConvert.DeserializeObject<List<Sequence>>(ser, _jsonSettings);
            }
            else 
            {
                Tuple<List<Sequence>, int> results = getGroundTruth(_file, _support);
                expectedResults = results.Item1;
                expectedResults.Sort((a,b) => b.Support.CompareTo(a.Support));
                var ser = JsonConvert.SerializeObject(expectedResults, _jsonSettings);
                File.WriteAllText(cacheFilename, ser);
            }
            Console.WriteLine(" done");


            for (int r = 0; r < _numberOfRuns; r++) 
            {
                Run run = new Run() 
                { 
                    RunId = r
                };

                // shuffle 
                Console.Write("shuffle file for run " + r + "...");
                var shuffledFile = shuffleDataset(_file);
                Console.WriteLine(" done");

                var ts = new CancellationTokenSource();
                CancellationToken ct = ts.Token;

                // setup memory monitor
                Task.Run(async () =>
                    {
                        try 
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                        
                            Console.WriteLine("Start Memory Monitor");
                            using (var call = client.monitorMemory(new MemoryRequest()))
                            {
                                while (await call.ResponseStream.MoveNext())
                                {                                    
                                    if (ct.IsCancellationRequested) 
                                    {
                                        break;
                                    }
                                    var response = call.ResponseStream.Current;
                                    double gb = response.MemoryUsgageInBytes / (double)1073741824;
                                    Console.WriteLine("Current Memory Usage: " + gb.ToString("N2") + "GB" + " / " + (sw.ElapsedMilliseconds / 1000) + "s / run nr: " + r);
                                    if (!run.MemoryUsagePerTimeInMillis.ContainsKey(sw.ElapsedMilliseconds)) 
                                    {
                                        run.MemoryUsagePerTimeInMillis.Add(sw.ElapsedMilliseconds, response.MemoryUsgageInBytes);
                                    }
                                }
                            }
                        }
                        catch(Exception e) 
                        {
                            Console.WriteLine("MEMORY MONITOR FAILED");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.InnerException.StackTrace);
                        }
                    }, ct);


                var request = new BenchmarkRequest 
                    {
                        Id = _id,
                        Algorithm = _algorithm == "ProSecCo" ? Algorithm.ProSecCo : _algorithm == "PrefixSpan" ? Algorithm.PrefixSpan :Algorithm.Undefined,
                        SampleSize = _sampleSize,
                        K = _k,
                        UseTopK = _useTopK,
                        DBSize = _dbSize,
                        ErrorTolerance = _errorTolerance,
                        Support = _support,
                        File = shuffledFile,
                        ShutdownServer = false
                    };

                Console.WriteLine(request);

                
                using (var call = client.runBenchmark(request))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;
                       
                        if (response.ReplyType == ReplyType.Batch) 
                        {
                            var sequences = JsonConvert.DeserializeObject<List<Sequence>>(response.SequencesInJson, _jsonSettings);
                            run.RuntimePerBatch.Add(response.BatchRuntimeInMillis);
                            run.Errors.Add(response.Error);
                            
                            run.LastBlockFileReadingAndParsingTimePerBatch.Add(response.LastBlockFileReadingAndParsingTime);
                            run.LastBlockPrefixSpanRuntimePerBatch.Add(response.LastBlockPrefixSpanRuntime);
                            run.LastBlockSubsequenceMatchingRuntimePerBatch.Add(response.LastBlockSubsequenceMatchingRuntime);

                            evaluate(run, response.Iteration, expectedResults, sequences, response.NrProcessedRecords);

                            /*Console.WriteLine("--- " + (run.Errors.Count() - 1));
                            foreach (var seq in sequences) 
                            {
                                var description = "";
                                foreach(var itemSet in seq.Transactions) {
                                    if (itemSet.NumItems == 1)
                                        description += itemSet.ToString() + " ";
                                    else
                                        description +="{" + itemSet.ToString() + "} ";
                                }

                                description = description.Substring(0, description.Length - 1);
                                
                                Console.WriteLine(description + ", " + ((double)seq.Support / (double)response.NrProcessedRecords) * (double)_dbSize);
                            }*/
                        }
                        else if (response.ReplyType == ReplyType.Complete) 
                        {
                            run.Errors.Add(response.Error);
                            run.TotalRuntimeInMillis = response.TotalRuntimeInMillis;
                        }
                    }
                }
                ts.Cancel();

                report.Runs.Add(run);
                Console.WriteLine("Wait for a little...");
                await Task.Delay(5000); // to make sure GC is done collecting on server side
            }
                          
            if (_terminateServer) 
            {
                client.runBenchmark(new BenchmarkRequest { ShutdownServer = true });
                await channel.ShutdownAsync();
            }

            // write report
            File.WriteAllText("../results/" + _id + ".json", JsonConvert.SerializeObject(report, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            }));
        }

        private static void evaluate(Run run, int iteration, List<Sequence> expectedResults, List<Sequence> results, int numberOfRecordsProcessed)
        {
            var minCount = (int)Math.Floor(_dbSize * _support);
            Error error = getSupportDifference(expectedResults, results, _dbSize, numberOfRecordsProcessed, minCount);

            run.NormalizedErrorsPerBatch.Add(error.NormalizedErrors);
            run.AbsoluteErrorsPerBatch.Add(error.AbsoluteErrors);
            run.TruePositivesPerBatch.Add(error.TruePositives);
            run.FalsePositivesPerBatch.Add(error.FalsePositives);
            run.FalseNegativesPerBatch.Add(error.FalseNegatives);
        }

        private static Error getSupportDifference(List<Sequence> expectedSequences, List<Sequence> foundSequences, int sizeTotal, int numProcessed, int minCount)
        {            
            double max = double.MinValue;
            double median = 0;

            int falsePositives = 0;
            int falseNegatives = 0;
            int truePositives = 0;

            List<double> diffs = new List<double>();
            List<double> normalizedErrors = new List<double>();
            List<double> absoluteErrors = new List<double>();
            
            Dictionary<Sequence, int> expectedSequencesDict = new Dictionary<Sequence, int>(Sequence.EqComp);
            foreach(var seq in expectedSequences) 
            {
                if (!expectedSequencesDict.ContainsKey(seq)) 
                {
                    expectedSequencesDict.Add(new Sequence(seq), seq.Support);
                }
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

                /*if (expectedSequencesDict[fp] < minCount)
                {
                    falsePositives++;
                    continue;
                }*/

                truePositives++;

                double diff = Math.Abs((expectedSequencesDict[fp]/(double)sizeTotal) - (fp.Support / (double) numProcessed));
                double normalizedError = Math.Abs((expectedSequencesDict[fp]/(double)sizeTotal) - (fp.Support / (double) numProcessed)) / (double)(expectedSequencesDict[fp]/(double)sizeTotal) ;
                double absoluteError = Math.Abs((expectedSequencesDict[fp]/(double)sizeTotal) - (fp.Support / (double) numProcessed));

                if (diff > max)
                    max = diff;

                countOfDiffs++;
                diffs.Add(diff);
                normalizedErrors.Add(normalizedError);                
                absoluteErrors.Add(absoluteError);
            }

            falseNegatives = expectedSequencesDict.Count - truePositives;

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

            return new Error    
                { 
                    TruePositives = truePositives,
                    FalsePositives = falsePositives,
                    FalseNegatives = falseNegatives,
                    NormalizedErrors = normalizedErrors,
                    AbsoluteErrors = absoluteErrors
                };
        }

        private static Tuple<List<Sequence>, int> getGroundTruth(string filepath, double minSupport)
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

        private static string shuffleDataset(string file)
        {
            var lines = File.ReadAllLines(file);
            var rnd = new Random();
            lines = lines.OrderBy(line => rnd.Next()).ToArray();
            var newFile = file + "_shuff";
            File.WriteAllLines(newFile, lines);     
            return newFile;
        }
    }

    public class Error 
    {
        public int TruePositives {get;set;} = 0;
        public int FalsePositives {get;set;} = 0;
        public int FalseNegatives {get;set;} = 0;
        public List<double> NormalizedErrors = null;
        public List<double> AbsoluteErrors = null;
    }

    public class Report 
    {
        public string Id {get; set;}
        public string Algorithm {get; set;}
        public int SampleSize {get; set;}
        public int NumberOfRuns {get; set;}
        public double ErrorTolerance {get; set;}
        public double Support {get; set;}
        public string File {get; set;}
        public List<Run> Runs {get; set;} = new List<Run>();
    }

    public class Run 
    {
        public int RunId {get; set;}
        public long TotalRuntimeInMillis {get; set;}
        public List<long> LastBlockFileReadingAndParsingTimePerBatch {get; set;} = new List<long>();
        public List<long> LastBlockPrefixSpanRuntimePerBatch {get; set;} = new List<long>();
        public List<long> LastBlockSubsequenceMatchingRuntimePerBatch {get; set;} = new List<long>();
        public List<long> RuntimePerBatch {get; set;} = new List<long>();
        public List<double> Errors{get; set;} = new List<double>();
        public List<List<double>> NormalizedErrorsPerBatch{get; set;} = new List<List<double>>();
        public List<List<double>> AbsoluteErrorsPerBatch{get; set;} = new List<List<double>>();
        public List<int> TruePositivesPerBatch {get;set;} = new List<int>();
        public List<int> FalsePositivesPerBatch {get;set;} = new List<int>();
        public List<int> FalseNegativesPerBatch {get;set;} = new List<int>();
        public Dictionary<long, long> MemoryUsagePerTimeInMillis = new Dictionary<long, long>();
    }
}
 