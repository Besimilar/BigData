using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Instantiating WordExtractor and TextStatistics..");
            WordExtractor we = new WordExtractor();
            TextStatistics ts = new TextStatistics();
            Console.WriteLine("done!");

            Console.WriteLine("Please enter textfile name to gather bigram statistics from:");
            var textfile = Console.ReadLine();
            //this.learnTextBackgroundWorker.RunWorkerAsync(this.openTextFileDialog.FileNames);
            Console.WriteLine("Learning text..");
            if (we.Extract(textfile) < 0)
                Console.WriteLine("Error: " + we.Error);
            Console.WriteLine("Extracting statistics..");
            if (ts.Calculate(we) < 0)
                Console.WriteLine("Error: " + ts.Error);

            if (ts.Stats.Count == 0)
            {
                Console.WriteLine("No statistics have been learned yet!");
            }
            else
            {
                Console.WriteLine("Time to simulate! Enter max words per sentence:");
                var maxWordsPerSentenceText = Console.ReadLine();
                ts.MaxWordsPerSentence = Convert.ToInt32(maxWordsPerSentenceText);

                Console.WriteLine("Enter number of sentence to generate:");
                var numberOfSentencesText = Console.ReadLine();   

                String[] simulation = null;
                while(true)
                {
                    if (ts.Simulate(Convert.ToInt32(numberOfSentencesText), out simulation) < 0)
                    {
                        Console.WriteLine("Error: " + ts.Error);
                    }
                    else
                    {
                        Console.WriteLine("=====================================");
                        foreach(var line in simulation)
                            Console.WriteLine(line);
                    }
                    Console.WriteLine("================================more?");
                    Console.ReadLine();
                }
            }

            Console.Read();
        }
    }
}

