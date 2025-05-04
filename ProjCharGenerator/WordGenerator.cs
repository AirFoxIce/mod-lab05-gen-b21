using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjCharGenerator
{
    class WordGenerator
    {
        private Dictionary<string, int> words = new Dictionary<string, int>();
        private Random rnd = new Random();

        public WordGenerator(string path)
        {
            LoadWords(path);
        }

        private void LoadWords(string path)
        {
            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(' ');
                if (parts.Length == 2)
                {
                    string word = parts[0];
                    int freq = int.Parse(parts[1].Replace(".", "")); // убираем дроби, если что
                    words[word] = freq;
                }
            }
        }

        public string Generate(int wordCount)
        {
            StringBuilder sb = new StringBuilder();
            List<string> wordList = new List<string>(words.Keys);

            int total = 0;
            foreach (var val in words.Values)
                total += val;

            for (int i = 0; i < wordCount; i++)
            {
                int roll = rnd.Next(total);
                int sum = 0;

                foreach (var word in wordList)
                {
                    sum += words[word];
                    if (roll < sum)
                    {
                        sb.Append(word + " ");
                        break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}
