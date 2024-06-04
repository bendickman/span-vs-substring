using System.Text;

public static class BenchmarkHelpers
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

    public static IEnumerable<string> LoremIpsum(
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
}