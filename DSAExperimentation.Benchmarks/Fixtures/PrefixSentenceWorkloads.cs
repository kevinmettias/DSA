namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1455's sentence scan. Everything about the
// strategies themselves lives in the LeetCode tier; what stays here is only how big
// the sentence is and which seed generates it.
internal static class PrefixSentenceWorkloads
{
    // Longer than any generated word and outside the generated alphabet's runs, so
    // no word ever starts with it and every strategy is forced through the whole
    // sentence instead of stopping at an early match.
    public const string UnmatchedSearchWord = "zzzunmatched";

    private const char WordSeparator = ' ';
    private const char FirstLetter = 'a';
    private const int AlphabetSize = 26;
    private const int MinWordLength = 3;
    private const int MaxWordLengthExclusive = 8;

    // `wordCount` space-separated runs of a single repeated letter, in LeetCode's
    // own one-string input shape.
    public static string BuildSentence(int wordCount, int seed)
    {
        var random = new Random(seed);
        var words = Enumerable.Range(0, wordCount)
            .Select(_ => new string(
                (char)(FirstLetter + random.Next(0, AlphabetSize)),
                random.Next(MinWordLength, MaxWordLengthExclusive)));

        return string.Join(WordSeparator, words);
    }
}
