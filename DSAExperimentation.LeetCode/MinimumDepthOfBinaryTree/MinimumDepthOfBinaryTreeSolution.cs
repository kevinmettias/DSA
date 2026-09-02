using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumDepthOfBinaryTree;

// LeetCode 111. Minimum Depth of Binary Tree: the length of the shortest
// root-to-leaf path, counted in nodes. A node with only one child does not count
// as a leaf, so the walk cannot just take the min of both children's depths - a
// missing side must defer to whichever side is actually present.
//
// There is exactly one strategy: pre-migration the test's private helper and both
// of the benchmark's [Benchmark] arms (RecursiveMinDepth, BinaryTreeNodeMinDepth)
// all called this identical recursive walk under different names, so nothing
// needed reconciling beyond naming it once and deleting the redundant second arm -
// the same SameTreeSolution shape.
internal static class MinimumDepthOfBinaryTreeSolution
{
    public static int MinDepthByRecursion(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return 0;
        }

        if (node.Left is null)
        {
            return 1 + MinDepthByRecursion(node.Right);
        }

        if (node.Right is null)
        {
            return 1 + MinDepthByRecursion(node.Left);
        }

        return 1 + Math.Min(MinDepthByRecursion(node.Left), MinDepthByRecursion(node.Right));
    }
}
