using System;
using System.IO;
//using System.Diagnostics;
using System.Collections.Generic;

namespace wm
{

/// <summary>
/// Collects dictionary from text
/// </summary>
public class WordExtractor
{        
//Constructors

//Enums, Structs, Classes

//Properties        

        /// <summary>
        /// Report frequency in "lines read from the text"
        /// </summary>
        public int ReportFreq
        {
                get { return this.reportFreq; }
                set { this.reportFreq = value; }
        }

        /// <summary>
        /// Last error
        /// </summary>
        public String Error
        {
                get { return this.error; }
        }

        /// <summary>
        /// Trimed text without invalid words
        /// </summary>
        public List<String> Text
        {
                get { return this.text; }
        }

        /// <summary>
        /// Text dictionary
        /// </summary>
        public List<String> Dictionary
        {
                get { return this.dictionary; }
        }

//Methods
//operators
//operations
        /// <summary>
        /// Extract dictionary from the text file
        /// </summary>
        /// <param name="fileName">Text file</param>
        /// <returns>zero upon success</returns>
        public int Extract(String fileName)
        {
                try
                {
                        int linesNumber = 0;                        
                        //using (TextReader tr = new StreamReader(fileName))
                        using (TextReader tr = new StreamReader(new FileStream(fileName, FileMode.Open)))
                        {
                                String str;
                                while ((str = tr.ReadLine()) != null)
                                {
                                        linesNumber++;

                                        String[] words = str.Split(this.WordsSeparator, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (String w in words)
                                        {
                                                String word = w;

                                                RemoveUnsupportedChars(ref word);
                                                if (IsValidWord(word) == false)
                                                        continue;                                                

                                                String punctuation;
                                                if (IsEndsWithPunctuation(word, out punctuation) == true)
                                                {                                                        
                                                        RemovePunctuation(ref word);
                                                        if (String.IsNullOrEmpty(word) == false)
                                                        {                                                                
                                                                this.text.Add(word);
                                                                this.text.Add(punctuation);
                                                                if (this.dictionary.Contains(word) == false)
                                                                        this.dictionary.Add(word);
                                                                if (this.dictionary.Contains(punctuation) == false)
                                                                        this.dictionary.Add(punctuation);
                                                        }
                                                }
                                                else
                                                {
                                                        RemovePunctuation(ref word);
                                                        if (String.IsNullOrEmpty(word) == false)
                                                        {                                                                
                                                                this.text.Add(word);
                                                                if (this.dictionary.Contains(word) == false)
                                                                        this.dictionary.Add(word);
                                                        }
                                                }                                                
                                        }

                                        if ((linesNumber % this.reportFreq) == 0)
                                        {
                                                Console.WriteLine("Processing {0} line {1} (found {2} different words)",
                                                                                       Path.GetFileName(fileName), linesNumber, 
                                                                                       this.dictionary.Count);
                                        }
                                }
                        }
                        Console.WriteLine("Processing {0} line {1} (found {2} different words)",
                                                                       Path.GetFileName(fileName), linesNumber,
                                                                       this.dictionary.Count);
                        this.dictionary.Sort();

                        return 0;
                }
                catch (Exception e)
                {
                        Console.WriteLine("WordExtractor.Extract() raised exception {0}", e.Message);
                        this.error = String.Format("WordExtractor.Extract() raised exception {0}", e.Message);
                        return -1;
                }
        }

        /// <summary>
        /// Save dictionary to text file
        /// </summary>
        /// <param name="fileName">Dictionary file name</param>
        /// <returns>zero upon success</returns>
        public int SaveDictionary(String fileName)
        {
                try
                {
                        //using (StreamWriter sw = new StreamWriter(fileName))
                        using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                        {
                                foreach (String str in this.dictionary)
                                        sw.WriteLine(str);
                        }
                        return this.dictionary.Count;
                }
                catch (Exception e)
                {
                        Console.WriteLine("WordExtractor.SaveDictionary() raised exception {0}", e.Message);
                        this.error = String.Format("WordExtractor.SaveDictionary() raised exception {0}", e.Message);
                        return -1;
                }
        }

        /// <summary>
        /// Save trimed text
        /// </summary>
        /// <param name="fileName">Trimed text file name</param>
        /// <returns>zero upon success</returns>
        public int SaveText(String fileName)
        {
                try
                {
                        //using (StreamWriter sw = new StreamWriter(fileName))
                        using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                        {
                                foreach (String str in this.text)
                                        sw.WriteLine(str);
                        }
                        return this.text.Count;
                }
                catch (Exception e)
                {
                        Console.WriteLine("WordExtractor.SaveText() raised exception {0}", e.Message);
                        this.error = String.Format("WordExtractor.SaveText() raised exception {0}", e.Message);
                        return -1;
                }
        }

        /// <summary>
        /// Determines if the string starts with capital letter
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>true if string starts with capital letter</returns>
        public bool IsCapitalWord(String str)
        {
                foreach (String letter in this.CapitalLetters)
                {
                        if (str.StartsWith(letter) == true)
                                return true;
                }
                return false;
        }

        /// <summary>
        /// Determines if the string is one of punctuation marks . , ! ? : ;
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>true if string is a punctuation mark</returns>
        public bool IsPunctuationMark(String str)
        {
                foreach (String punctuation in this.PunctuationMarks)
                {
                        if (str == punctuation)
                                return true;
                }
                return false;
        }

        /// <summary>
        /// Determines if the string contains invalid symbols
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>true is the string contains invalid symbols</returns>
        public bool IsValidWord(String str)
        {
                if (String.IsNullOrEmpty(str) == true)
                        return false;

                foreach (String s in this.ForbiddenChars)
                {
                        if (str.Contains(s))
                                return false;
                }

                if (str.Contains("-") == true)
                {
                        if (String.IsNullOrEmpty(str.Replace("-", " ").Trim()) == true)
                                return false;
                }

                return true;
        }

        /// <summary>
        /// Determines if string contains digits
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>true if string contains digits</returns>
        public bool IsDigit(String str)
        {
                foreach (String s in this.DigitChars)
                        str = str.Replace(s, " ").Trim();
                return String.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Clear dictionary, trimed text, last error string and background report callback
        /// </summary>
        public void Clear()
        {
                this.text.Clear();
                this.dictionary.Clear();
                this.error = String.Empty;
        }

        //access
        //inquiry

        //Fields        
        int reportFreq = 1000;

        string error = String.Empty;

        readonly char[] WordsSeparator = new char[] { ' ' };
        readonly String[] PunctuationMarks = new String[] { ".", ",", "!", "?", ":", ";", "。", "，" };
        readonly String[] UnsupportedChars = new String[] { "\"", "'" };
        readonly String[] ForbiddenChars = new String[] { "`", "~", "@", "#", "$", "%", "^", "&", "*", "(", ")",
                                                          "_", "+", "=", "{", "}", "[", "]", "|", @"\", "/",  
                                                          "<", ">", "\n", "\t" };
        readonly String[] DigitChars = new String[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", ",", "。", "，"};
        //readonly String[] CapitalLetters = new String[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
        //                                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };        

        readonly String[] CapitalLetters = new String[] { "我", "你", "他", "她"};
                
        List<String> text = new List<String>();
        List<String> dictionary = new List<String>();
        
              
        bool IsEndsWithPunctuation(String str, out String punctuation)
        {
                foreach (String s in this.PunctuationMarks)
                {
                        if (str.EndsWith(s))
                        {
                                punctuation = s;                                
                                return true;
                        }
                }
                punctuation = String.Empty;
                return false;
        }

        void RemovePunctuation(ref String str)
        {                
                foreach (String s in this.PunctuationMarks)                
                        str = str.Replace(s, " ").Trim();                
        }

        void RemoveUnsupportedChars(ref String str)
        {
                foreach (String s in this.UnsupportedChars)
                        str = str.Replace(s, " ").Trim();
                str = str.Replace(" ", "'");                        
        }
}
}