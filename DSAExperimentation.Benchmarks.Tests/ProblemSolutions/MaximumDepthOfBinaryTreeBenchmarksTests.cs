using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumDepthOfBinaryTreeBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for one question - the node count along the longest root-to-leaf path - so a harness
// whose arms disagree is timing two different problems.
//
// Setup turns a gapless level-order array into its tree, which is complete: every level is full but
// the last. Its depth therefore follows from the node count alone - levels 1 to 6 hold 63 nodes, so
// the smallest size's 100 nodes put the remaining 37 on level 7 - and is decisive at 7 whatever
// values were drawn. Setup's determinism is asserted against that literal as well as against a second
// build, since a rebuilt harness that reached a different depth would mean the workload itself was not
// fixed.
public sealed partial class MaximumDepthOfBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 100;
    private const int ExpectedDepth = 7;

    [Fact]
    public void Setup_SameTree_RebuildsTheSameDepth()
    {
        Assert.Equal(BuildHarness().RecursiveHeight(), BuildHarness().RecursiveHeight());
        Assert.Equal(ExpectedDepth, BuildHarness().RecursiveHeight());
    }

    [Fact]
    public void RecursiveHeight_AgreesWithTreeMetricsHeight()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveHeight(), harness.TreeMetricsHeight());
    }

    private static MaximumDepthOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MaximumDepthOfBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
