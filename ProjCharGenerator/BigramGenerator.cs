using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjCharGenerator
{
    class BigramGenerator
    {
        private Dictionary<string, double> expected = new();
        private Dictionary<char, Dictionary<char, int>> table = new();
        private Random random = new();

        public Dictionary<string, double> ExpectedFrequencies => expected;

        public BigramGenerator(string path)
        {
            Load(path);
        }

        private void Load(string path)
        {
            var raw = new Dictionary<string, int>();

            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(' ');
                if (parts.Length != 3) continue;

                string bigram = parts[0] + parts[1];
                if (!int.TryParse(parts[2], out int freq)) continue;

                raw[bigram] = freq;

                char a = parts[0][0];
                char b = parts[1][0];

                if (!table.ContainsKey(a))
                    table[a] = new Dictionary<char, int>();

                table[a][b] = freq;
            }

            double total = raw.Values.Sum();
            foreach (var kv in raw)
                expected[kv.Key] = kv.Value / total;
        }

        public string Generate(int length)
        {
            if (table.Count == 0) return "";

            var result = new List<char>();
            var first = table.Keys.ToList()[random.Next(table.Count)];
            result.Add(first);

            while (result.Count < length)
            {
                char current = result[^1];
                if (!table.ContainsKey(current)) break;

                var nextOptions = table[current];
                int sum = nextOptions.Values.Sum();
                int roll = random.Next(sum);
                int acc = 0;

                foreach (var kv in nextOptions)
                {
                    acc += kv.Value;
                    if (roll < acc)
                    {
                        result.Add(kv.Key);
                        break;
                    }
                }
            }

            return new string(result.ToArray());
        }
    }
}
