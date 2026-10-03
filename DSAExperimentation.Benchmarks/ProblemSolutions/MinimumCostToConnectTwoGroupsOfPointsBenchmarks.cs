using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostToConnectTwoGroupsOfPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToConnectTwoGroupsOfPointsSolution's, the
// same methods MinimumCostToConnectTwoGroupsOfPointsSolutionTests proves correct - the
// textbook unmemoized (index, connectedMask) recursion against the same recurrence
// routed through this repo's own Memoizer. Each is handed the prepared
// ConnectionCosts its hoisted overload takes, so generating the matrix and reducing
// it to per-column minimums is charged to [GlobalSetup] rather than to the recursion
// being measured.
//
// Sizes are per arm. The unmemoized recursion tries every group-2 point for every
// group-1 point, n^n paths, so it stops at 7; the memoized arm solves each of the
// n * 2^n (index, mask) states once and runs on to LC 1595's own bound of 12. The two
// are compared at the sizes both run.
public class MinimumCostToConnectTwoGroupsOfPointsBenchmarks
{
    private const int RandomSeed = 1595; // LC problem number
    private const int CostExclusiveBound = 100;

    private Dictionary<int, ConnectionCosts> _costsBySize = [];

    public static IEnumerable<int> BaselineSizes => [4, 7];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 10, 12];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _costsBySize = MemoizedSizes.ToDictionary(groupSize => groupSize, BuildCosts);

    private static ConnectionCosts BuildCosts(int groupSize)
    {
        var random = new Random(RandomSeed);
        var cost = new int[groupSize][];

        for (var i = 0; i < groupSize; i++)
        {
            cost[i] = SeededDraws.Values(groupSize, 1, CostExclusiveBound, random);
        }

        return ConnectionCosts.Build(cost);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceRecursion(int groupSize) =>
        MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByBruteForceRecursion(_costsBySize[groupSize]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int groupSize) =>
        MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByMemoizedBitmask(_costsBySize[groupSize]);
}
