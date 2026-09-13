using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.RangeSumOfBST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumOfBST;

// Harness only. Both strategies are RangeSumOfBSTSolution's; this file states
// LeetCode's published examples once as insertion orders, builds each tree through
// this repo's own BinarySearchTree<int>.Insert (which reproduces the published
// shapes exactly), and asserts every strategy against the same expectations - the
// full scan included, which the benchmark's baseline arm previously left unchecked.
public sealed class RangeSumOfBSTTests
{
    // values (insertion order), low, high, expected sum
    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            // [10,5,15,3,7,null,18], low=7, high=15 -> 7 + 10 + 15 = 32
            { [10, 5, 15, 3, 7, 18], 7, 15, 32 },

            // [10,5,15,3,7,13,18,1,null,6], low=6, high=10 -> 6 + 7 + 10 = 23
            { [10, 5, 15, 3, 7, 13, 18, 1, 6], 6, 10, 23 },

            // A range entirely above the tree contributes nothing.
            { [10, 5, 15], 100, 200, 0 },

            // A range spanning the whole tree sums every node.
            { [10, 5, 15, 3, 7, 18], 1, 100, 58 },

            // A single-node tree whose value is both bounds.
            { [7], 7, 7, 7 },

            // Bounds are inclusive at both ends.
            { [10, 5, 15, 3, 7, 18], 5, 7, 12 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RangeSumByFullScan_LeetCodeExamples_SumsValuesInsideRange(
        int[] values, int low, int high, int expected) =>
        Assert.Equal(expected, RangeSumOfBSTSolution.RangeSumByFullScan(BuildTree(values), low, high));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RangeSumBySearchTreePruning_LeetCodeExamples_SumsValuesInsideRange(
        int[] values, int low, int high, int expected) =>
        Assert.Equal(expected, RangeSumOfBSTSolution.RangeSumBySearchTreePruning(BuildTree(values), low, high));

    private static BinaryTreeNode<int>? BuildTree(int[] values)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }
}
