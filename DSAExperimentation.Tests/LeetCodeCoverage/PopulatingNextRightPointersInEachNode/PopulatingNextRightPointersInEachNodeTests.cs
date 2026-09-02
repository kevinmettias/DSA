using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PopulatingNextRightPointersInEachNode;

// Harness only. Both strategies are PopulatingNextRightPointersInEachNodeSolution's
// - this file pins them to LeetCode's published examples. Examples are given as a
// perfect tree's values in array-heap order (BinaryTreeNode<int> is internal, so it
// cannot appear in a public TheoryData signature) alongside the expected value of
// each node's next pointer in that same order, null marking the rightmost node of
// its level.
public sealed class PopulatingNextRightPointersInEachNodeTests
{
    public static TheoryData<int[], int?[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, 6, 7], [null, 3, null, 5, 6, 7, null] },
            { [1, 2, 3], [null, 3, null] },
            { [1], [null] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByManualQueueBfs_LeetCodePerfectTrees_LinksEachNodeToItsRightNeighbor(
        int[] values, int?[] expectedNextValues)
    {
        var nodes = BuildPerfectTree(values);
        var next = PopulatingNextRightPointersInEachNodeSolution.ConnectByManualQueueBfs(nodes[0]);

        AssertNextValues(nodes, expectedNextValues, node => next.TryGetValue(node, out var n) ? n : null);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByLevelGroupedTraversal_LeetCodePerfectTrees_LinksEachNodeToItsRightNeighbor(
        int[] values, int?[] expectedNextValues)
    {
        var nodes = BuildPerfectTree(values);
        var next = PopulatingNextRightPointersInEachNodeSolution.ConnectByLevelGroupedTraversal(nodes[0]);

        AssertNextValues(nodes, expectedNextValues, node => next.TryGetValue(node, out var n) ? n : null);
    }

    private static void AssertNextValues(
        BinaryTreeNode<int>[] nodes,
        int?[] expectedNextValues,
        Func<BinaryTreeNode<int>, BinaryTreeNode<int>?> nextOf)
    {
        for (var i = 0; i < nodes.Length; i++)
        {
            Assert.Equal(expectedNextValues[i], nextOf(nodes[i])?.Value);
        }
    }

    // Array-heap layout: node i's children sit at 2i+1 and 2i+2, which is exactly a
    // perfect binary tree's breadth-first index order - values.Length is always
    // 2^k - 1 in the examples above, so every row is fully populated.
    private static BinaryTreeNode<int>[] BuildPerfectTree(int[] values)
    {
        var nodes = new BinaryTreeNode<int>[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            nodes[i] = new BinaryTreeNode<int>(values[i]);
        }

        for (var i = 0; i < nodes.Length; i++)
        {
            var left = 2 * i + 1;
            var right = 2 * i + 2;

            if (left < nodes.Length)
            {
                nodes[i].Left = nodes[left];
            }

            if (right < nodes.Length)
            {
                nodes[i].Right = nodes[right];
            }
        }

        return nodes;
    }
}
