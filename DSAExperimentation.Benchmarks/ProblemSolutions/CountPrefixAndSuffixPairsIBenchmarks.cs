using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountPrefixAndSuffixPairsISolution's, the same
// methods CountPrefixAndSuffixPairsISolutionTests proves correct. Each word's RollingHash
// is built once in [GlobalSetup], so the rolling-hash arm is only ever charged
// for the O(1) prefix/suffix comparisons themselves.
public class CountPrefixAndSuffixPairsIBenchmarks
{
    private const int Seed = 3042;

    private string[] _words = [];

    private RollingHash[] _hashes = [];
    // LC 3042 gives at most 50 words of at most 10 letters.
    [Params(25, 50)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _words = PrefixSuffixPairWorkloads.BuildWords(WordCount, maxLength: 10, seed: Seed);
        _hashes = CountPrefixAndSuffixPairsISolution.BuildHashes(_words);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => CountPrefixAndSuffixPairsISolution.CountPairsByBruteForce(_words);

    [Benchmark]
    public int RollingHash() => CountPrefixAndSuffixPairsISolution.CountPairsByRollingHash(_hashes);
}
