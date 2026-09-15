using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfPrefixScoresOfStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfPrefixScoresOfStringsSolution's, the same
// methods SumOfPrefixScoresOfStringsTests proves correct. [GlobalSetup] generates
// the word list - LeetCode's own input shape, so it is handed straight to each
// strategy and no prepared-input overload is needed - leaving each arm to measure
// only the scoring.
//
// Re-scanning the whole word list with string.StartsWith for each of a word's own
// prefixes is O(wordCount^2 * wordLength); building the prefix-counting
// LowercaseTrie<int> once and reading each word's score off its root-to-leaf path
// is O(wordCount * wordLength) twice over.
[MemoryDiagnoser]
public class SumOfPrefixScoresOfStringsBenchmarks
{
    private const int WordLength = 8;
    private const int RandomSeed = 2416; // LC problem number
    private const int AlphabetSize = 26;

    private string[] _words = [];

    [Params(300, 1_500)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount).Select(_ => RandomWord(random)).ToArray();
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

    [Benchmark(Baseline = true)]
    public int[] StartsWithScan() => SumOfPrefixScoresOfStringsSolution.SumPrefixScoresByStartsWithScan(_words);

    [Benchmark]
    public int[] PrefixCountingTrie() =>
        SumOfPrefixScoresOfStringsSolution.SumPrefixScoresByPrefixCountingTrie(_words);
}
