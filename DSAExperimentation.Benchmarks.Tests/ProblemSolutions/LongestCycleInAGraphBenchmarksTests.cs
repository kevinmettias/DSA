using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCycleInAGraphBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - walking forward from every start against one
// Tarjan SCC pass - so a harness whose arms disagree is timing two different problems. Both arms
// return the longest cycle's node count, a scalar compared directly. Setup builds one cycle
// spanning every node, so the only cycle there is holds all NodeCount nodes: that count is the
// decisive value both arms must reach, and the same NodeCount must rebuild the same graph.
public sealed partial class LongestCycleInAGraphBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // FunctionalGraphs.BuildSingleCycleEdges links every node into one cycle, so the longest
    // cycle visits every node.
    private const int ExpectedLongestCycleLength = SmallestNodeCount;

    [Fact]
    public void Setup_SmallestNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestCycleLength, BuildHarness().TarjanScc());
        Assert.Equal(BuildHarness().PerStartWalk(), BuildHarness().PerStartWalk());
    }

    [Fact]
    public void PerStartWalk_SmallestNodeCount_AgreesWithTarjanScc()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestCycleLength, harness.PerStartWalk());
        Assert.Equal(harness.TarjanScc(), harness.PerStartWalk());
    }

    [Fact]
    public void TarjanScc_SmallestNodeCount_AgreesWithPerStartWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestCycleLength, harness.TarjanScc());
        Assert.Equal(harness.PerStartWalk(), harness.TarjanScc());
    }

    private static LongestCycleInAGraphBenchmarks BuildHarness()
    {
        var harness = new LongestCycleInAGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
