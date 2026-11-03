using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.MinimumDepthOfBinaryTree;

// LeetCode 111. Minimum Depth of Binary Tree: the length of the shortest
// root-to-leaf path, counted in nodes. A node with only one child does not count
// as a leaf, so the walk cannot just take the min of both children's depths - a
// missing side must defer to whichever side is actually present.
//
// Two strategies sit here. MinDepthByRecursion is the natural recursive walk, but
// its single-child rule means it may descend the long side of such a node before
// finding the shallow leaf. MinDepthByBreadthFirstReduce is level-order - this repo's
// Reduce in BreadthFirstReduceOrder - so it stops the moment a leaf first surfaces and
// never visits a level below it.
// Pre-migration the test's private helper and both of the benchmark's [Benchmark]
// arms (RecursiveMinDepth, BinaryTreeNodeMinDepth) all called the identical
// recursive walk under different names; only the recursion predates this pair.
internal static class MinimumDepthOfBinaryTreeSolution
{
    // Level-order through this repo's Reduce in BreadthFirstReduceOrder: FirstLeaf is done
    // at the first leaf the walk enters, so it never touches the levels below the
    // shallowest one - unlike the recursion, which may follow the long side of a
    // one-child node down before reaching that leaf. Depth counts edges from the root, so
    // the leaf's depth in nodes is one more; no tree has depth 0.
    public static int MinDepthByBreadthFirstReduce(BinaryTreeNode<int>? node)
        => Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            BreadthFirstReduceOrder<BinaryTreeNode<int>>, FirstLeaf, int>(node);

    // The depth in nodes of the first leaf entered, 0 until there is one.
    private readonly struct FirstLeaf : IReduceAlgebra<BinaryTreeNode<int>, int>
    {
        public static int Seed => 0;

        public static bool StopsEarly => true;

        public static int Enter(int state, BinaryTreeNode<int> node, int depth)
        {
            var isLeaf = node.Left is null && node.Right is null;
            var depthInNodes = depth + 1;

            return isLeaf ? depthInNodes : state;
        }

        public static bool IsDone(int state) => state > 0;
    }

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
