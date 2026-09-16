using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestPathWithDifferentAdjacentCharactersBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - recomputing every subtree height
// from scratch against this repo's single TreeFold pass - so a harness whose arms disagree is
// timing two different problems. Both arms return the path's node count, a scalar compared
// directly. Setup builds a chain whose labels alternate between two characters, so every edge is
// valid and the longest path spans all NodeCount nodes: that count is the decisive value both
// arms must reach, and the same NodeCount must rebuild both the chain and its labels.
public sealed partial class LongestPathWithDifferentAdjacentCharactersBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // Every adjacent pair in Setup's chain differs by construction, so no edge blocks the path.
    private const int ExpectedLongestPathNodeCount = SmallestNodeCount;

    [Fact]
    public void Setup_SmallestNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestPathNodeCount, BuildHarness().TreeFoldLongestPath());
        Assert.Equal(
            BuildHarness().RecomputedChainPerNode(),
            BuildHarness().RecomputedChainPerNode());
    }

    [Fact]
    public void RecomputedChainPerNode_SmallestNodeCount_AgreesWithTreeFoldLongestPath()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestPathNodeCount, harness.RecomputedChainPerNode());
        Assert.Equal(harness.TreeFoldLongestPath(), harness.RecomputedChainPerNode());
    }

    [Fact]
    public void TreeFoldLongestPath_SmallestNodeCount_AgreesWithRecomputedChainPerNode()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestPathNodeCount, harness.TreeFoldLongestPath());
        Assert.Equal(harness.RecomputedChainPerNode(), harness.TreeFoldLongestPath());
    }

    private static LongestPathWithDifferentAdjacentCharactersBenchmarks BuildHarness()
    {
        var harness = new LongestPathWithDifferentAdjacentCharactersBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
