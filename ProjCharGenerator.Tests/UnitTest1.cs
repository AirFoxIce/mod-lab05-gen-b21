using System;
using Xunit;
using ProjCharGenerator;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjCharGenerator.Tests
{
    public class GeneratorTests
    {
        private string GetFullPath(string filename)
        {
            return Path.Combine(AppContext.BaseDirectory, filename);
        }

        [Fact]
        public void BigramGenerator_GeneratesCorrectLength()
        {
            var path = GetFullPath("bigram.txt");
            var gen = new BigramGenerator(path);
            string text = gen.Generate(200);
            Assert.Equal(200, text.Length);
        }

        [Fact]
        public void BigramGenerator_ReturnsNotEmptyText()
        {
            var path = GetFullPath("bigram.txt");
            var gen = new BigramGenerator(path);
            string text = gen.Generate(50);
            Assert.False(string.IsNullOrWhiteSpace(text));
        }

        [Fact]
        public void WordGenerator_GeneratesWords()
        {
            var path = GetFullPath("word.txt");
            var gen = new WordGenerator(path);
            string text = gen.Generate(100);
            Assert.Contains(" ", text);
        }

        [Fact]
        public void WordGenerator_WordCountIsCorrect()
        {
            var path = GetFullPath("word.txt");
            var gen = new WordGenerator(path);
            string text = gen.Generate(50);
            Assert.Equal(50, text.Split(' ').Length);
        }

        [Fact]
        public void Dictionary_NormValuesSumToOne()
        {
            var dict = new Dictionary<string, int>
            {
                {"а", 2}, {"б", 3}, {"в", 5}
            };
            int total = 10;
            var norm = dict.ToDictionary(kv => kv.Key, kv => kv.Value / (double)total);
            double sum = norm.Values.Sum();
            Assert.True(Math.Abs(sum - 1.0) < 0.0001);
        }

        [Fact]
        public void GraphBuilder_CreatesFile()
        {
            var expected = new Dictionary<string, double> { { "аб", 0.4 } };
            var actual = new Dictionary<string, double> { { "аб", 0.6 } };
            string dir = Path.Combine(AppContext.BaseDirectory, "Results");
            Directory.CreateDirectory(dir);
            string filename = "test-graph";
            GraphBuilder.Build(expected, actual, dir, filename, "Test", "X");
            Assert.True(File.Exists(Path.Combine(dir, filename + ".png")));
        }

        [Fact]
        public void BigramDictionary_LoadsFrequencies()
        {
            var path = GetFullPath("bigram.txt");
            var gen = new BigramGenerator(path);
            Assert.NotEmpty(gen.ExpectedFrequencies);
        }

        [Fact]
        public void WordGenerator_HasExpectedFrequencies()
        {
            var path = GetFullPath("word.txt");
            var gen = new WordGenerator(path);
            Assert.NotEmpty(gen.ExpectedFrequencies);
        }

        [Fact]
        public void BigramText_ContainsOnlyKnownSymbols()
        {
            var path = GetFullPath("bigram.txt");
            var gen = new BigramGenerator(path);
            string text = gen.Generate(200);
            foreach (char c in text)
                Assert.Contains(c, "абвгдеёжзийклмнопрстуфхцчшщьыъэюя");
        }

        [Fact]
        public void WordGenerator_FirstWordIsValid()
        {
            var path = GetFullPath("word.txt");
            var gen = new WordGenerator(path);
            string text = gen.Generate(10);
            string firstWord = text.Split(' ').First();
            Assert.Matches("^[а-яё]+$", firstWord);
        }
    }
}
