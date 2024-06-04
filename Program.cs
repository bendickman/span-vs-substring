using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Buffers;

namespace MyBenchmarks
{
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net80)]
    [RankColumn, MarkdownExporterAttribute.StackOverflow]
    public class Benchmark
    {
        private string[]? _lines;

        [Params(1, 100)]
        public int N;

        [Params("lorem", "lorem ipsum dolor sit amet consectetuer")]
        public string? SearchValue;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _lines = BenchmarkHelpers.LoremIpsum(new Random(), 6, 8, 2, 3, 1_000).ToArray();
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