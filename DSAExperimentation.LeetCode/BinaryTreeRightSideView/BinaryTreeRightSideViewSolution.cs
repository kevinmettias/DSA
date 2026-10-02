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
// level" - the Hooks struct exists only because THooks must be a
// static-interface-generic parameter, not an ordinary closure, so the result list
// has to live behind an AsyncLocal instead of a captured local.
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
        Hooks.BeginCapture();

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            Hooks>(root);

        return Hooks.CapturedLevels;
    }

    private readonly struct Hooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        // Static because ILevelGroupedHooks is static-abstract - there is no Hooks instance
        // that could own the buffer - and AsyncLocal is what keeps it safe: the list belongs
        // to the flow that started the Walk, so a traversal on another thread reads its own.
        // Private, with BeginCapture/CapturedLevels the only way in and out: the buffer exists
        // for this one caller's start-and-read pair, so nothing outside the hook needs to name
        // it. The AsyncLocal itself is the mechanism suppressions.json names against
        // check-scope-discipline's static-state rule; keeping the field private narrows that
        // claim to this type rather than widening it.
        private static readonly AsyncLocal<List<int>> Result = new();

        public static List<int> CapturedLevels => Result.Value!;

        public static void BeginCapture() => Result.Value = [];

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) =>
            Result.Value!.Add(level[^1].Value);
    }
}
