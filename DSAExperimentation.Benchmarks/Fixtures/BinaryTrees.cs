using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

internal static class BinaryTrees
{
    // A complete, perfectly balanced tree - the easy case for both fold strategies,
    // recursion depth stays O(log n).
    public static BinaryTreeNode<int> Balanced(int nodeCount)
    {
        var nodes = new BinaryTreeNode<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new BinaryTreeNode<int>(i);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            var left = AlgorithmConstants.BranchingFactor * i + 1;
            var right = left + 1;
            nodes[i].Left = left < nodeCount ? NodeAt(nodes, left) : null;
            nodes[i].Right = right < nodeCount ? NodeAt(nodes, right) : null;
        }

        return nodes[0];
    }

    private static BinaryTreeNode<int> NodeAt(BinaryTreeNode<int>[] nodes, int index) => nodes[index];

    // A degenerate, right-only chain - the case IterativeFoldEvaluation exists for
    // (RecursiveFoldEvaluation's call stack grows with nodeCount here, not log(nodeCount)).
    public static BinaryTreeNode<int> Skewed(int nodeCount)
    {
        var root = new BinaryTreeNode<int>(0);
        var current = root;

        for (var i = 1; i < nodeCount; i++)
        {
            var next = new BinaryTreeNode<int>(i);
            current.Right = next;
            current = next;
        }

        return root;
    }
}
