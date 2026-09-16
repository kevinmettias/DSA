using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LinkedListInBinaryTreeWorkloads (ARCHITECTURE 17.7). The reading depends on
// the needle matching LC 1367's skewed chain for a real depth and then breaking, so the one genuine
// candidate forces genuine matching work instead of failing at the first comparison.
public sealed partial class LinkedListInBinaryTreeWorkloadsTests
{
    private const int NodeCount = 64;
    private const int MatchDepth = NodeCount / 2;
    private const int MismatchValue = -1;

    [Fact]
    public void BuildNeedleValues_NodeCount_ReturnsTheMatchingPrefixThenTheMismatch()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues(NodeCount);

        Assert.Equal(MatchDepth + 1, values.Length);
        Assert.Equal(Enumerable.Range(0, MatchDepth), values.SkipLast(1));
        Assert.Equal(MismatchValue, values[^1]);
    }

    // The needle is written for BinaryTrees.Skewed(nodeCount): the prefix repeats that chain's own
    // values, and the value after the prefix is deliberately not the chain's next one, which is what
    // makes the match go deep and then fail.
    [Fact]
    public void BuildNeedleValues_NodeCount_FollowsTheSkewedChainThenBreaksFromIt()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues(NodeCount);
        var chainValues = SkewedValues(NodeCount);

        Assert.Equal(chainValues.Take(MatchDepth), values.SkipLast(1));
        Assert.NotEqual(chainValues[MatchDepth], values[^1]);
    }

    // The scenario word after `BuildNeedle` is deliberately not `Values`: a test name is addressed to the
    // longest method name it reads as, and `BuildNeedle_Values_` reads as the sibling `BuildNeedleValues`,
    // which would leave `BuildNeedle` itself addressed by no test at all.
    [Fact]
    public void BuildNeedle_ChainOfValues_CarriesThemInOrder()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues(NodeCount);
        var needle = LinkedListInBinaryTreeWorkloads.BuildNeedle(values);

        Assert.Equal(values, Values(needle));
    }

    [Fact]
    public void BuildNeedle_ChainOfValues_EndsAfterTheLastValue()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues(NodeCount);
        var needle = LinkedListInBinaryTreeWorkloads.BuildNeedle(values);

        Assert.Null(Nodes(needle)[^1].Next);
    }

    private static List<int> SkewedValues(int nodeCount)
    {
        var values = new List<int>();

        for (BinaryTreeNode<int>? node = BinaryTrees.Skewed(nodeCount); node is not null; node = node.Right)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static List<int> Values(SinglyLinkedListNode<int> head) =>
        [.. Nodes(head).Select(node => node.Value)];

    private static List<SinglyLinkedListNode<int>> Nodes(SinglyLinkedListNode<int> head)
    {
        var nodes = new List<SinglyLinkedListNode<int>>();

        for (SinglyLinkedListNode<int>? node = head; node is not null; node = node.Next)
        {
            nodes.Add(node);
        }

        return nodes;
    }
}
