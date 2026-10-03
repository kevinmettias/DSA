using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BinaryTreeRightSideView;

// LeetCode 199. Binary Tree Right Side View: the rightmost node's value at every
// depth, top to bottom.
//
// Two strategies sit here. RightSideViewByLevelGroupedTraversal uses the repo's
// LevelGroupedBreadthFirstTraversal, which already buffers a whole depth before
// firing its hook, so "rightmost per level" is just "last element of each buffered
// level" - the Hooks struct holds the result list it appends each level's last
// value to.
// RightSideViewByDepthFirstRightFirst takes the opposite tack: a depth-first walk
// that visits each right child before its left, so the first node it reaches at any
// depth is that level's rightmost.
internal static class BinaryTreeRightSideViewSolution
{
    // The textbook arm the buffered-level traversal is measured against: a
    // depth-first walk that descends right before left and records the first node
    // reached at each depth - which, right-first, is that level's rightmost. It
    // carries one call stack rather than one buffered level, and appends only once
    // per level.
    public static List<int> RightSideViewByDepthFirstRightFirst(BinaryTreeNode<int>? root)
    {
        var view = new List<int>();

        VisitRightFirst(root, 0, view);

        return view;
    }

    private static void VisitRightFirst(BinaryTreeNode<int>? node, int depth, List<int> view)
    {
        if (node is null)
        {
            return;
        }

        if (depth == view.Count)
        {
            view.Add(node.Value);
        }

        VisitRightFirst(node.Right, depth + 1, view);
        VisitRightFirst(node.Left, depth + 1, view);
    }

    public static List<int> RightSideViewByLevelGroupedTraversal(BinaryTreeNode<int>? root)
    {
        var rightmost = new List<int>();

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            Hooks>(root, new Hooks(rightmost));

        return rightmost;
    }

    private readonly struct Hooks(List<int> rightmost) : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => rightmost.Add(level[^1].Value);
    }
}
