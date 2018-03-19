using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using NDesk.Options;

namespace ConsoleApplication {
    
    public class Program 
    {    
        public static void Main(string[] args) 
        {
            string filePath = "";
            int maxLength = 0;
            double minSupport = -1;
            bool help = false;
            int k = 0;

            OptionSet p = new OptionSet ();
            p.Add("f|file=", "Source data file {PATH}.", arg => filePath = arg);
            p.Add("s|support=", "Minimum sequence {SUPPORT}.", arg => { if (arg != null) minSupport = double.Parse(arg); });
            p.Add("k|top-k=", "Mine top {K} sequences.", arg => { if (arg != null) k = int.Parse(arg); });
            p.Add("l|length=", "Maximum sequence {LENGTH}.", arg => { if (arg != null) maxLength = int.Parse(arg); });
            p.Add("?|h|help", "Show this message and exit.", arg => { if (arg != null) help = true; });

            try
            {
                p.Parse(args);
            } 
            catch {}

            if (!help && filePath != "" && (minSupport >= 0 || k > 0))
            {
                processFile(filePath, minSupport, maxLength);
            }
            else
            {
                printHelp(p);
            }
        }

        private static void printHelp(OptionSet p) 
        {
            Console.WriteLine("Usage: PrefixSpan --file: <file-path> (--top-k <k> | --support: <min-support>) [-length: <max-sequence-length>]");
            p.WriteOptionDescriptions (Console.Out);
        }

        private static long processFile(string filepath, double minSupport, int maxLength)
        {
            // measure the time it takes for the algorithm to complete
            Stopwatch stopwatch = Stopwatch.StartNew();

            // current batch to process
            List<Sequence> sequences = new List<Sequence>(5000000);

            // read the csv file 
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
                sequences.Add(sequence);
            }

            long csvReadingTime = stopwatch.ElapsedMilliseconds;

            PrefixSpan algo = new PrefixSpan(sequences);
            
            var frequentSequences = algo.MinSupportFrequentSequences(minSupport);
            frequentSequences.Sort(Sequence.SequenceSorter);
            frequentSequences.ForEach(Console.WriteLine);
            
            Console.WriteLine("*****************************************************************************");
            Console.WriteLine("Done with PrefixSpan results.");
            Console.WriteLine("Number of frequent sequences:        " + frequentSequences.Count + " sequences.");
            Console.WriteLine("Minimum support:                     " + minSupport);
            Console.WriteLine("CSV reading time:                    " + csvReadingTime + "ms.");
            Console.WriteLine("Total run time:                      " + stopwatch.ElapsedMilliseconds + "ms.");
            Console.WriteLine("*****************************************************************************");

            return stopwatch.ElapsedMilliseconds;
        }
    }
}
 