using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CollectCoinsInATreeBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes from the
// workload's shape rather than from either arm. The adjacency list and the coin array are both
// private, and the trimmed edge count is the only thing either arm reports, so the shape is asserted
// through that: a tree on NodeCount vertices has NodeCount-1 edges, and no trim can ever remove more
// than all of them.
public sealed partial class CollectCoinsInATreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void RescanUntilFixedPoint_RandomTreeWithSparseCoins_StaysWithinTheTreesEdgeCount() =>
        Assert.InRange(BuildHarness().RescanUntilFixedPoint(), 0, SmallestNodeCount - 1);

    private static CollectCoinsInATreeBenchmarks BuildHarness()
    {
        var harness = new CollectCoinsInATreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
