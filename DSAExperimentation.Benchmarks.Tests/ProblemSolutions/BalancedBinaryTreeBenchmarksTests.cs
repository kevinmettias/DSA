using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BalancedBinaryTreeBenchmarks (ARCHITECTURE 17.9): the class has two arms - the
// bottom-up single-pass recursion and the top-down re-measuring check - so the pair must agree as well
// as each matching the one tree Setup builds, which is LC 110's own first example: 3 with children 9 and
// 20, and 20 with children 15 and 7. That tree is balanced (every node's subtrees differ in height by at
// most one), so both verdicts are a decisive TRUE rather than a restatement of an arm. Setup builds it
// from fixed values with no [Params] at all, so the harness is a bare initializer plus Setup; each arm's
// only observable is its verdict, so the rebuild is witnessed through those verdicts.
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

    [Fact]
    public void TopDownHeightCheck_LeetCodeOneTenExampleOne_ReturnsTrue()
    {
        var harness = BuildHarness();

        Assert.True(harness.TopDownHeightCheck());
    }

    [Fact]
    public void TopDownHeightCheck_AgreesWithIsBalancedByHeightRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsBalancedByHeightRecursion(), harness.TopDownHeightCheck());
    }

    private static BalancedBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new BalancedBinaryTreeBenchmarks();
        harness.Setup();

        return harness;
    }
}
