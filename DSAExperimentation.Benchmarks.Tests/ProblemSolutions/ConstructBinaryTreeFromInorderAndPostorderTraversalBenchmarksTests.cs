using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructBinaryTreeFromInorderAndPostorderTraversalBenchmarks
// (ARCHITECTURE 17.9): the class has a single arm, so there is no second strategy to reconcile it
// against and the assertion has to come from what the class comment makes decisive instead - a
// balanced tree built by Fixtures.BinaryTrees, flattened into its own inorder/postorder pair, must
// rebuild into that same tree. BinaryTrees.Balanced puts value i at heap index i, so that tree's
// level order is exactly 0..NodeCount-1; a reconstruction that loses or reorders a subtree shows up
// here as a different level order.
//
// Setup is where the flattening happens, so the same NodeCount must rebuild the same pair of
// traversals - asserted through the one observable the arm exposes, its rebuilt tree.
public sealed partial class ConstructBinaryTreeFromInorderAndPostorderTraversalBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameInorderAndPostorder()
    {
        Assert.Equal(Enumerable.Range(0, SmallestNodeCount), LevelOrderValues(Rebuilt()));

        Assert.Equal(
            LevelOrderValues(Rebuilt()),
            LevelOrderValues(Rebuilt()));
    }

    [Fact]
    public void PostorderIndexMap_BalancedTreeFlattenedAndRebuilt_RebuildsThatTree() =>
        Assert.Equal(Enumerable.Range(0, SmallestNodeCount), LevelOrderValues(Rebuilt()));

    private static BinaryTreeNode<int>? Rebuilt() =>
        (BinaryTreeNode<int>?)BuildHarness().PostorderIndexMap();

    private static ConstructBinaryTreeFromInorderAndPostorderTraversalBenchmarks BuildHarness()
    {
        var harness = new ConstructBinaryTreeFromInorderAndPostorderTraversalBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    private static List<int> LevelOrderValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        var pending = new Queue<BinaryTreeNode<int>>();

        if (root is not null)
        {
            pending.Enqueue(root);
        }

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            values.Add(node.Value);

            if (node.Left is not null)
            {
                pending.Enqueue(node.Left);
            }

            if (node.Right is not null)
            {
                pending.Enqueue(node.Right);
            }
        }

        return values;
    }
}
