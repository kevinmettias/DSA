namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3042 - words drawn from a small 4-letter
// alphabet, not the full 26, so prefix/suffix matches actually occur at a
// realistic rate: a uniform 26-letter alphabet would make almost every
// candidate pair fail on its very first character, letting both strategies
// short-circuit instead of walking a meaningful prefix/suffix span.
internal static class PrefixSuffixPairWorkloads
{
    private const string Alphabet = "abcd";

    public static string[] BuildWords(int count, int maxLength, int seed) =>
        RandomWords.Build(count, Alphabet, maxLength, seed);
}
