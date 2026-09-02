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
        Hooks.Result.Value = [];

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            Hooks>(root);

        return Hooks.Result.Value!;
    }

    private readonly struct Hooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<int>> Result = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) =>
            Result.Value!.Add(level[^1].Value);
    }
}
