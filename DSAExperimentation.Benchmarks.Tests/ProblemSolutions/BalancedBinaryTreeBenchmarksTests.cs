using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BalancedBinaryTreeBenchmarks (ARCHITECTURE 17.9): the class has a single arm - the original
// benchmark's two [Benchmark] methods called the identical private helper - so there is no second strategy to
// reconcile it against and the assertion comes from the one tree Setup builds, which is LC 110's own first
// example: 3 with children 9 and 20, and 20 with children 15 and 7. That tree is balanced (every node's subtrees
// differ in height by at most one), so the verdict is a decisive TRUE rather than a restatement of the arm.
// Setup builds it from fixed values with no [Params] at all, so the harness is a bare initializer plus Setup; the
// arm's only observable is its verdict, so the rebuild is witnessed through that verdict.
public sealed partial class BalancedBinaryTreeBenchmarksTests
{
    [Fact]
    public void Setup_FixedExampleTree_RebuildsTheSameTree() =>
        Assert.Equal(
            BuildHarness().IsBalancedByHeightRecursion(),
            BuildHarness().IsBalancedByHeightRecursion());

    [Fact]
    public void IsBalancedByHeightRecursion_LeetCodeOneTenExampleOne_ReturnsTrue()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsBalancedByHeightRecursion());
    }

    private static BalancedBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new BalancedBinaryTreeBenchmarks();
        harness.Setup();

        return harness;
    }
}
