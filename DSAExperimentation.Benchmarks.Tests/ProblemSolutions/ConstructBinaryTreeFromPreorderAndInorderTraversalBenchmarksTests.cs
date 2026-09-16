using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks (ARCHITECTURE
// 17.9): the class has a single arm, so there is no second strategy to reconcile it against and the
// assertion has to come from what the class comment makes decisive instead - a balanced tree built
// by Fixtures.BinaryTrees, flattened into its own preorder/inorder pair, must rebuild into that same
// tree, so the arm counts exactly NodeCount reconstructed nodes and no fewer.
//
// Setup is where the flattening happens, so the same NodeCount must rebuild the same pair of
// traversals - asserted through the one thing the arm reports, the reconstructed node count. That
// count is a proxy for the rebuilt tree, not the tree itself: agreement across two harnesses
// witnesses that the reconstruction is deterministic and complete, not that every parent and child
// landed where the flattened balanced tree had them.
public sealed partial class ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSamePreorderAndInorder()
    {
        Assert.Equal(SmallestNodeCount, BuildHarness().PreorderIndexMap());
        Assert.Equal(BuildHarness().PreorderIndexMap(), BuildHarness().PreorderIndexMap());
    }

    [Fact]
    public void PreorderIndexMap_BalancedTreeFlattenedAndRebuilt_RebuildsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().PreorderIndexMap());

    private static ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks BuildHarness()
    {
        var harness = new ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
