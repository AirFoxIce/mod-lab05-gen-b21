using System;
using System.Collections.Generic;
using System.IO;

namespace ProjCharGenerator
{
    class BigramLoader
    {
        public static Dictionary<char, Dictionary<char, int>> Load(string path)
        {
            var result = new Dictionary<char, Dictionary<char, int>>();

            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3) continue;

                char first = parts[0][0];
                char second = parts[1][0];
                if (!int.TryParse(parts[2], out int frequency)) continue;

                if (!result.ContainsKey(first))
                    result[first] = new Dictionary<char, int>();

                result[first][second] = frequency;
            }

            return result;
        }
    }
}
