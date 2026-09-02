namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3042 - words drawn from a small 4-letter
// alphabet, not the full 26, so prefix/suffix matches actually occur at a
// realistic rate: a uniform 26-letter alphabet would make almost every
// candidate pair fail on its very first character, letting both strategies
// short-circuit instead of walking a meaningful prefix/suffix span.
internal static class PrefixSuffixPairWorkloads
{
    private const string Alphabet = "abcd";

    public static string[] BuildWords(int count, int maxLength, int seed)
    {
        var random = new Random(seed);
        var words = new string[count];

        for (var i = 0; i < count; i++)
        {
            var length = random.Next(1, maxLength + 1);
            var chars = new char[length];

            for (var j = 0; j < length; j++)
            {
                chars[j] = Alphabet[random.Next(Alphabet.Length)];
            }

            words[i] = new string(chars);
        }

        return words;
    }
}
