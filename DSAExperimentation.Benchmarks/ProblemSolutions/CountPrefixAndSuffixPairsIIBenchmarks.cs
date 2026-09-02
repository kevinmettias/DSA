using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountPrefixAndSuffixPairsIISolution's, the
// same methods CountPrefixAndSuffixPairsIITests proves correct. Reuses
// CountPrefixAndSuffixPairsIBenchmarks' own PrefixSuffixPairWorkloads - the
// words themselves are the only shared input either strategy needs, so
// there is nothing further to hoist into [GlobalSetup] for the trie arm
// (building the trie IS the O(total length) computation being measured, not
// preparation for it).
[MemoryDiagnoser]
public class CountPrefixAndSuffixPairsIIBenchmarks
{
    private const int Seed = 3045;

    [Params(100, 400)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup() => _words = PrefixSuffixPairWorkloads.BuildWords(WordCount, maxLength: 30, seed: Seed);

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountPrefixAndSuffixPairsIISolution.CountPairsByBruteForce(_words);

    [Benchmark]
    public long LowercaseTrie() => CountPrefixAndSuffixPairsIISolution.CountPairsByLowercaseTrie(_words);
}
