using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Text;

namespace MyBenchmarks
{
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net80)]
    [RankColumn, MarkdownExporterAttribute.StackOverflow]
    public class Benchmark
    {
        private static readonly string[] words =
        [
            "lorem",
            "ipsum",
            "dolor",
            "sit",
            "amet",
            "consectetuer",
            "adipiscing",
            "elit",
            "sed",
            "diam",
            "nonummy",
            "nibh",
            "euismod",
            "tincidunt",
            "ut",
            "laoreet",
            "dolore",
            "magna",
            "aliquam",
            "erat"
        ];

        private static IEnumerable<string> LoremIpsum(
            Random random,
            int minWords,
            int maxWords,
            int minSentences,
            int maxSentences,
            int numLines)
        {
            var line = new StringBuilder();
            for (var l = 0; l < numLines; l++)
            {
                line.Clear();
                var numSentences = random.Next(maxSentences - minSentences) + minSentences + 1;
                for (var s = 0; s < numSentences; s++)
                {
                    var numWords = random.Next(maxWords - minWords) + minWords + 1;
                    line.Append(words[random.Next(words.Length)]);

                    for (var w = 1; w < numWords; w++)
                    {
                        line.Append(" ");
                        line.Append(words[random.Next(words.Length)]);
                    }

                    line.Append(". ");
                }

                yield return line.ToString();
            }
        }

        private string[]? _lines;

        [Params(1000, 1_000_000)]
        public int N;

        [Params("lorem", "lorem ipsum dolor sit amet consectetuer")]
        public string? SearchValue;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _lines = LoremIpsum(new Random(), 6, 8, 2, 3, 1_000_000).ToArray();
        }

        public static int CountOccurrencesSub(
            string val,
            string searchFor)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(searchFor))
            {
                return 0;
            }

            var count = 0;
            for (var x = 0; x <= val.Length - searchFor.Length; x++)
            {
                if (val.Substring(x, searchFor.Length) == searchFor)
                {
                    count++;
                }
            }

            return count;
        }

        public static int CountOccurrences(string val, string searchFor)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(searchFor))
            {
                return 0;
            }

            var count = 0;

            var vSpan = val.AsSpan();
            var searchSpan = searchFor.AsSpan();

            for (var x = 0; x <= vSpan.Length - searchSpan.Length; x++)
            {
                if (vSpan.Slice(x, searchSpan.Length).SequenceEqual(searchSpan))
                {
                    count++;
                }
            }

            return count;
        }

        public static int CountOccurrencesCmp(string val, string searchFor)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(searchFor))
            {
                return 0;
            }

            var count = 0;

            for (var x = 0; x <= val.Length - searchFor.Length; x++)
            {
                if (string.CompareOrdinal(val, x, searchFor, 0, searchFor.Length) == 0)
                {
                    count++;
                }
            }

            return count;
        }


        [Benchmark(Baseline = true)]
        public int Substring()
        {
            var occurrences = 0;
            for (var i = 0; i < N; i++)
            {
                occurrences += CountOccurrencesSub(_lines[i], SearchValue);
            }

            return occurrences;
        }

        [Benchmark]
        public int Span()
        {
            var occurrences = 0;
            for (var i = 0; i < N; i++)
            {
                occurrences += CountOccurrences(_lines[i], SearchValue);
            }

            return occurrences;
        }

        [Benchmark]
        public int Compare()
        {
            var occurrences = 0;
            for (var i = 0; i < N; i++)
            {
                occurrences += CountOccurrencesCmp(_lines[i], SearchValue);
            }
            return occurrences;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }
}