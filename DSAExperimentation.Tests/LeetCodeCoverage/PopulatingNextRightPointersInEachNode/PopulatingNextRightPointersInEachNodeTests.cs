using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PopulatingNextRightPointersInEachNode;

// Harness only. Both strategies are PopulatingNextRightPointersInEachNodeSolution's
// - this file pins them to LeetCode's published examples. Examples are given as a
// perfect tree's values in array-heap order (BinaryTreeNode<int> is internal, so it
// cannot appear in a public TheoryData signature) alongside the expected value of
// each node's next pointer in that same order, null marking the rightmost node of
// its level. As in the sibling PopulatingNextRightPointersInEachNodeIITests, the
// chain a strategy produced is read through a named strategy type rather than a
// bare Func.
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

        AssertNextValues(nodes, expectedNextValues, new DictionaryNextPointers(next));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByLevelGroupedTraversal_LeetCodePerfectTrees_LinksEachNodeToItsRightNeighbor(
        int[] values, int?[] expectedNextValues)
    {
        var nodes = BuildPerfectTree(values);
        var next = PopulatingNextRightPointersInEachNodeSolution.ConnectByLevelGroupedTraversal(nodes[0]);

        AssertNextValues(nodes, expectedNextValues, new HashMapNextPointers(next));
    }

    private static void AssertNextValues(
        BinaryTreeNode<int>[] nodes,
        int?[] expectedNextValues,
        NextPointerChain chain)
    {
        for (var i = 0; i < nodes.Length; i++)
        {
            Assert.Equal(expectedNextValues[i], chain.NextOf(nodes[i])?.Value);
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
            LinkChildren(nodes, i);
        }

        return nodes;
    }

    private static void LinkChildren(BinaryTreeNode<int>[] nodes, int i)
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

    // The next-pointer chain one strategy produced, read as a lookup from a node to the
    // node to its right, or null at the end of that node's level. A named strategy type
    // rather than a bare Func: the method says what is being asked for, and the contract
    // below has somewhere to live.
    private abstract class NextPointerChain
    {
        // Null is the end of a level, so a strategy that skipped a node must not be able
        // to read as one: a missing entry is the strategy breaking the contract every
        // implementation here is written against, which is what a guard reports.
        public BinaryTreeNode<int>? NextOf(BinaryTreeNode<int> node)
        {
            if (TryNextOf(node, out var nextNode))
            {
                return nextNode;
            }

            throw new InvalidOperationException(
                "the strategy returned no next pointer for a node of the tree it was given");
        }

        protected abstract bool TryNextOf(BinaryTreeNode<int> node, out BinaryTreeNode<int>? nextNode);
    }

    // ConnectByManualQueueBfs's result, keyed by the node each pointer belongs to.
    private sealed class DictionaryNextPointers(Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
        : NextPointerChain
    {
        protected override bool TryNextOf(BinaryTreeNode<int> node, out BinaryTreeNode<int>? nextNode)
            => next.TryGetValue(node, out nextNode);
    }

    // ConnectByLevelGroupedTraversal's result: the same try-get surface, on this
    // repository's own HashMap rather than a Dictionary.
    private sealed class HashMapNextPointers(HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
        : NextPointerChain
    {
        protected override bool TryNextOf(BinaryTreeNode<int> node, out BinaryTreeNode<int>? nextNode)
            => next.TryGetValue(node, out nextNode);
    }
}
