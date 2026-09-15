using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.StepByStepDirectionsFromABinaryTreeNodeToAnother;

// LeetCode 2096. Step-By-Step Directions From a Binary Tree Node to Another:
// spell the route from start to dest as "U" (climb to the parent), "L" and "R"
// (descend into that child). Every route climbs to the two nodes' lowest common
// ancestor and then descends, so both strategies are really "find where the route
// turns around, then read off the two half-paths".
//
// LeetCode states the endpoints as node VALUES and guarantees they are unique in
// the tree, so each strategy carries the LeetCode-shaped overload plus the
// node-reference overload a caller that already holds the nodes wants (a
// benchmark's [GlobalSetup], say) - the two-overload shape section 17.4 describes.
internal static class StepByStepDirectionsFromABinaryTreeNodeToAnotherSolution
{
    private const char Up = 'U';
    private const char Left = 'L';
    private const char Right = 'R';
    private const string TargetNotReachableMessage = "target is not reachable from the given root.";

    // The textbook answer: one bespoke recursive find-path-from-the-root per
    // endpoint, then drop their common prefix - what is left of the start path is
    // the climb, what is left of the dest path is the descent. Deliberately
    // written without this repo's primitives, on plain BCL lists; it is the arm
    // the composed strategy below has to justify itself against.
    public static string GetDirectionsByPathSearch(BinaryTreeNode<int> root, int startValue, int destValue)
    {
        var start = FindNode(root, startValue);
        var dest = FindNode(root, destValue);

        return GetDirectionsByPathSearch(root, start, dest);
    }

    public static string GetDirectionsByPathSearch(
        BinaryTreeNode<int> root, BinaryTreeNode<int> start, BinaryTreeNode<int> dest)
    {
        var pathToStart = FindPath(root, start);
        var pathToDest = FindPath(root, dest);

        return BuildDirections(pathToStart, pathToDest);
    }

    private static List<BinaryTreeNode<int>> FindPath(BinaryTreeNode<int> root, BinaryTreeNode<int> target)
    {
        var path = new List<BinaryTreeNode<int>>();
        TryFindPath(root, target, path);

        return path;
    }

    private static bool TryFindPath(
        BinaryTreeNode<int> node, BinaryTreeNode<int> target, List<BinaryTreeNode<int>> path)
    {
        path.Add(node);

        if (node == target)
        {
            return true;
        }

        if (DescendsToTarget(node, target, path))
        {
            return true;
        }

        path.RemoveAt(path.Count - 1);
        return false;
    }

    // The target hangs off this node: the left child is tried before the right,
    // because each attempt appends to the shared path and rolls it back on failure.
    private static bool DescendsToTarget(
        BinaryTreeNode<int> node, BinaryTreeNode<int> target, List<BinaryTreeNode<int>> path)
        => (node.Left is not null && TryFindPath(node.Left, target, path))
            || (node.Right is not null && TryFindPath(node.Right, target, path));

    // The two paths still agree here: both have a step at this offset, and it is
    // the same node.
    private static bool PathsAgreeAt(
        List<BinaryTreeNode<int>> pathToStart, List<BinaryTreeNode<int>> pathToDest, int offset)
        => offset < pathToStart.Count
            && offset < pathToDest.Count
            && pathToStart[offset] == pathToDest[offset];

    // The shared prefix of the two root-to-node paths ends at the lowest common
    // ancestor, so its length is where the climb stops and the descent starts.
    private static string BuildDirections(
        List<BinaryTreeNode<int>> pathToStart, List<BinaryTreeNode<int>> pathToDest)
    {
        var commonLength = 0;

        while (PathsAgreeAt(pathToStart, pathToDest, commonLength))
        {
            commonLength++;
        }

        var up = new string(Up, pathToStart.Count - commonLength);
        var down = new char[pathToDest.Count - commonLength];

        for (var i = 0; i < down.Length; i++)
        {
            var parent = pathToDest[commonLength + i - 1];
            var child = pathToDest[commonLength + i];
            down[i] = parent.Left == child ? Left : Right;
        }

        return up + new string(down);
    }

    // This repo's own engines: LowestCommonAncestor.Find (the same ancestry walk
    // LowestCommonAncestorOfABinaryTreeSolution composes) locates the turnaround
    // point, and AllRootToLeafPaths.Find rooted AT that ancestor (the engine
    // PathSumII/PathSumIII/KthAncestorOfATreeNode already lean on) hands back the
    // chains both endpoints sit on. It collects EVERY leaf path under the ancestor
    // rather than just the two this problem needs, so it is expected to do
    // strictly more work than the bespoke search - the same "a generic primitive
    // proves feasibility, not necessarily minimal cost" trade-off
    // ReconstructItinerary documents for its own repo-primitive side.
    public static string GetDirectionsByLowestCommonAncestorPaths(
        BinaryTreeNode<int> root, int startValue, int destValue)
    {
        var start = FindNode(root, startValue);
        var dest = FindNode(root, destValue);

        return GetDirectionsByLowestCommonAncestorPaths(root, start, dest);
    }

    public static string GetDirectionsByLowestCommonAncestorPaths(
        BinaryTreeNode<int> root, BinaryTreeNode<int> start, BinaryTreeNode<int> dest)
    {
        var ancestor = FindLowestCommonAncestor(root, start, dest);
        var leafPaths = FindLeafPaths(ancestor);

        var pathToStart = PathTo(leafPaths, start);
        var pathToDest = PathTo(leafPaths, dest);

        var up = new string(Up, pathToStart.Length - 1);

        return up + BuildDownwardMoves(pathToDest);
    }

    private static BinaryTreeNode<int> FindLowestCommonAncestor(
        BinaryTreeNode<int> root, BinaryTreeNode<int> start, BinaryTreeNode<int> dest)
    {
        var ancestor = LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(
            root, start, dest);

        // presumption: allow -- start and dest are always nodes of the tree rooted
        // at root, so Find can only return null when neither is reachable
        // (impossible here), the same guarantee LowestCommonAncestorOfABinaryTree
        // relies on.
        return ancestor!;
    }

    private static List<BinaryTreeNode<int>[]> FindLeafPaths(BinaryTreeNode<int> ancestor)
        => AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(ancestor);

    private static string BuildDownwardMoves(BinaryTreeNode<int>[] pathToDest)
    {
        var down = new char[pathToDest.Length - 1];

        for (var i = 0; i < down.Length; i++)
        {
            var child = pathToDest[i + 1];
            down[i] = pathToDest[i].Left == child ? Left : Right;
        }

        return new string(down);
    }

    // Every leaf path AllRootToLeafPaths returns starts at the same root (here,
    // the lowest common ancestor); the chain from it down to target is whichever
    // leaf path contains target, truncated right after it.
    private static BinaryTreeNode<int>[] PathTo(
        List<BinaryTreeNode<int>[]> leafPaths, BinaryTreeNode<int> target)
    {
        foreach (var path in leafPaths)
        {
            var index = Array.IndexOf(path, target);

            if (index >= 0)
            {
                return path[..(index + 1)];
            }
        }

        throw new InvalidOperationException(TargetNotReachableMessage);
    }

    private static BinaryTreeNode<int> FindNode(BinaryTreeNode<int> root, int value)
    {
        var node = TryFindNode(root, value);

        // presumption: allow -- LeetCode guarantees startValue and destValue each
        // name a node of the given tree, so the walk always finds one.
        return node!;
    }

    private static BinaryTreeNode<int>? TryFindNode(BinaryTreeNode<int>? node, int value)
    {
        if (node is null)
        {
            return null;
        }

        return node.Value == value ? node : TryFindInSubtrees(node, value);
    }

    // The value is not at this node, so it is somewhere below it: the left child is
    // searched first, and the right one only while the left came back empty.
    private static BinaryTreeNode<int>? TryFindInSubtrees(BinaryTreeNode<int> node, int value) =>
        TryFindNode(node.Left, value) ?? TryFindNode(node.Right, value);
}
