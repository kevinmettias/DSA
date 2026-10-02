using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumDepthOfBinaryTree;

// LeetCode 111. Minimum Depth of Binary Tree: the length of the shortest
// root-to-leaf path, counted in nodes. A node with only one child does not count
// as a leaf, so the walk cannot just take the min of both children's depths - a
// missing side must defer to whichever side is actually present.
//
// Two strategies sit here. MinDepthByRecursion is the natural recursive walk, but
// its single-child rule means it may descend the long side of such a node before
// finding the shallow leaf. MinDepthByBreadthFirstSearch is level-order, so it
// returns the moment a leaf first surfaces and never visits a level below it.
// Pre-migration the test's private helper and both of the benchmark's [Benchmark]
// arms (RecursiveMinDepth, BinaryTreeNodeMinDepth) all called the identical
// recursive walk under different names; only the recursion predates this pair.
internal static class MinimumDepthOfBinaryTreeSolution
{
    // The textbook arm the recursive walk is measured against: level-order traversal
    // that stops at the first leaf, so it never touches the levels below the
    // shallowest one - unlike the recursion, which may follow the long side of a
    // one-child node down before reaching that leaf.
    public static int MinDepthByBreadthFirstSearch(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return 0;
        }

        var frontier = new Queue<BinaryTreeNode<int>>();
        frontier.Enqueue(node);
        var depth = 0;

        while (frontier.Count > 0)
        {
            depth++;
            var levelCount = frontier.Count;

            for (var i = 0; i < levelCount; i++)
            {
                var current = frontier.Dequeue();

                if (current.Left is null && current.Right is null)
                {
                    return depth;
                }

                if (current.Left is not null)
                {
                    frontier.Enqueue(current.Left);
                }

                if (current.Right is not null)
                {
                    frontier.Enqueue(current.Right);
                }
            }
        }

        return depth;
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
