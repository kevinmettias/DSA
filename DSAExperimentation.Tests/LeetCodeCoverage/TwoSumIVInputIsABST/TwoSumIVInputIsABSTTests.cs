using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.TwoSumIVInputIsABST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoSumIVInputIsABST;

// Harness only. Both strategies are TwoSumIVInputIsABSTSolution's - this file just
// builds the classic LeetCode example tree (and a couple of edge cases) with this
// repo's own BinarySearchTree<int>.Insert and pins each strategy to LeetCode's
// published answer.
public sealed class TwoSumIVInputIsABSTTests
{
    // Insert order [5,3,6,2,4,7] recreates the classic LC 653 example tree exactly:
    //         5
    //        / \
    //       3   6
    //      / \   \
    //     2   4   7
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [5, 3, 6, 2, 4, 7], 9, true },
            { [5, 3, 6, 2, 4, 7], 28, false },
            { [5, 3], 8, true },
            { [5], 10, false }, // no second node - a value can't pair with itself
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTargetByNestedPairScan_LeetCodeExamples_ReturnsWhetherAPairSumsToTarget(
        int[] insertOrder, int k, bool expected)
    {
        var tree = BuildTree(insertOrder);

        Assert.Equal(expected, TwoSumIVInputIsABSTSolution.FindTargetByNestedPairScan(tree.Root, k));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTargetByDepthFirstSetLookup_LeetCodeExamples_ReturnsWhetherAPairSumsToTarget(
        int[] insertOrder, int k, bool expected)
    {
        var tree = BuildTree(insertOrder);

        Assert.Equal(expected, TwoSumIVInputIsABSTSolution.FindTargetByDepthFirstSetLookup(tree.Root, k));
    }

    private static BinarySearchTree<int> BuildTree(int[] insertOrder)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in insertOrder)
        {
            tree.Insert(value);
        }

        return tree;
    }
}
