using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.CompleteBinaryTreeInserter;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CompleteBinaryTreeInserter;

// Harness only. Both strategies are CompleteBinaryTreeInserterSolution's - this
// file pins them to LeetCode's published examples, given as the seed tree's
// level-order values (BinaryTreeNode<int> is internal, so it cannot appear in a
// public TheoryData signature; BuildComplete reconstructs it), the Insert sequence,
// the parent value each Insert must return, and the tree's level-order values
// afterwards, which is what proves each node landed in the next left-to-right slot.
public sealed class CompleteBinaryTreeInserterTests
{
    public static TheoryData<int[], int[], int[], int[]> Examples =>
        new()
        {
            { [1, 2], [3, 4], [1, 2], [1, 2, 3, 4] },
            { [1, 2, 3, 4, 5, 6], [7, 8, 9], [3, 4, 4], [1, 2, 3, 4, 5, 6, 7, 8, 9] },
            { [1], [2, 3, 4], [1, 1, 2], [1, 2, 3, 4] },
            { [1, 2, 3], [4], [2], [1, 2, 3, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBfsRescan_LeetCodeExamples_FillsLeftToRightAndReturnsParentValues(
        int[] seed, int[] inserts, int[] expectedParents, int[] expectedLevelOrder)
    {
        var root = BuildComplete(seed);

        AssertScript(
            CompleteBinaryTreeInserterSolution.CreateByBfsRescan(root),
            root,
            inserts,
            expectedParents,
            expectedLevelOrder);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByIncompleteQueue_LeetCodeExamples_FillsLeftToRightAndReturnsParentValues(
        int[] seed, int[] inserts, int[] expectedParents, int[] expectedLevelOrder)
    {
        var root = BuildComplete(seed);

        AssertScript(
            CompleteBinaryTreeInserterSolution.CreateByIncompleteQueue(root),
            root,
            inserts,
            expectedParents,
            expectedLevelOrder);
    }

    private static void AssertScript(
        ICompleteBinaryTreeInserter inserter,
        BinaryTreeNode<int> root,
        int[] inserts,
        int[] expectedParents,
        int[] expectedLevelOrder)
    {
        for (var i = 0; i < inserts.Length; i++)
        {
            Assert.Equal(expectedParents[i], inserter.Insert(inserts[i]));
        }

        Assert.Same(root, inserter.Root);
        Assert.Equal(expectedLevelOrder, LevelOrder(inserter.Root));
    }

    // The seed tree is complete, so its level-order values are exactly the array:
    // the node at index i takes 2i+1 and 2i+2 as its children when those exist.
    private static BinaryTreeNode<int> BuildComplete(int[] values)
    {
        var nodes = values.Select(value => new BinaryTreeNode<int>(value)).ToArray();

        for (var i = 0; i < nodes.Length; i++)
        {
            var left = (2 * i) + 1;
            var right = left + 1;
            nodes[i].Left = left < nodes.Length ? nodes[left] : null;
            nodes[i].Right = right < nodes.Length ? nodes[right] : null;
        }

        return nodes[0];
    }

    // Assertion scaffolding: reads the resulting tree back out in level order so the
    // placement of every inserted node is checked, not just the value Insert returned.
    private static int[] LevelOrder(BinaryTreeNode<int> root)
    {
        var values = new List<int>();
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            values.Add(node.Value);

            if (node.Left is not null)
            {
                queue.Enqueue(node.Left);
            }

            if (node.Right is not null)
            {
                queue.Enqueue(node.Right);
            }
        }

        return values.ToArray();
    }
}
