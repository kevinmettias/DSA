using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MergeBSTsToCreateSingleBST;

namespace DSAExperimentation.LeetCode.Tests.MergeBSTsToCreateSingleBST;

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
public sealed partial class MergeBSTsToCreateSingleBSTSolutionTests
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
            LeetCodeWireFormat.FromBinaryTree(
                MergeBSTsToCreateSingleBSTSolution.CanMergeByLinearScan(BuildForest(trees))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMergeByHashMapIndex_LeetCodeExamples_ReturnsMergedBstOrNull(int?[][] trees, int?[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromBinaryTree(
                MergeBSTsToCreateSingleBSTSolution.CanMergeByHashMapIndex(BuildForest(trees))));

    // Every tree in the examples above starts with its root's value, so none of them
    // reads back as an empty tree.
    private static List<BinaryTreeNode<int>> BuildForest(int?[][] trees) =>
        [.. trees.Select(levelOrder => LeetCodeWireFormat.ToBinaryTree(levelOrder)!)];
}
