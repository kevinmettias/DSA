using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.AllElementsInTwoBinarySearchTrees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllElementsInTwoBinarySearchTrees;

// Harness only. Both strategies are AllElementsInTwoBinarySearchTreesSolution's -
// this file states LeetCode's examples once and asserts each strategy against them.
// Each tree is given as its BST insertion order rather than as a node graph:
// BinaryTreeNode<TValue> is internal, so a public MemberData member cannot name it,
// and the insertion order pins the same shape LeetCode draws.
public sealed class AllElementsInTwoBinarySearchTreesTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            // root1 = [2,1,4], root2 = [1,0,3] -> [0,1,1,2,3,4]
            { [2, 1, 4], [1, 0, 3], [0, 1, 1, 2, 3, 4] },

            // root1 = [1,null,8], root2 = [8,1] -> [1,1,8,8]
            { [1, 8], [8, 1], [1, 1, 8, 8] },

            // Either tree may be empty.
            { [], [5, 2, 9], [2, 5, 9] },
            { [5, 2, 9], [], [2, 5, 9] },
            { [], [], [] },

            // A right-skewed chain against a left-skewed one, so the merge has to
            // alternate sources rather than drain one and then the other.
            { [1, 3, 5], [6, 4, 2], [1, 2, 3, 4, 5, 6] },

            // Disjoint ranges: one sequence is fully consumed before the other starts.
            { [1, 0, 2], [11, 10, 12], [0, 1, 2, 10, 11, 12] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetAllElementsByInOrderMerge_LeetCodeExamples_ReturnsMergedAscendingValues(
        int[] tree1,
        int[] tree2,
        int[] expected)
    {
        var actual = AllElementsInTwoBinarySearchTreesSolution.GetAllElementsByInOrderMerge(
            BuildTree(tree1),
            BuildTree(tree2));

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetAllElementsByCollectThenSort_LeetCodeExamples_ReturnsMergedAscendingValues(
        int[] tree1,
        int[] tree2,
        int[] expected)
    {
        var actual = AllElementsInTwoBinarySearchTreesSolution.GetAllElementsByCollectThenSort(
            BuildTree(tree1),
            BuildTree(tree2));

        Assert.Equal(expected, actual);
    }

    private static BinaryTreeNode<int>? BuildTree(int[] insertionOrder)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in insertionOrder)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }
}
