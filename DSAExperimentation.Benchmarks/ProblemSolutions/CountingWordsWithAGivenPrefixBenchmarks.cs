using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountingWordsWithAGivenPrefix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountingWordsWithAGivenPrefixSolution's, the same methods
// CountingWordsWithAGivenPrefixTests proves correct. Words are randomly generated
// single-repeated-character strings, so a single-letter prefix gives a realistic,
// non-trivial ~1/26 match rate and neither arm can stop early - the answer is a count.
[MemoryDiagnoser]
public class CountingWordsWithAGivenPrefixBenchmarks
{
    private const string Prefix = "a";
    private const int RandomSeed = 2185; // LC problem number
    private const int AlphabetSize = 26;
    private const int MinWordLength = 3;
    private const int MaxWordLengthExclusive = 8;

    [Params(200, 5_000)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount)
            .Select(_ => new string((char)('a' + random.Next(0, AlphabetSize)), random.Next(MinWordLength, MaxWordLengthExclusive)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int StartsWithScan() =>
        CountingWordsWithAGivenPrefixSolution.CountWordsWithPrefixByStartsWithScan(_words, Prefix);

    [Benchmark]
    public int TriePerWordHasPrefix() =>
        CountingWordsWithAGivenPrefixSolution.CountWordsWithPrefixByTriePerWord(_words, Prefix);
}
