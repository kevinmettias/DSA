using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 337: a perfect binary tree of the given depth with random
// node values, so the Rob/NotRobbed choice at every node is genuinely data-dependent
// rather than following the same branch throughout.
internal static class HouseRobberIIIWorkloads
{
    private const int MaxNodeValueExclusive = 100;

    public static BinaryTreeNode<int> RandomFullTree(int depth, int seed)
    {
        var random = new Random(seed);
        return Build(depth, random);
    }

    private static BinaryTreeNode<int> Build(int depth, Random random)
    {
        var node = new BinaryTreeNode<int>(random.Next(1, MaxNodeValueExclusive));

        if (depth > 0)
        {
            node.Left = Build(depth - 1, random);
            node.Right = Build(depth - 1, random);
        }

        return node;
    }
}
