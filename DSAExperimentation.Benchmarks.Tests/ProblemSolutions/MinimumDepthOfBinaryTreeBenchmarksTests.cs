using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumDepthOfBinaryTreeBenchmarks (ARCHITECTURE 17.9): the baseline's depth
// against one derived from the tree's shape rather than from either arm, which the generic check
// cannot supply - it holds the level-order arm to the baseline. Setup turns a gapless level-order
// array into its tree, which is complete, so the decisive depth follows from the node count alone,
// whatever values were drawn: at the smallest size of 1,000 nodes, levels 1 to 9 hold 511, and the
// other 489 fill level 10 from the left as the children of the first 245 of level 9's 256 nodes.
// The last 11 nodes of level 9 are therefore leaves, and the shortest root-to-leaf path holds nine
// nodes.
public sealed partial class MinimumDepthOfBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 1_000;
    private const int ExpectedMinimumDepth = 9;

    [Fact]
    public void RecursiveMinDepth_CompleteTree_ReturnsTheShortestRootToLeafPath() =>
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().RecursiveMinDepth());

    private static MinimumDepthOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MinimumDepthOfBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
