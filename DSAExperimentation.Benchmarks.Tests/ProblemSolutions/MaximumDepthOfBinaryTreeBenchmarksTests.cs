using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumDepthOfBinaryTreeBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for one question - the node count along the longest root-to-leaf path - so a harness
// whose arms disagree is timing two different problems.
//
// This class has no [Params]: Setup builds one fixed five-node tree, the one LeetCode 104's own first
// example documents (3 over 9 and 20, with 20 over 15 and 7), so the depth is decisive at 3 and needs
// no tuning to be reproducible. Setup's determinism is asserted against that literal as well as
// against a second build, since a rebuilt harness that reached a different depth would mean the
// workload itself was not fixed.
public sealed partial class MaximumDepthOfBinaryTreeBenchmarksTests
{
    private const int ExpectedDepth = 3;

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

    [Fact]
    public void TreeMetricsHeight_AgreesWithRecursiveHeight()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TreeMetricsHeight(), harness.RecursiveHeight());
    }

    private static MaximumDepthOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MaximumDepthOfBinaryTreeBenchmarks();
        harness.Setup();

        return harness;
    }
}
