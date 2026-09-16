using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumDepthOfBinaryTreeBenchmarks (ARCHITECTURE 17.9): the class carries no
// [Params] at all, and after the redundant second arm was deleted it carries a single [Benchmark] -
// Setup builds the tree and RecursiveMinDepth walks it. With one arm there is no agreement to observe,
// so the assertion is the decisive literal the harness's own tree pins: root 1 has a leaf child 2 on
// the left and a child 3 whose only child 4 is a leaf on the right, so the shortest root-to-leaf path
// is 1 -> 2, two nodes. The tree is fixed, so a rebuilt harness must answer the same value.
public sealed partial class MinimumDepthOfBinaryTreeBenchmarksTests
{
    // The root's left child is a leaf and the right branch is one node deeper, so the shortest
    // root-to-leaf path - counted in nodes, per this problem's contract - is the two-node left one.
    private const int ExpectedMinimumDepth = 2;

    [Fact]
    public void Setup_FixedTree_RebuildsTheSameShortestRootToLeafPath()
    {
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().RecursiveMinDepth());
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().RecursiveMinDepth());
    }

    [Fact]
    public void RecursiveMinDepth_TwoLevelTree_ReturnsTheShortestRootToLeafPath() =>
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().RecursiveMinDepth());

    private static MinimumDepthOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MinimumDepthOfBinaryTreeBenchmarks();
        harness.Setup();

        return harness;
    }
}
