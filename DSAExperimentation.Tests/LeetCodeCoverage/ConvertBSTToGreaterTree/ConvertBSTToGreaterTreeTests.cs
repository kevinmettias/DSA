using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConvertBSTToGreaterTree;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertBSTToGreaterTree;

// Harness only: both strategies live in ConvertBSTToGreaterTreeSolution and are
// asserted against LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape. BinaryTreeNode<int> is internal, so - as in
// BinaryTreeLevelOrderTraversalTests - it stays out of a public TheoryData signature
// and LeetCodeWireFormat.ToBinaryTree reconstructs both the input tree and the expected one from that
// shape (CS0053 is why this file used one [Fact] per example before).
public sealed class ConvertBSTToGreaterTreeTests
{
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            // [0, null, 1] -> [1, null, 1]
            { new TreeExample([0, null, 1], [1, null, 1]) },

            //       5                  29
            //      / \                /  \
            //     3   8      ->     36    17
            //    / \ / \            / \   / \
            //   2  4 7  9         38 33  24  9
            { new TreeExample([5, 3, 8, 2, 4, 7, 9], [29, 36, 17, 38, 33, 24, 9]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConvertByReverseInOrder_LeetCodeExamples_AccumulatesSumOfGreaterValues(TreeExample example)
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByReverseInOrder(LeetCodeWireFormat.ToBinaryTree(example.Values));

        AssertTreeEqual(LeetCodeWireFormat.ToBinaryTree(example.Expected), actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConvertByInOrderHooks_LeetCodeExamples_AccumulatesSumOfGreaterValues(TreeExample example)
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByInOrderHooks(LeetCodeWireFormat.ToBinaryTree(example.Values));

        AssertTreeEqual(LeetCodeWireFormat.ToBinaryTree(example.Expected), actual);
    }

    private static void AssertTreeEqual(BinaryTreeNode<int>? expected, BinaryTreeNode<int>? actual)
    {
        if (expected is null || actual is null)
        {
            // The two trees must end together, and a tree that ends where the other
            // continues is the shape mismatch this recursion reports. One guard covers
            // both sides, so nothing here has to promise a node is present.
            Assert.Equal(expected is null, actual is null);
            return;
        }

        Assert.Equal(expected.Value, actual.Value);
        AssertTreeEqual(expected.Left, actual.Left);
        AssertTreeEqual(expected.Right, actual.Right);
    }

    public readonly record struct TreeExample(int?[] Values, int?[] Expected);
}
