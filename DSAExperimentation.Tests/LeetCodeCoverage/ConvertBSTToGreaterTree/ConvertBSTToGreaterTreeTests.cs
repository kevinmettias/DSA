using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConvertBSTToGreaterTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertBSTToGreaterTree;

// Harness only: both strategies live in ConvertBSTToGreaterTreeSolution. One [Fact]
// per example per strategy rather than a [Theory] - BinaryTreeNode<int> is internal,
// so it cannot appear in a public TheoryData<...> member (CS0053), the same reason
// FlattenNestedListIteratorTests uses one [Fact] per case.
public sealed class ConvertBSTToGreaterTreeTests
{
    [Fact]
    public void ConvertByReverseInOrder_LeetCodeExample_AccumulatesSumOfGreaterValues()
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByReverseInOrder(SmallExampleInput());
        AssertTreeEqual(SmallExampleExpected(), actual);
    }

    [Fact]
    public void ConvertByReverseInOrder_LargerTree_EachNodeGetsSumOfItselfAndGreaterKeys()
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByReverseInOrder(LargerExampleInput());
        AssertTreeEqual(LargerExampleExpected(), actual);
    }

    [Fact]
    public void ConvertByInOrderHooks_LeetCodeExample_AccumulatesSumOfGreaterValues()
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByInOrderHooks(SmallExampleInput());
        AssertTreeEqual(SmallExampleExpected(), actual);
    }

    [Fact]
    public void ConvertByInOrderHooks_LargerTree_EachNodeGetsSumOfItselfAndGreaterKeys()
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByInOrderHooks(LargerExampleInput());
        AssertTreeEqual(LargerExampleExpected(), actual);
    }

    // [0, null, 1] -> [1, null, 1]
    private static BinaryTreeNode<int> SmallExampleInput() => new(0) { Right = new(1) };

    private static BinaryTreeNode<int> SmallExampleExpected() => new(1) { Right = new(1) };

    //       5                  29
    //      / \                /  \
    //     3   8      ->     36    17
    //    / \ / \            / \   / \
    //   2  4 7  9         38 33  24  9
    private static BinaryTreeNode<int> LargerExampleInput() => new(5)
    {
        Left = new(3) { Left = new(2), Right = new(4) },
        Right = new(8) { Left = new(7), Right = new(9) },
    };

    private static BinaryTreeNode<int> LargerExampleExpected() => new(29)
    {
        Left = new(36) { Left = new(38), Right = new(33) },
        Right = new(17) { Left = new(24), Right = new(9) },
    };

    private static void AssertTreeEqual(BinaryTreeNode<int>? expected, BinaryTreeNode<int>? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(expected.Value, actual!.Value);
        AssertTreeEqual(expected.Left, actual.Left);
        AssertTreeEqual(expected.Right, actual.Right);
    }
}
