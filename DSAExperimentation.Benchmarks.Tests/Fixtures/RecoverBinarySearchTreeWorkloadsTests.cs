using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RecoverBinarySearchTreeWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 99's tree being a balanced BST over the whole value range with exactly its two extremal values swapped,
// so both recovery strategies have the one non-adjacent violation pair to find and fix.
public sealed partial class RecoverBinarySearchTreeWorkloadsTests
{
    private const int Size = 100;
    private const int FirstValue = 0;
    private const int LastValue = Size - 1;

    [Fact]
    public void BuildCorruptedBst_Size_ReturnsOneNodePerValue() =>
        Assert.Equal(Size, InOrderValues(RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(Size)).Count);

    // The decisive reading: the tree is built balanced over 0..size-1 and then only its two extremal
    // values are swapped, so an in-order walk is the sorted range with its first and last readings
    // exchanged - which is exactly one non-adjacent violation pair.
    [Fact]
    public void BuildCorruptedBst_InOrderReading_IsTheSortedRangeWithOnlyItsTwoExtremesSwapped() =>
        Assert.Equal(
            AnswerText.Of(ExpectedInOrderValues()),
            AnswerText.Of(InOrderValues(RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(Size))));

    // The swap moves values, it does not add or drop one: the walk still carries the whole range exactly
    // once, which is what makes the violation pair the only thing wrong with the tree.
    [Fact]
    public void BuildCorruptedBst_Tree_CarriesEveryValueExactlyOnce() =>
        Assert.Equal(
            Enumerable.Range(FirstValue, Size),
            InOrderValues(RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(Size)).OrderBy(value => value));

    // A corruption that left both extremes where they were would give both strategies nothing to fix, so
    // the two extremal NODES are checked to hold each other's value rather than their own.
    [Fact]
    public void BuildCorruptedBst_ExtremalNodes_HoldEachOthersSwappedValue()
    {
        var root = RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(Size);

        Assert.Equal(LastValue, LeftmostValue(root));
        Assert.Equal(FirstValue, RightmostValue(root));
    }

    private static int[] ExpectedInOrderValues()
    {
        var expected = new int[Size];
        expected[0] = LastValue;
        expected[Size - 1] = FirstValue;

        for (var value = FirstValue + 1; value < LastValue; value++)
        {
            expected[value] = value;
        }

        return expected;
    }

    private static List<int> InOrderValues(BinaryTreeNode<int> node)
    {
        var values = new List<int>();
        AppendInOrder(node, values);

        return values;
    }

    private static void AppendInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        AppendInOrder(node.Left, values);
        values.Add(node.Value);
        AppendInOrder(node.Right, values);
    }

    private static int LeftmostValue(BinaryTreeNode<int> node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }

        return node.Value;
    }

    private static int RightmostValue(BinaryTreeNode<int> node)
    {
        while (node.Right is not null)
        {
            node = node.Right;
        }

        return node.Value;
    }
}
