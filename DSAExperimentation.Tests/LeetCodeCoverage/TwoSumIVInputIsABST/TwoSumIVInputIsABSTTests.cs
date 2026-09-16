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
    public static TheoryData<TreePairExample> Examples =>
        new()
        {
            new TreePairExample([5, 3, 6, 2, 4, 7], 9, HasPair: true),
            new TreePairExample([5, 3, 6, 2, 4, 7], 28, HasPair: false),
            new TreePairExample([5, 3], 8, HasPair: true),
            new TreePairExample([5], 10, HasPair: false), // no second node - a value can't pair with itself
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTargetByNestedPairScan_LeetCodeExamples_ReturnsWhetherAPairSumsToTarget(
        TreePairExample example)
    {
        var tree = BuildTree(example.InsertOrder);

        var actual = TwoSumIVInputIsABSTSolution.FindTargetByNestedPairScan(tree.Root, example.Target);

        Assert.Equal(example.HasPair, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTargetByDepthFirstSetLookup_LeetCodeExamples_ReturnsWhetherAPairSumsToTarget(
        TreePairExample example)
    {
        var tree = BuildTree(example.InsertOrder);

        var actual = TwoSumIVInputIsABSTSolution.FindTargetByDepthFirstSetLookup(tree.Root, example.Target);

        Assert.Equal(example.HasPair, actual);
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

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct TreePairExample(int[] InsertOrder, int Target, bool HasPair);
}
