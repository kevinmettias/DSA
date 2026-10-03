using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumDepthOfBinaryTreeBenchmarks (ARCHITECTURE 17.9). Its two arms are
// competing strategies for the same question - the recursive walk and the level-order walk - so a
// harness whose arms disagree is measuring two different trees: both must report the same depth.
// Setup turns a gapless level-order array into its tree, which is complete, so the decisive depth
// follows from the node count alone, whatever values were drawn: at the smallest size of 1,000
// nodes, levels 1 to 9 hold 511, and the other 489 fill level 10 from the left as the children of
// the first 245 of level 9's 256 nodes. The last 11 nodes of level 9 are therefore leaves, and the
// shortest root-to-leaf path holds nine nodes. The tree is fixed by its seed, so a rebuilt harness
// must answer the same value.
public sealed partial class MinimumDepthOfBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 1_000;
    private const int ExpectedMinimumDepth = 9;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameShortestRootToLeafPath()
    {
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().RecursiveMinDepth());
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().RecursiveMinDepth());
    }

    [Fact]
    public void RecursiveMinDepth_CompleteTree_ReturnsTheShortestRootToLeafPath() =>
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().RecursiveMinDepth());

    [Fact]
    public void BreadthFirstSearch_CompleteTree_ReturnsTheShortestRootToLeafPath() =>
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().BreadthFirstSearch());

    [Fact]
    public void BreadthFirstSearch_AgreesWithRecursiveMinDepth()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveMinDepth(), harness.BreadthFirstSearch());
    }

    private static MinimumDepthOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MinimumDepthOfBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
