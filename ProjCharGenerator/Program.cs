using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ProjCharGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Выбери генератор:");
            Console.WriteLine("1 — случайный (CharGenerator)");
            Console.WriteLine("2 — по биграммам (BigramGenerator)");
            Console.WriteLine("3 — по частотным словам (WordGenerator)");
            Console.Write("Ввод: ");
            string input = Console.ReadLine();

            if (input == "1")
            {
                RunRandom();
            }
            else if (input == "2")
            {
                RunBigram();
            }
            else if (input == "3")
            {
                RunWords();
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Попробуй заново.");
            }

            Console.WriteLine("\nНажми Enter для выхода...");
            Console.ReadLine();
        }

        static void RunRandom()
        {
            CharGenerator gen = new();
            var stat = new SortedDictionary<char, int>();

            for (int i = 0; i < 1000; i++)
            {
                char ch = gen.getSym();
                Console.Write(ch);

                if (stat.ContainsKey(ch))
                    stat[ch]++;
                else
                    stat[ch] = 1;
            }

            Console.WriteLine("\n\nЧастоты:");
            foreach (var item in stat)
            {
                Console.WriteLine($"{item.Key} - {item.Value / 1000.0}");
            }
        }

        static void RunBigram()
        {
            var gen = new BigramGenerator("bigram.txt");
            string text = gen.Generate(1000);
            Console.WriteLine(text);

            var stat = new Dictionary<string, int>();
            for (int i = 0; i < text.Length - 1; i++)
            {
                string pair = text.Substring(i, 2);
                if (stat.ContainsKey(pair))
                    stat[pair]++;
                else
                    stat[pair] = 1;
            }

            int total = stat.Values.Sum();
            var actual = stat.ToDictionary(kv => kv.Key, kv => kv.Value / (double)total);

            Directory.CreateDirectory("../Results");
            File.WriteAllText("../Results/gen-1.txt", text);

            var expected = gen.ExpectedFrequencies;

            int partSize = 200;
            var keys = actual.Keys.ToList();

            for (int i = 0; i < (keys.Count + partSize - 1) / partSize; i++)
            {
                var part = new Dictionary<string, double>();
                for (int j = i * partSize; j < Math.Min((i + 1) * partSize, keys.Count); j++)
                {
                    string key = keys[j];
                    part[key] = actual[key];
                }

                var partExpected = expected
                    .Where(kv => part.ContainsKey(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                GraphBuilder.Build(partExpected, part, Directory.GetCurrentDirectory(),$"gen-1-part-{i + 1}", $"Биграммы (часть {i + 1})", "Биграмма");

            }

            Console.WriteLine("\nГрафики сохранены по частям в папке Results.");
        }



        static void RunWords()
        {
            var gen = new WordGenerator("word.txt");
            string text = gen.Generate(1000);
            Console.WriteLine(text);

            var stat = new Dictionary<string, int>();
            foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (stat.ContainsKey(word))
                    stat[word]++;
                else
                    stat[word] = 1;
            }

            var actual = stat.ToDictionary(kvp => kvp.Key, kvp => kvp.Value / 1000.0);
            var expected = gen.ExpectedFrequencies;

            string resultsDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Results");
            Directory.CreateDirectory(resultsDir);
            File.WriteAllText(Path.Combine(resultsDir, "gen-2.txt"), text);

            GraphBuilder.Build(expected, actual, resultsDir, "Слова", "Слова", "Слово");


        }

    }
}
