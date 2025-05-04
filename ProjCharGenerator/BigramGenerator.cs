using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjCharGenerator
{
    class BigramGenerator
    {
        private Dictionary<char, Dictionary<char, int>> bigrams;
        private Random rand = new Random();

        public BigramGenerator(string filePath)
        {
            bigrams = LoadBigrams(filePath);
        }

        private Dictionary<char, Dictionary<char, int>> LoadBigrams(string path)
        {
            var result = new Dictionary<char, Dictionary<char, int>>();

            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3) continue;

                char first = parts[0][0];
                char second = parts[1][0];
                if (!int.TryParse(parts[2], out int freq)) continue;

                if (!result.ContainsKey(first))
                    result[first] = new Dictionary<char, int>();

                result[first][second] = freq;
            }

            return result;
        }

        public string Generate(int length)
        {
            StringBuilder sb = new StringBuilder();

            // выбираем случайную стартовую букву
            List<char> startOptions = new List<char>(bigrams.Keys);
            char current = startOptions[rand.Next(startOptions.Count)];
            sb.Append(current);

            while (sb.Length < length)
            {
                if (!bigrams.ContainsKey(current)) break;

                var next = bigrams[current];
                int total = 0;
                foreach (var val in next.Values)
                    total += val;

                int roll = rand.Next(total);
                int sum = 0;

                foreach (var kv in next)
                {
                    sum += kv.Value;
                    if (roll < sum)
                    {
                        current = kv.Key;
                        sb.Append(current);
                        break;
                    }
                }
            }

            return sb.ToString();
        }
    }
}
