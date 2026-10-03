using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountPairsWithXorInARange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountPairsWithXorInARangeSolution's, the same methods
// CountPairsWithXorInARangeSolutionTests proves correct - the textbook O(n^2) pairwise scan
// against this repo's own BitTrie plus a HashMap of per-node subtree counts, which
// answers each value's range query in O(32) instead of rescanning every earlier
// value. The values themselves are drawn once in [GlobalSetup] so neither arm is
// charged for building its input.
public class CountPairsWithXorInARangeBenchmarks
{
    private const int Low = 100;
    private const int High = 5_000;
    private const int ValueUpperBound = 20_000;
    private const int RandomSeed = 1;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        // LC 1803's values run from 1 to 2 * 10^4.
        _values = SeededDraws.Values(Length, 1, ValueUpperBound + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan() =>
        CountPairsWithXorInARangeSolution.CountPairsByPairwiseScan(_values, Low, High);

    [Benchmark]
    public int BitTrieRangeCount() =>
        CountPairsWithXorInARangeSolution.CountPairsByBitTrieRangeCount(_values, Low, High);
}
