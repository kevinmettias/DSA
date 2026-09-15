using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfPairsAfterIncrementSolution's, the same
// methods NumberOfPairsAfterIncrementTests proves correct. Each is handed the
// already-parsed PairQuery stream its hoisted overload takes, so int[][]
// parsing is charged to [GlobalSetup] rather than to the search being measured;
// nums1/nums2 themselves are cheap arrays with nothing to hoist beyond their own
// construction.
[MemoryDiagnoser]
public class NumberOfPairsAfterIncrementBenchmarks
{
    private const int Seed = 3943; // LC problem number
    private const int QueryCount = 2_000;

    private int[] _nums1 = [];

    private int[] _nums2 = [];
    private PairQuery[] _queries = [];
    [Params(500, 5_000)]
    public int Nums2Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums1 = NumberOfPairsAfterIncrementWorkloads.BuildNums1(Seed);
        _nums2 = NumberOfPairsAfterIncrementWorkloads.BuildNums2(Nums2Length, Seed);
        _queries = NumberOfPairsAfterIncrementWorkloads.BuildQueries(QueryCount, Nums2Length, Seed);
    }

    [Benchmark(Baseline = true)]
    public int[] DirectArray() =>
        NumberOfPairsAfterIncrementSolution.CountPairsByDirectArray(_nums1, _nums2, _queries);

    [Benchmark]
    public int[] RangeFenwickTree() =>
        NumberOfPairsAfterIncrementSolution.CountPairsByRangeFenwickTree(_nums1, _nums2, _queries);
}
