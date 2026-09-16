using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountGoodNodesInBinaryTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a plain recursive DFS threading the running maximum
// through call-stack parameters against this repo's TopDownTraversal threading it through
// ITopDownHooks.Descend - so a harness whose arms disagree is timing two different problems. Both
// arms return an int, so they are compared directly. Setup draws from one fixed seed, so the same
// NodeCount must rebuild the same tree, and its documented shape is that the count is neither
// vacuous nor trivially every node: the root is always good against the int.MinValue seed, while the
// deep nodes below it are not.
public sealed partial class CountGoodNodesInBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSamePartlyGoodTree()
    {
        Assert.Equal(BuildHarness().RecursiveDfs(), BuildHarness().RecursiveDfs());

        Assert.InRange(BuildHarness().TopDownTraversalCount(), 1, SmallestNodeCount - 1);
    }

    [Fact]
    public void RecursiveDfs_TwoHundredNodeTree_AgreesWithTopDownTraversalCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TopDownTraversalCount(), harness.RecursiveDfs());
    }

    [Fact]
    public void TopDownTraversalCount_TwoHundredNodeTree_AgreesWithRecursiveDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveDfs(), harness.TopDownTraversalCount());
    }

    private static CountGoodNodesInBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new CountGoodNodesInBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
