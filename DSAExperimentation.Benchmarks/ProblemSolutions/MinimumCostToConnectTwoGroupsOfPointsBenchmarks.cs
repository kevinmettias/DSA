using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumCostToConnectTwoGroupsOfPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToConnectTwoGroupsOfPointsSolution's, the
// same methods MinimumCostToConnectTwoGroupsOfPointsTests proves correct - the
// textbook unmemoized (index, connectedMask) recursion against the same recurrence
// routed through this repo's own Memoizer. Each is handed the prepared
// ConnectionCosts its hoisted overload takes, so generating the matrix and reducing
// it to per-column minimums is charged to [GlobalSetup] rather than to the recursion
// being measured.
[MemoryDiagnoser]
public class MinimumCostToConnectTwoGroupsOfPointsBenchmarks
{
    private const int RandomSeed = 1595; // LC problem number
    private const int CostExclusiveBound = 100;

    [Params(4, 7)]
    public int GroupSize;

    private ConnectionCosts _costs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var cost = new int[GroupSize][];

        for (var i = 0; i < GroupSize; i++)
        {
            cost[i] = Enumerable.Range(0, GroupSize).Select(_ => random.Next(1, CostExclusiveBound)).ToArray();
        }

        _costs = ConnectionCosts.Build(cost);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByBruteForceRecursion(_costs);

    [Benchmark]
    public int MemoizedRecursion() =>
        MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByMemoizedBitmask(_costs);
}
