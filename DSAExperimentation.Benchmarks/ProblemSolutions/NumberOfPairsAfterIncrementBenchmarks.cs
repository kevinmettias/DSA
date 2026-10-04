using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are NumberOfPairsAfterIncrementSolution's, the same
// methods NumberOfPairsAfterIncrementSolutionTests proves correct. Each is handed the
// already-parsed PairQuery stream its hoisted overload takes, so int[][] parsing is
// charged to [GlobalSetup] rather than to the search being measured. Sizes are per
// arm: the two rebuild strategies pay O(n) for every count query and stop at 5,000
// values; the block decomposition answers either kind of query in O(sqrt n) and runs
// on to LC 3943's 5 * 10^4.
public class NumberOfPairsAfterIncrementBenchmarks
{
    private const int Seed = 3943; // LC problem number
    private const int QueryCount = 2_000;

    private Dictionary<int, (int[] Nums1, int[] Nums2, PairQuery[] Queries)> _workloadBySize = [];

    public static IEnumerable<int> RebuildSizes => [500, 5_000];

    public static IEnumerable<int> ValueBlockSizes => [.. RebuildSizes, 50_000];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _workloadBySize = RebuildSizes.Union(ValueBlockSizes).ToDictionary(
            size => size,
            size => (
                NumberOfPairsAfterIncrementWorkloads.BuildNums1(Seed),
                NumberOfPairsAfterIncrementWorkloads.BuildNums2(size, Seed),
                NumberOfPairsAfterIncrementWorkloads.BuildQueries(QueryCount, size, Seed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(RebuildSizes))]
    public int[] DirectArray(int nums2Length)
    {
        var workload = _workloadBySize[nums2Length];

        return NumberOfPairsAfterIncrementSolution.CountPairsByDirectArray(workload.Nums1, workload.Nums2, workload.Queries);
    }

    [Benchmark]
    [ArgumentsSource(nameof(RebuildSizes))]
    public int[] RangeFenwickTree(int nums2Length)
    {
        var workload = _workloadBySize[nums2Length];

        return NumberOfPairsAfterIncrementSolution.CountPairsByRangeFenwickTree(workload.Nums1, workload.Nums2, workload.Queries);
    }

    [Benchmark]
    [ArgumentsSource(nameof(ValueBlockSizes))]
    public int[] ValueBlocks(int nums2Length)
    {
        var workload = _workloadBySize[nums2Length];

        return NumberOfPairsAfterIncrementSolution.CountPairsByValueBlocks(workload.Nums1, workload.Nums2, workload.Queries);
    }
}
