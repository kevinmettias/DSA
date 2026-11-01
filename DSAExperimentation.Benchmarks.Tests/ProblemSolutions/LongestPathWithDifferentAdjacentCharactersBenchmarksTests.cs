using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestPathWithDifferentAdjacentCharactersBenchmarks (ARCHITECTURE 17.9):
// Setup builds a chain whose labels alternate between two characters, so every edge is valid and
// the longest path spans all NodeCount nodes - an expected value derived from the workload's
// construction rather than from either arm, which the generic agreement check cannot supply. The
// baseline returns the path's node count, a scalar compared directly.
public sealed partial class LongestPathWithDifferentAdjacentCharactersBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // Every adjacent pair in Setup's chain differs by construction, so no edge blocks the path.
    private const int ExpectedLongestPathNodeCount = SmallestNodeCount;

    [Fact]
    public void RecomputedChainPerNode_SmallestNodeCount_SpansEveryNode() =>
        Assert.Equal(ExpectedLongestPathNodeCount, BuildHarness().RecomputedChainPerNode());

    private static LongestPathWithDifferentAdjacentCharactersBenchmarks BuildHarness()
    {
        var harness = new LongestPathWithDifferentAdjacentCharactersBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
