using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountPairsWithXorInARange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountPairsWithXorInARangeSolution's, the same methods
// CountPairsWithXorInARangeTests proves correct - the textbook O(n^2) pairwise scan
// against this repo's own BitTrie plus a HashMap of per-node subtree counts, which
// answers each value's range query in O(32) instead of rescanning every earlier
// value. The values themselves are drawn once in [GlobalSetup] so neither arm is
// charged for building its input.
[MemoryDiagnoser]
public class CountPairsWithXorInARangeBenchmarks
{
    private const int Low = 100;
    private const int High = 5_000;
    private const int ValueUpperBound = 20_000;
    private const int RandomSeed = 1;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan() =>
        CountPairsWithXorInARangeSolution.CountPairsByPairwiseScan(_values, Low, High);

    [Benchmark]
    public int BitTrieRangeCount() =>
        CountPairsWithXorInARangeSolution.CountPairsByBitTrieRangeCount(_values, Low, High);
}
