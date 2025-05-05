using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace ProjCharGenerator
{
    class WordGenerator
    {
        private Dictionary<string, double> frequencies = new Dictionary<string, double>();
        private Random random = new Random();

        public Dictionary<string, double> ExpectedFrequencies => frequencies;

        public WordGenerator(string path)
        {
            LoadFrequencies(path);
        }

        private void LoadFrequencies(string path)
        {
            Dictionary<string, double> raw = new Dictionary<string, double>();
            double total = 0;

            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(' ');
                if (parts.Length < 2) continue;

                try
                {
                    string word = parts[0];
                    double freq = Convert.ToDouble(parts[1], CultureInfo.InvariantCulture);

                    raw[word] = freq;
                    total += freq;
                }
                catch
                {
                    // Если вдруг ошибка....
                }
            }

            foreach (var pair in raw)
            {
                frequencies[pair.Key] = pair.Value / total;
            }
        }


        public string Generate(int count)
        {
            List<string> result = new List<string>();
            var words = new List<string>(frequencies.Keys);
            var probs = new List<double>(frequencies.Values);

            for (int i = 0; i < count; i++)
            {
                double roll = random.NextDouble();
                double sum = 0;

                for (int j = 0; j < probs.Count; j++)
                {
                    sum += probs[j];
                    if (roll < sum)
                    {
                        result.Add(words[j]);
                        break;
                    }
                }
            }

            return string.Join(" ", result);
        }
    }
}
