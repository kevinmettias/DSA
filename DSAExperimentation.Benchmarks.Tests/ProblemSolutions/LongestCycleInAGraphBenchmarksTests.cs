using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCycleInAGraphBenchmarks (ARCHITECTURE 17.9): Setup builds one cycle
// spanning every node, so the only cycle there is holds all NodeCount nodes - an expected value
// derived from the workload's construction rather than from either arm, which the generic agreement
// check cannot supply. The baseline returns the longest cycle's node count, a scalar compared
// directly.
public sealed partial class LongestCycleInAGraphBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // FunctionalGraphs.BuildSingleCycleEdges links every node into one cycle, so the longest
    // cycle visits every node.
    private const int ExpectedLongestCycleLength = SmallestNodeCount;

    [Fact]
    public void PerStartWalk_SmallestNodeCount_VisitsEveryNode() =>
        Assert.Equal(ExpectedLongestCycleLength, BuildHarness().PerStartWalk());

    private static LongestCycleInAGraphBenchmarks BuildHarness()
    {
        var harness = new LongestCycleInAGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
