using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Grpc.Core;
using Benchmark;
using NDesk.Options;
using System.Linq;
using Newtonsoft.Json;

namespace server {
    
    class BenchmarkerImpl : Benchmarker.BenchmarkerBase
    {        
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        public override async Task monitorMemory(MemoryRequest request, IServerStreamWriter<MemoryReply> responseStream, ServerCallContext context)
        {
            Console.WriteLine(request);
            try 
            {
                List<long> memoryUsage = new List<long>();
                while (!context.CancellationToken.IsCancellationRequested) 
                {
                    await Task.Delay(100);

                    Process currentProcess = Process.GetCurrentProcess();
                    currentProcess.Refresh();
                    long totalBytesOfMemoryUsed = currentProcess.WorkingSet64;
                    memoryUsage.Add(totalBytesOfMemoryUsed);
                    if (memoryUsage.Count >= 1)
                    {
                        var avg = memoryUsage.Average();
                        if (!context.CancellationToken.IsCancellationRequested) 
                        {
                            await responseStream.WriteAsync(new MemoryReply
                            {
                                MemoryUsgageInBytes = (long) avg,
                            });
                        }

                        memoryUsage.Clear();
                    }
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine("MEMORY MONITOR EXCEPTION");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public override async Task runBenchmark(BenchmarkRequest request, IServerStreamWriter<BenchmarkReply> responseStream, ServerCallContext context)
        {
            GC.Collect();
            Console.WriteLine(request);
            try 
            { 
                if (request.ShutdownServer) 
                {
                    Program.ShutdownRequested = true;
                }
                else 
                {
                    if (request.Algorithm == Algorithm.ProSecCo) 
                    {
                        await runBatchMiner(request, responseStream, context);
                    }
                    else if (request.Algorithm == Algorithm.PrefixSpan) 
                    {
                        await runPrefixSpan(request, responseStream, context);
                    }
                }

                GC.Collect();
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async Task runPrefixSpan(BenchmarkRequest request, IServerStreamWriter<BenchmarkReply> responseStream, ServerCallContext context) 
        {
            Console.WriteLine("run prefix span");

            await Task.Run(async () => {
                Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch

                List<Sequence> sequenceList = new List<Sequence>();

                // open the CSV file 
                using (var reader = new StreamReader(File.OpenRead(request.File))) 
                {
                    while(!reader.EndOfStream) 
                    {
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
                }

                PrefixSpan algorithm = new PrefixSpan(sequenceList);
                
                List<Sequence> frequentSequences = algorithm.MinSupportFrequentSequences(request.Support);
                
                // stop the stopwatch after frequent patterns are 
                // returned
                stopwatch.Stop();

                var reply = new BenchmarkReply
                {
                    NrProcessedRecords = request.DBSize,
                    ReplyType = ReplyType.Batch,
                    Iteration = 0,
                    Error = 0,
                    BatchRuntimeInMillis = stopwatch.ElapsedMilliseconds,
                    TotalRuntimeInMillis = stopwatch.ElapsedMilliseconds
                };
                var ser = JsonConvert.SerializeObject(frequentSequences, _jsonSettings);
                reply.SequencesInJson = ser;
                await responseStream.WriteAsync(reply);

                reply = new BenchmarkReply
                {
                    ReplyType = ReplyType.Complete,
                    TotalRuntimeInMillis = stopwatch.ElapsedMilliseconds
                };
                
                await responseStream.WriteAsync(reply);
            });
        }

        private async Task runBatchMiner(BenchmarkRequest request, IServerStreamWriter<BenchmarkReply> responseStream, ServerCallContext context) 
        {
            Console.WriteLine("run batch miner " + request.UseTopK);
           
            await Task.Run(async () => {
                BatchMiner miner = new BatchMiner();
                var batchStopwatch = new Stopwatch();
                batchStopwatch.Restart();
                long totalTimeElapsed = 0;
                //int numBlocks = (int)Math.Ceiling(request.DBSize/(double)request.SampleSize);

                await miner.ProcessCSVFile(
                    request.File,
                    request.Support,
                    request.K,         
                    request.UseTopK,    
                    request.SampleSize,
                    request.DBSize,          
                    request.ErrorTolerance,
                    async delegate(List<Sequence> frequentSequencePatterns, double error, int iteration) 
                    {
                        batchStopwatch.Stop();
                        totalTimeElapsed += batchStopwatch.ElapsedMilliseconds;
                        var reply = new BenchmarkReply
                        {
                            NrProcessedRecords = miner.NumTransactionsProcessed,
                            ReplyType = ReplyType.Batch,
                            Iteration = iteration,
                            Error = error,
                            BatchRuntimeInMillis = batchStopwatch.ElapsedMilliseconds,
                            TotalRuntimeInMillis = totalTimeElapsed
                        };
                      
                        var ser = JsonConvert.SerializeObject(frequentSequencePatterns, _jsonSettings);
                        reply.SequencesInJson = ser;
                        await responseStream.WriteAsync(reply);
                        batchStopwatch.Restart();
                    },
                    long.MaxValue
                );
                
                await responseStream.WriteAsync(new BenchmarkReply
                {
                    ReplyType = ReplyType.Complete,
                    TotalRuntimeInMillis = totalTimeElapsed
                });
            });
        }
    }

    public class Program 
    {    
        public static bool ShutdownRequested {get;set;} = false;

        public static void Main(string[] args)
        {
            int port = -1;
            bool help = false;

            OptionSet p = new OptionSet ();
            p.Add("p|port=", "Port.", arg => { if (arg != null) port = int.Parse(arg); });       
            p.Add("?|h|help", "Show this message and exit.", arg => { if (arg != null) help = true; });

            try
            {
                p.Parse(args);
            } 
            catch {}

            if (port != -1) 
            {
                startServer(port);
            }
            else
            {
                printHelp(p);
            }
        }

        private static void printHelp(OptionSet p) 
        {
            Console.WriteLine("Usage: BenchmarkServer --port: <Port>");
            p.WriteOptionDescriptions (Console.Out);
            Console.ReadKey();
        }

        private static void startServer(int port) 
        {
            var server = new Server
            {
                Services = { Benchmarker.BindService(new BenchmarkerImpl()) },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            }; 
            server.Start();

            Console.WriteLine("Benchmark server listening on port " + port);
            
            while (!ShutdownRequested) 
            {
                Task.Delay(1000).Wait();
            }
            server.ShutdownAsync().Wait();
        }
    }
}
 