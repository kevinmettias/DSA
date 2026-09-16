namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3093 - both word lists are drawn over a 2-letter
// alphabet, so container and query words share long suffixes instead of diverging on
// their first character: the same narrowing LongestCommonPrefixOfKStringsWorkloads
// applies for LC 3485 and PrefixSuffixPairWorkloads for LC 3042.
internal static class LongestCommonSuffixQueriesWorkloads
{
    private const int MaxWordLength = 10;
    private const string Alphabet = "ab";

    // A Random is taken rather than a seed because the harness draws the container
    // words and the query words from one stream - handing each list its own Random
    // would give the two lists the same draws.
    public static string[] BuildWords(int count, Random random) =>
        RandomWords.Build(count, Alphabet, MaxWordLength, random);
}
