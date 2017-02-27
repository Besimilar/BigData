using System;
using System.IO;
//using System.Diagnostics;
using System.Collections.Generic;

namespace wm
{

/// <summary>
/// Calculates text statistics from WordExtractor object as P(NextWord|PrevWord)
/// </summary>
public class TextStatistics
{        
//Constructors

//Enums, Structs, Classes  
        /// <summary>
        /// Next word and its P(NextWord|PrevWord)
        /// </summary>
        public class WordFreqPair
        {
                /// <summary>
                /// Next word
                /// </summary>
                public String Word { get; set; }
                /// <summary>
                /// Next word probability
                /// </summary>
                public double Prob { get; set; }
        }

        //Properties

        /// <summary>
        /// Last error
        /// </summary>
        public String Error
        {
                get { return this.error; }
        }

        /// <summary>
        /// Conditional probabilities P(NextWord|PrevWord)
        /// </summary>
        public Dictionary<String, List<WordFreqPair>> Stats
        {
                get { return this.stats; }
        }

        /// <summary>
        /// Words starting with capital letter
        /// </summary>
        public List<String> CapitalWords
        {
                get { return this.capitalWords; }
        }

        /// <summary>
        /// Limit for maximum words per simulated sentence
        /// </summary>
        public int MaxWordsPerSentence
        {
                get { return this.maxWordsPerSentence; }
                set { this.maxWordsPerSentence = value; }
        }

        
        
        //Methods
        //operators
        //operations
        
        /// <summary>
        /// Extract statistics as conditional probabilities P(NextWord|PrevWord)
        /// </summary>
        /// <param name="we">WordExtractor object</param>
        /// <returns>zero upon success</returns>
        public int Calculate(WordExtractor we)
        {
                try
                {
                        if (we.Text.Count < 3 || we.Dictionary.Count < 3)
                                throw new Exception("WordExtractor contains less than 3 words.");

                        this.stats.Clear();
                        this.capitalWords.Clear();
                        this.error = String.Empty;

                        foreach (String word in we.Dictionary)
                                this.stats.Add(word, new List<WordFreqPair>());
                        
                        //calculate conditional p = nextWord|prevWord and normalize it
                        for (int i = 1; i < we.Text.Count; i++)
                        {
                                String prevWord = we.Text[i - 1];
                                String nextWord = we.Text[i];
                                List<WordFreqPair> wfpList = this.stats[prevWord];
                                int index = IsWordPresent(wfpList, nextWord);
                                if (index < 0)                                
                                        wfpList.Add(new WordFreqPair() { Word = nextWord, Prob = 1.0 });                                
                                else                                                                        
                                        wfpList[index].Prob++;                                                                
                        }

                        List<String> emptyNodes = new List<string>();
                        foreach (String word in this.stats.Keys)
                        {
                                List<WordFreqPair> wfpList = this.stats[word];
                                if (wfpList.Count == 0)
                                {
                                        emptyNodes.Add(word);
                                        continue;
                                }

                                if (we.IsCapitalWord(word) == true)
                                        this.capitalWords.Add(word);

                                double sum = 0.0;
                                for (int i = 0; i < wfpList.Count; i++)
                                        sum += wfpList[i].Prob;
                                for (int i = 0; i < wfpList.Count; i++)
                                        wfpList[i].Prob /= sum;
                        }

                        //remove last empty word from text if any
                        foreach (String word in emptyNodes)
                                this.stats.Remove(word);
                        
                        if (this.stats.ContainsKey(".") == true)
                                this.isPointPresent = true;                                                

                        return 0;
                }
                catch (Exception e)
                {
                        Console.WriteLine("TextStatistics.Calculate() raised exception {0}", e.Message);
                        this.error = String.Format("TextStatistics.Calculate() raised exception {0}", e.Message);
                        return -1;
                }
        }

        /// <summary>
        /// Save dictionary with conditional probabilities P(NextWord|PrevWord)
        /// </summary>
        /// <param name="fileName">Text file</param>
        /// <returns>zero upon success</returns>
        public int SaveDictionary(String fileName)
        {
                try
                {
                        //using (StreamWriter sw = new StreamWriter(fileName))
                        using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                        {
                                foreach (String word in this.stats.Keys)
                                {
                                        sw.WriteLine(word);
                                        List<WordFreqPair> wfpList = this.stats[word];
                                        for (int i = 0; i < wfpList.Count; i++)
                                                sw.WriteLine(String.Format("    {0,-25} {1}", wfpList[i].Word, wfpList[i].Prob));
                                        sw.WriteLine("");
                                }                        
                        }
                        return 0;
                }
                catch (Exception e)
                {
                        Console.WriteLine("TextStatistics.SaveDictionary({0}) raised exception {1}", fileName, e.Message);
                        this.error = String.Format("TextStatistics.SaveDictionary({0}) raised exception {1}", fileName, e.Message);
                        return -1;
                }
        }

        /// <summary>
        /// Save list of capital words
        /// </summary>
        /// <param name="fileName">Text file name</param>
        /// <returns>zero upon success</returns>
        public int SaveCapitalWords(String fileName)
        {
                try
                {
                        //using (StreamWriter sw = new StreamWriter(fileName))
                        using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                        {
                                foreach (String word in this.capitalWords)                                
                                        sw.WriteLine(word);                                                                                                
                        }
                        return 0;
                }
                catch (Exception e)
                {
                        Console.WriteLine("TextStatistics.SaveCapitalWords({0}) raised exception {1}", fileName, e.Message);
                        this.error = String.Format("TextStatistics.SaveCapitalWords({0}) raised exception {1}", fileName, e.Message);
                        return -1;
                }
        }
        
        /// <summary>
        /// Simulate sentences from learned model P(NextWord|PrevWord)
        /// </summary>
        /// <param name="sentencesNumber">Number of sentences to generate</param>
        /// <param name="simulationText">Output simulated sentences to string array</param>
        /// <returns>zero upon success</returns>
        public int Simulate(int sentencesNumber, out String[] simulationText)
        {
                try
                {
                        if (sentencesNumber < 1)
                                throw new Exception("wrong number of sentences to simulate.");
                        if (this.stats.Count == 0)
                                throw new Exception("the text have not been learned. call TextStatistics.Learn()");
                        if (this.isPointPresent == false)
                                throw new Exception("there is no . in dictionary, can not simulate sentence");

                        int index = 0;
                        simulationText = new String[sentencesNumber];
                        WordExtractor we = new WordExtractor();
                        
                        String sentence = String.Empty;
                        String word = SimulateFirstWord();
                        int wordsInSentence = 1;
                        while (true)
                        {
                                if (we.IsPunctuationMark(word) == true)
                                {
                                        sentence += word;
                                }
                                else
                                {
                                        sentence += " " + word;
                                        wordsInSentence++;
                                }

                                if (word == "." || word == "!" || word == "?")
                                {
                                        simulationText[index++] = sentence + "\r\n";
                                        sentence = String.Empty;
                                        wordsInSentence = 0;
                                        sentencesNumber--;
                                }                                

                                if (sentencesNumber <= 0)
                                        break;
                                
                                word = SimulateNextWord(this.stats[word], wordsInSentence);
                        }        
                                        
                        return 0;
                }
                catch (Exception e)
                {
                        simulationText = null;
                        Console.WriteLine("TextStatistics.Simulate({0}) raised exception {1}", sentencesNumber, e.Message);
                        this.error = String.Format("TextStatistics.Simulate({0}) raised exception {1}", sentencesNumber, e.Message);                        
                        return -1;
                }
        }

        //access
        //inquiry

        //Fields
        string error = String.Empty;

        Dictionary<String, List<WordFreqPair>> stats = new Dictionary<String, List<WordFreqPair>>();
        List<String> capitalWords = new List<String>();
        bool isPointPresent = false;

        int maxWordsPerSentence = 30;        

        Random rnd = new Random();


        int IsWordPresent(List<WordFreqPair> wfpList, String str)
        {
                for (int i = 0; i < wfpList.Count; i++)
                {
                        if (wfpList[i].Word == str)
                                return i;
                }
                return -1;
        }        

        String SimulateFirstWord()
        {                
                String startWord = String.Empty;

                if (this.capitalWords.Count > 0)
                {
                        int index = this.rnd.Next(this.capitalWords.Count);
                        startWord = this.capitalWords[index];
                }
                else
                {
                        int index = this.rnd.Next(this.stats.Count);
                        foreach (String word in this.stats.Keys)
                        {
                                if (--index <= 0)
                                {
                                        startWord = word;
                                        break;
                                }
                        }
                }

                return startWord;
        }

        String SimulateNextWord(List<WordFreqPair> wfpList, int wordsInSentence)
        {
                if (wfpList.Count == 1)
                        return wfpList[0].Word;

                if (wordsInSentence > this.maxWordsPerSentence)
                {
                        for (int i = 0; i < wfpList.Count; i++)
                        {
                                if (wfpList[i].Word == "." ||
                                    wfpList[i].Word == "!" ||
                                    wfpList[i].Word == "?")
                                        return wfpList[i].Word;
                        }
                }

                double val = this.rnd.NextDouble();
                double sum = 0.0;
                for (int i = 0; i < wfpList.Count; i++)
                {
                        sum += wfpList[i].Prob;
                        if (val < sum)
                                return wfpList[i].Word;
                }

                return wfpList[wfpList.Count - 1].Word;
        }

}

}