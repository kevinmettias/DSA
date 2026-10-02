using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumDepthOfBinaryTreeBenchmarks (ARCHITECTURE 17.9). Its two arms are
// competing strategies for the same question - the recursive walk and the level-order walk - so a
// harness whose arms disagree is measuring two different trees: both must report the same depth. The
// class carries no [Params] at all; Setup builds the tree and each arm walks it. The decisive literal
// the harness's own tree pins is that root 1 has a leaf child 2 on the left and a child 3 whose only
// child 4 is a leaf on the right, so the shortest root-to-leaf path is 1 -> 2, two nodes. The tree is
// fixed, so a rebuilt harness must answer the same value.
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

    [Fact]
    public void BreadthFirstSearch_TwoLevelTree_ReturnsTheShortestRootToLeafPath() =>
        Assert.Equal(ExpectedMinimumDepth, BuildHarness().BreadthFirstSearch());

    [Fact]
    public void BreadthFirstSearch_AgreesWithRecursiveMinDepth()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveMinDepth(), harness.BreadthFirstSearch());
    }

    private static MinimumDepthOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MinimumDepthOfBinaryTreeBenchmarks();
        harness.Setup();

        return harness;
    }
}
