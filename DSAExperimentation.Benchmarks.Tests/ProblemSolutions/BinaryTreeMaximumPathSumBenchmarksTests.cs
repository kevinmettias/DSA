using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinaryTreeMaximumPathSumBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile it against and the assertion has to come
// from what the class comment makes decisive instead - the tree is LeetCode 124's own second
// example, whose best path the comment names as 15 -> 20 -> 7. The class carries no [Params]: the
// fixed five-node literal is the whole workload, so the same tree must always report the same sum.
public sealed partial class BinaryTreeMaximumPathSumBenchmarksTests
{
    private const int ExpectedMaximumPathSum = 42;

    [Fact]
    public void Setup_ExampleTree_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().GainRecursion(), BuildHarness().GainRecursion());

    [Fact]
    public void GainRecursion_PathBendingThroughTheRightChild_ReturnsTheExampleSum()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMaximumPathSum, harness.GainRecursion());
    }

    private static BinaryTreeMaximumPathSumBenchmarks BuildHarness()
    {
        var harness = new BinaryTreeMaximumPathSumBenchmarks();
        harness.Setup();

        return harness;
    }
}
