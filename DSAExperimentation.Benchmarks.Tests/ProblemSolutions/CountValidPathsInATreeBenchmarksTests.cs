using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountValidPathsInATreeBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes
// from the workload's shape rather than from either arm.
//
// The edge array is private and the path count is the only thing either arm reports, so the
// documented shape is asserted through that: a valid path is one unordered pair among the tree's
// NodeCount nodes, so no count can exceed C(NodeCount, 2), and the NodeCount - 1 edges of a tree on
// that many nodes are what make the whole pair set reachable at all.
public sealed partial class CountValidPathsInATreeBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    private const long NodePairCount = (long)SmallestNodeCount * (SmallestNodeCount - 1) / 2;

    [Fact]
    public void PerPairPathWalk_SmallestNodeCount_StaysWithinTheNodePairCount() =>
        Assert.InRange(BuildHarness().PerPairPathWalk(), 0, NodePairCount);

    private static CountValidPathsInATreeBenchmarks BuildHarness()
    {
        var harness = new CountValidPathsInATreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
