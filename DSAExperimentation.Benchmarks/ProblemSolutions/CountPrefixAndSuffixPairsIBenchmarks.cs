using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountPrefixAndSuffixPairsISolution's, the same
// methods CountPrefixAndSuffixPairsITests proves correct. Each word's RollingHash
// is built once in [GlobalSetup], so the rolling-hash arm is only ever charged
// for the O(1) prefix/suffix comparisons themselves.
[MemoryDiagnoser]
public class CountPrefixAndSuffixPairsIBenchmarks
{
    private const int Seed = 3042;

    [Params(25, 100)]
    public int WordCount;

    private string[] _words = null!;
    private RollingHash[] _hashes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _words = PrefixSuffixPairWorkloads.BuildWords(WordCount, maxLength: 50, seed: Seed);
        _hashes = CountPrefixAndSuffixPairsISolution.BuildHashes(_words);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => CountPrefixAndSuffixPairsISolution.CountPairsByBruteForce(_words);

    [Benchmark]
    public int RollingHash() => CountPrefixAndSuffixPairsISolution.CountPairsByRollingHash(_hashes);
}
