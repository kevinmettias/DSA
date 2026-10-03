using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks (ARCHITECTURE
// 17.9): the class has a single arm, so there is no second strategy to reconcile it against and the
// assertion has to come from what the class comment makes decisive instead - a balanced tree built
// by Fixtures.BinaryTrees, flattened into its own preorder/inorder pair, must rebuild into that same
// tree, every node in its place, and so with exactly NodeCount nodes. The arm returns the rebuilt
// root as object? (the node type is internal, CS0050), so both trees are compared in LeetCode's
// level-order notation.
public sealed partial class ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void PreorderIndexMap_BalancedTreeFlattenedAndRebuilt_RebuildsEveryNodeInPlace()
    {
        var rebuilt = LeetCodeWireFormat.FromBinaryTree(Assert.IsType<BinaryTreeNode<int>>(BuildHarness().PreorderIndexMap()));

        Assert.Equal(SmallestNodeCount, rebuilt.Length);
        Assert.Equal(LeetCodeWireFormat.FromBinaryTree(BinaryTrees.Balanced(SmallestNodeCount)), rebuilt);
    }

    private static ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks BuildHarness()
    {
        var harness = new ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
