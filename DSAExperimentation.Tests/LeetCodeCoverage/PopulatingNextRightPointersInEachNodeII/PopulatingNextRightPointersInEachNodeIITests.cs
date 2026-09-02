using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PopulatingNextRightPointersInEachNodeII;

// Harness only. Both strategies are
// PopulatingNextRightPointersInEachNodeIISolution's - this file pins them to
// LeetCode's published examples, given in LeetCode's own level-order-with-null
// array shape (BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData signature; BuildTree reconstructs it). Every value in an example tree
// is distinct, so a value can stand in for its node's identity when stating the
// expected next-pointer chain.
public sealed class PopulatingNextRightPointersInEachNodeIITests
{
    public static TheoryData<int?[], (int Value, int? NextValue)[]> Examples =>
        new()
        {
            // [1,2,3,4,5,null,7] -> 5's next must "reach across" 3's missing left
            // child to land on 7.
            {
                [1, 2, 3, 4, 5, null, 7],
                [(1, null), (2, 3), (3, null), (4, 5), (5, 7), (7, null)]
            },
            { [], [] },
            { [1], [(1, null)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByManualQueueBfs_LeetCodeExamples_LinksEachNodeToItsRightNeighbor(
        int?[] values, (int Value, int? NextValue)[] expectedNext)
    {
        var (root, byValue) = BuildTree(values);
        var next = PopulatingNextRightPointersInEachNodeIISolution.ConnectByManualQueueBfs(root);

        AssertLinks(expectedNext, byValue, node => next.TryGetValue(node, out var nextNode) ? nextNode : throw new KeyNotFoundException());
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByLevelGroupedTraversal_LeetCodeExamples_LinksEachNodeToItsRightNeighbor(
        int?[] values, (int Value, int? NextValue)[] expectedNext)
    {
        var (root, byValue) = BuildTree(values);
        var next = PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(root);

        AssertLinks(expectedNext, byValue, node => next.TryGetValue(node, out var nextNode) ? nextNode : throw new KeyNotFoundException());
    }

    private static void AssertLinks(
        (int Value, int? NextValue)[] expectedNext,
        Dictionary<int, BinaryTreeNode<int>> byValue,
        Func<BinaryTreeNode<int>, BinaryTreeNode<int>?> nextOf)
    {
        foreach (var (value, nextValue) in expectedNext)
        {
            var actual = nextOf(byValue[value]);
            var expected = nextValue is int expectedValue ? byValue[expectedValue] : null;
            Assert.Equal(expected, actual);
        }
    }

    // LeetCode's level-order array shape: each existing node consumes exactly two
    // subsequent slots for its children, null marking a missing one.
    private static (BinaryTreeNode<int>? Root, Dictionary<int, BinaryTreeNode<int>> ByValue) BuildTree(int?[] values)
    {
        var byValue = new Dictionary<int, BinaryTreeNode<int>>();

        if (values.Length == 0 || values[0] is null)
        {
            return (null, byValue);
        }

        var root = new BinaryTreeNode<int>(values[0]!.Value);
        byValue[root.Value] = root;
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var i = 1;

        while (queue.Count > 0 && i < values.Length)
        {
            var node = queue.Dequeue();

            if (values[i] is int leftValue)
            {
                node.Left = new BinaryTreeNode<int>(leftValue);
                byValue[leftValue] = node.Left;
                queue.Enqueue(node.Left);
            }

            i++;

            if (i < values.Length && values[i] is int rightValue)
            {
                node.Right = new BinaryTreeNode<int>(rightValue);
                byValue[rightValue] = node.Right;
                queue.Enqueue(node.Right);
            }

            i++;
        }

        return (root, byValue);
    }
}
