using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumDepthOfBinaryTreeBenchmarks (ARCHITECTURE 17.9): Setup turns a gapless
// level-order array into its tree, which is complete: every level is full but the last. Its depth
// therefore follows from the node count alone - levels 1 to 6 hold 63 nodes, so the smallest size's
// 100 nodes put the remaining 37 on level 7 - and is decisive at 7 whatever values were drawn: an
// expected value the generic agreement check, which only compares arms with each other, cannot
// supply.
public sealed partial class MaximumDepthOfBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 100;
    private const int ExpectedDepth = 7;

    [Fact]
    public void RecursiveHeight_CompleteTree_ReachesTheSeventhLevel() =>
        Assert.Equal(ExpectedDepth, BuildHarness().RecursiveHeight());

    private static MaximumDepthOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MaximumDepthOfBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
