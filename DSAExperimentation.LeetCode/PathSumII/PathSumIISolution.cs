using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.PathSumII;

// LeetCode 113. Path Sum II: every root-to-leaf path whose node values sum to a
// target, returned in full (not just counted or detected).
internal static class PathSumIISolution
{
    // Textbook DFS backtracking: carry the running path and remaining budget down
    // the recursion, record a copy at every leaf that zeroes it out. No repo
    // primitives beyond the tree node itself.
    public static List<List<int>> FindPathsByRecursiveBacktrack(BinaryTreeNode<int>? root, int targetSum)
    {
        var results = new List<List<int>>();

        Search(root, targetSum, [], results);

        return results;
    }

    // Compose Algorithms.Paths' AllRootToLeafPaths (every root-to-leaf path, already
    // proven for LC 112's single-sum question) and filter to the ones matching the
    // target - the same reduction LC 112's own coverage uses, just keeping every
    // match instead of asking whether one exists.
    public static List<List<int>> FindPathsByAllRootToLeafPaths(BinaryTreeNode<int>? root, int targetSum) =>
        AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root)
            .Where(path => path.Sum(node => node.Value) == targetSum)
            .Select(path => path.Select(node => node.Value).ToList())
            .ToList();

    private static void Search(BinaryTreeNode<int>? node, int remaining, List<int> path, List<List<int>> results)
    {
        if (node is null)
        {
            return;
        }

        path.Add(node.Value);
        remaining -= node.Value;

        if (IsMatchingLeaf(node, remaining))
        {
            results.Add([.. path]);
        }
        else
        {
            Search(node.Left, remaining, path, results);
            Search(node.Right, remaining, path, results);
        }

        path.RemoveAt(path.Count - 1);
    }

    // The path is a match only at a leaf, and only when its values have cancelled the
    // target sum exactly.
    private static bool IsMatchingLeaf(BinaryTreeNode<int> node, int remaining) =>
        node.Left is null && node.Right is null && remaining == 0;
}
