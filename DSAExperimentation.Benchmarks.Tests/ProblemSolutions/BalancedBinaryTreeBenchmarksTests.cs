using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BalancedBinaryTreeBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to
// each other, and this pins the baseline to the verdict the tree Setup builds makes decisive. Setup turns a
// gapless level-order array into its tree, and a gapless level order is a complete tree: every level full
// except possibly the last, which fills from the left, so no two sibling subtrees differ in height by more
// than one. The verdict is therefore a decisive TRUE derived from that shape rather than a restatement of an
// arm.
public sealed partial class BalancedBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void IsBalancedByHeightRecursion_CompleteTree_ReturnsTrue()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsBalancedByHeightRecursion());
    }

    private static BalancedBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new BalancedBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
