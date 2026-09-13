using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.CountGoodNodesInBinaryTree;

// LeetCode 1448. Count Good Nodes in Binary Tree: a node is good when no node on
// the path from the root down to it carries a strictly larger value, so the answer
// is a count over "max value seen among strict ancestors" - state that flows DOWN a
// single root-to-node path and never sideways between siblings.
//
// Both strategies thread exactly that state and differ only in what carries it: the
// call stack's own parameters, or this repo's own ITopDownHooks, whose Descend is
// the inherited-attribute contract this shape exists for (the same composition
// FindElementsInAContaminatedBinaryTree, LC 1261, uses for its recovered values).
internal static class CountGoodNodesInBinaryTreeSolution
{
    // The textbook answer: a plain recursive DFS passing the running maximum through
    // call-stack parameters and summing the good nodes on the way back up.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed traversal below has to justify itself against.
    public static int CountGoodNodesByRecursiveDfs(BinaryTreeNode<int>? root)
        => CountFrom(root, int.MinValue);

    // The composed answer: TopDownTraversal threads the inherited maximum for us -
    // Descend folds a parent's own value into what its children inherit, Visit bumps
    // a shared counter whenever the node it is handed is not outranked by any strict
    // ancestor - so this strategy contributes only the seed (int.MinValue, so the
    // root is always good) and the tally.
    public static int CountGoodNodesByTopDownTraversal(BinaryTreeNode<int>? root)
    {
        var counter = new Counter();

        TopDownTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            GoodNodeHooks, (int MaxSoFar, Counter Good)>(root, (int.MinValue, counter));

        return counter.Count;
    }

    private static int CountFrom(BinaryTreeNode<int>? node, int maxSoFar)
    {
        if (node is null)
        {
            return 0;
        }

        var good = node.Value >= maxSoFar ? 1 : 0;
        var nextMax = Math.Max(maxSoFar, node.Value);

        return good + CountFrom(node.Left, nextMax) + CountFrom(node.Right, nextMax);
    }

    // ITopDownHooks.Visit returns nothing, so the tally has to live in a reference
    // the state tuple can carry unchanged down every path.
    private sealed class Counter
    {
        public int Count;
    }

    // A witness for this problem alone: "good" is LC 1448's own predicate, which is
    // why it stays beside the solution rather than in Algorithms/.
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
