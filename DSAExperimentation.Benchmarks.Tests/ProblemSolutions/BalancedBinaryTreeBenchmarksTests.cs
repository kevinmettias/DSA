using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BalancedBinaryTreeBenchmarks (ARCHITECTURE 17.9): the class has two arms - the
// bottom-up single-pass recursion and the top-down re-measuring check - so the pair must agree as well
// as each matching the verdict the tree Setup builds makes decisive. Setup turns a gapless level-order
// array into its tree, and a gapless level order is a complete tree: every level full except possibly
// the last, which fills from the left, so no two sibling subtrees differ in height by more than one.
// Both verdicts are therefore a decisive TRUE derived from that shape rather than a restatement of an
// arm. Each arm's only observable is its verdict, so the rebuild is witnessed through those verdicts.
public sealed partial class BalancedBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTree() =>
        Assert.Equal(
            BuildHarness().IsBalancedByHeightRecursion(),
            BuildHarness().IsBalancedByHeightRecursion());

    [Fact]
    public void IsBalancedByHeightRecursion_CompleteTree_ReturnsTrue()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsBalancedByHeightRecursion());
    }

    [Fact]
    public void TopDownHeightCheck_CompleteTree_ReturnsTrue()
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
        var harness = new BalancedBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
