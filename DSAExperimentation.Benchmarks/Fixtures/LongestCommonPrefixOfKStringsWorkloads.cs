namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3485 - words drawn from a small 3-letter
// alphabet rather than the full 26, the same PrefixSuffixPairWorkloads reasoning
// (LC 3042): a uniform 26-letter alphabet would make almost every pair diverge on
// its first character, leaving nothing for either strategy to actually walk.
internal static class LongestCommonPrefixOfKStringsWorkloads
{
    private const string Alphabet = "abc";

    public static string[] BuildWords(int count, int maxLength, int seed) =>
        RandomWords.Build(count, Alphabet, maxLength, seed);
}
