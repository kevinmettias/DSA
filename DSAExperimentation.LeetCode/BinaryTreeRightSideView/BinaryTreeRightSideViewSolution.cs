using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BinaryTreeRightSideView;

// LeetCode 199. Binary Tree Right Side View: the rightmost node's value at every
// depth, top to bottom.
//
// LevelGroupedBreadthFirstTraversal already buffers a whole depth before firing
// its hook, so "rightmost per level" is just "last element of each buffered
// level" - the Hooks struct exists only because THooks must be a
// static-interface-generic parameter, not an ordinary closure, so the result
// list has to live behind an AsyncLocal instead of a captured local.
internal static class BinaryTreeRightSideViewSolution
{
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
