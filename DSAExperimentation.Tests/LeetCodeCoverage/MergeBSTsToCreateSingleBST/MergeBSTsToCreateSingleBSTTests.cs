using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MergeBSTsToCreateSingleBST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeBSTsToCreateSingleBST;

// Harness only. Both strategies live in MergeBSTsToCreateSingleBSTSolution; this
// file pins them to the same examples, so a failure names the strategy that broke.
// The linear rescan was previously only a [Benchmark(Baseline = true)] arm - and one
// that answered a weaker question, assuming the first tree was the overall root and
// never checking BST order - so these are its first assertions.
//
// Trees are stated in LeetCode's own level-order-with-null array shape, because
// BinaryTreeNode<int> is internal and cannot appear in a public TheoryData
// signature; BuildForest reconstructs them inside each test method, which also gives
// every strategy its own forest to splice (the merge mutates Left/Right in place).
// An empty expected array means "no valid merge", which is how LeetCode renders it.
public sealed partial class MergeBSTsToCreateSingleBSTTests
{
    public static TheoryData<int?[][], int?[]> Examples =>
        new()
        {
            // LeetCode example 1: trees = [[2,1],[3,2,5],[5,4]].
            { [[2, 1], [3, 2, 5], [5, 4]], [3, 2, 5, 1, null, 4] },

            // LeetCode example 2: merging is possible but puts 6 in 5's left
            // subtree, so the result is not a BST.
            { [[5, 3, 8], [3, 2, 6]], [] },

            // LeetCode example 3: no leaf matches another tree's root.
            { [[5, 4], [3]], [] },

            // treeA's leaf 1 is where treeB's root 1 attaches.
            { [[2, 1, 4], [1, 0]], [2, 1, 4, 0] },

            // Neither root is anyone's leaf, so there is no single overall root.
            { [[10, 5], [20, 15]], [] },

            // Splicing succeeds but puts 5 under 2's LEFT subtree, breaking order.
            { [[2, 1], [1, null, 5]], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMergeByLinearScan_LeetCodeExamples_ReturnsMergedBstOrNull(int?[][] trees, int?[] expected) =>
        Assert.Equal(
            expected,
            LevelOrder(MergeBSTsToCreateSingleBSTSolution.CanMergeByLinearScan(BuildForest(trees))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMergeByHashMapIndex_LeetCodeExamples_ReturnsMergedBstOrNull(int?[][] trees, int?[] expected) =>
        Assert.Equal(
            expected,
            LevelOrder(MergeBSTsToCreateSingleBSTSolution.CanMergeByHashMapIndex(BuildForest(trees))));

    private static List<BinaryTreeNode<int>> BuildForest(int?[][] trees)
    {
        var forest = new List<BinaryTreeNode<int>>(trees.Length);

        foreach (var levelOrder in trees)
        {
            forest.Add(BuildTree(levelOrder));
        }

        return forest;
    }

    // LeetCode's own level-order input shape: a BFS-ordered array with null standing
    // in for a missing child.
    private static BinaryTreeNode<int> BuildTree(int?[] levelOrder)
    {
        var rootValue = levelOrder[0]
            ?? throw new InvalidOperationException(
                "every tree in the examples above starts with its root's value; a null first slot would mean an empty tree.");

        var root = new BinaryTreeNode<int>(rootValue);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var cursor = 1;

        while (cursor < levelOrder.Length)
        {
            var current = queue.Dequeue();
            cursor = AttachChildren(current, levelOrder, queue, cursor);
        }

        return root;
    }

    // Attaches the next level-order entries as the left and then the right child of
    // the node at the front of the queue, and returns the cursor past them.
    private static int AttachChildren(
        BinaryTreeNode<int> current, int?[] levelOrder, Queue<BinaryTreeNode<int>> queue, int cursor)
    {
        if (levelOrder[cursor] is { } leftValue)
        {
            current.Left = new BinaryTreeNode<int>(leftValue);
            queue.Enqueue(current.Left);
        }

        cursor++;

        if (cursor < levelOrder.Length && levelOrder[cursor] is { } rightValue)
        {
            current.Right = new BinaryTreeNode<int>(rightValue);
            queue.Enqueue(current.Right);
        }

        return cursor + 1;
    }

    // The same shape back out, so an expected tree reads exactly as LeetCode prints
    // it: trailing nulls trimmed, and an empty array for "no valid merge".
    private static int?[] LevelOrder(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return [];
        }

        var values = CollectLevelOrderValues(root);

        TrimTrailingNulls(values);

        return [.. values];
    }

    private static List<int?> CollectLevelOrderValues(BinaryTreeNode<int> root)
    {
        var values = new List<int?>();
        var queue = new Queue<BinaryTreeNode<int>?>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            values.Add(node?.Value);

            if (node is not null)
            {
                queue.Enqueue(node.Left);
                queue.Enqueue(node.Right);
            }
        }

        return values;
    }

    // LeetCode trims the trailing nulls of a level-order print, so an expected
    // array ends at the last real value.
    private static void TrimTrailingNulls(List<int?> values)
    {
        while (values.Count > 0 && values[^1] is null)
        {
            values.RemoveAt(values.Count - 1);
        }
    }
}
