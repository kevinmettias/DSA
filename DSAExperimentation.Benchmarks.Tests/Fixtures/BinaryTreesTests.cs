using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BinaryTrees (ARCHITECTURE 17.7). The two shapes put the fold strategies on
// opposite ends of the recursion-depth axis, and that is exactly what a test here owes: a complete
// tree whose depth stays within the logarithmic bound, and a degenerate right-only chain whose
// depth grows one step per node - the case IterativeFoldEvaluation exists for.
public sealed partial class BinaryTreesTests
{
    private const int NodeCount = 64;

    [Fact]
    public void Balanced_NodeCount_ReturnsACompleteTreeWithThatManyNodes()
    {
        var root = BinaryTrees.Balanced(NodeCount);

        Assert.Equal(NodeCount, CountNodes(root));
        Assert.Equal(Enumerable.Range(0, NodeCount), LevelOrderValues(root));
    }

    // A binary tree with n nodes and height floor(log2 n) is complete: every level above the last
    // must be full. Together with the node count that is the whole "balanced, easy case" claim.
    [Fact]
    public void Balanced_NodeCount_KeepsTheDepthAtTheLogarithmicBound() =>
        Assert.Equal(FloorLogarithm(NodeCount), Height(BinaryTrees.Balanced(NodeCount)));

    [Fact]
    public void Skewed_NodeCount_ReturnsARightOnlyChainWithThatManyNodes()
    {
        var root = BinaryTrees.Skewed(NodeCount);

        Assert.Equal(NodeCount, CountNodes(root));
        Assert.Equal(Enumerable.Range(0, NodeCount), LevelOrderValues(root));
    }

    [Fact]
    public void Skewed_NodeCount_PutsTheDepthAtOneStepPerNode() =>
        Assert.Equal(NodeCount - 1, Height(BinaryTrees.Skewed(NodeCount)));

    [Fact]
    public void Skewed_EveryNode_HasNoLeftChild() =>
        Assert.All(Nodes(BinaryTrees.Skewed(NodeCount)), node => Assert.Null(node.Left));

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);

    private static List<int> LevelOrderValues(BinaryTreeNode<int> root) =>
        [.. Nodes(root).Select(node => node.Value)];

    private static int Height(BinaryTreeNode<int>? node) =>
        node is null ? -1 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    private static int FloorLogarithm(int value)
    {
        var exponent = 0;

        while ((1 << (exponent + 1)) <= value)
        {
            exponent++;
        }

        return exponent;
    }

    private static List<BinaryTreeNode<int>> Nodes(BinaryTreeNode<int> root)
    {
        var nodes = new List<BinaryTreeNode<int>>();
        var pending = new Queue<BinaryTreeNode<int>>();
        pending.Enqueue(root);

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            nodes.Add(node);

            if (node.Left is not null)
            {
                pending.Enqueue(node.Left);
            }

            if (node.Right is not null)
            {
                pending.Enqueue(node.Right);
            }
        }

        return nodes;
    }
}
