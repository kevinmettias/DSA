using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountGoodNodesInBinaryTree;

// LeetCode 1448. Count Good Nodes in Binary Tree: this repo's own TopDownTraversal
// threads "max value seen among strict ancestors" down from root to every node -
// exactly the inherited root-to-node state ITopDownHooks exists for (same shape as
// FindElementsInAContaminatedBinaryTreeTests' recovered-value threading, LC 1261).
// Visit bumps a shared counter whenever the current node's value is >= that
// inherited max; Descend folds the parent's own value into the max passed to its
// children, so a node is "good" iff no strict ancestor outranks it.
public sealed partial class CountGoodNodesInBinaryTreeTests
{
    [Fact]
    public void GoodNodes_ClassicExample_ReturnsFour()
        => Assert.Equal(4, GoodNodes(Tree()));

    [Fact]
    public void GoodNodes_StrictlyDecreasingPath_OnlyRootIsGood()
        => Assert.Equal(1, GoodNodes(new(3) { Left = new(1) { Left = new(0) } }));

    [Fact]
    public void GoodNodes_SingleNode_ReturnsOne()
        => Assert.Equal(1, GoodNodes(new(5)));

    private static BinaryTreeNode<int> Tree()
        => new(3)
        {
            Left = new(1) { Right = new(3) },
            Right = new(4) { Left = new(1), Right = new(5) },
        };

    private static int GoodNodes(BinaryTreeNode<int> root)
    {
        var counter = new Counter();

        TopDownTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            GoodNodeHooks, (int MaxSoFar, Counter Good)>(root, (int.MinValue, counter));

        return counter.Count;
    }

    private sealed class Counter
    {
        public int Count;
    }

    private readonly struct GoodNodeHooks : ITopDownHooks<BinaryTreeNode<int>, (int MaxSoFar, Counter Good)>
    {
        public static void Visit(
            BinaryTreeNode<int> node, (int MaxSoFar, Counter Good) state, int depth, NodePosition position)
        {
            if (node.Value >= state.MaxSoFar)
            {
                state.Good.Count++;
            }
        }

        public static (int MaxSoFar, Counter Good) Descend(
            BinaryTreeNode<int> parent, (int MaxSoFar, Counter Good) parentState, BinaryTreeNode<int> child)
            => (Math.Max(parentState.MaxSoFar, parent.Value), parentState.Good);
    }
}
