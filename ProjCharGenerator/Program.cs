using System;
using System.Collections.Generic;
using System.IO;

namespace ProjCharGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Выбери генератор:");
            Console.WriteLine("1 — случайный (CharGenerator)");
            Console.WriteLine("2 — по биграммам (BigramGenerator)");
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

            var stat = new Dictionary<char, int>();
            foreach (char ch in text)
            {
                if (stat.ContainsKey(ch)) stat[ch]++;
                else stat[ch] = 1;
            }

            Console.WriteLine("\nЧастоты:");
            foreach (var item in stat)
            {
                Console.WriteLine($"{item.Key} - {item.Value / 1000.0}");
            }

            string outDir = Path.Combine("..", "Results");
            Directory.CreateDirectory(outDir);
            File.WriteAllText(Path.Combine(outDir, "gen-1.txt"), text);

            Console.WriteLine("\nСохранил в Results/gen-1.txt");
        }
    }
}
