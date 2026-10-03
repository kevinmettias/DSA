using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LinkedListInBinaryTreeWorkloads (ARCHITECTURE 17.7). The reading depends on
// the needle matching LC 1367's skewed chain for a real depth and then breaking, so the one genuine
// candidate forces genuine matching work instead of failing at the first comparison - and on staying
// inside LC 1367's contract: a needle of at most 100 nodes, a tree of at most 2,500, and every value
// in [1, 100].
public sealed partial class LinkedListInBinaryTreeWorkloadsTests
{
    private const int NodeCount = 2_500; // LC 1367's tree cap, the benchmark's largest size
    private const int NeedleLength = 100; // LC 1367's list cap
    private const int MatchDepth = NeedleLength - 1;
    private const int MinValue = 1;
    private const int MaxValue = 100;

    [Fact]
    public void BuildNeedleValues_FullLengthNeedle_ReturnsTheChainsOpeningValuesThenABreak()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues();

        Assert.Equal(NeedleLength, values.Length);
        Assert.Equal(Enumerable.Range(MinValue, MatchDepth), values.SkipLast(1));
        Assert.All(values, value => Assert.InRange(value, MinValue, MaxValue));
    }

    // The needle is written for BuildChain: the prefix repeats that chain's own values, and the value
    // after the prefix is deliberately not the chain's next one, which is what makes the match go deep
    // and then fail.
    [Fact]
    public void BuildNeedleValues_FullLengthNeedle_FollowsTheChainThenBreaksFromIt()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues();
        var chainValues = ChainValues(NodeCount);

        Assert.Equal(chainValues.Take(MatchDepth), values.SkipLast(1));
        Assert.NotEqual(chainValues[MatchDepth], values[^1]);
    }

    // Only the root carries the needle's first value, so the walk has one real candidate.
    [Fact]
    public void BuildChain_NodeCount_ReturnsARightOnlyChainWithEveryValueInRange()
    {
        var chainValues = ChainValues(NodeCount);
        var needleStart = LinkedListInBinaryTreeWorkloads.BuildNeedleValues()[0];

        Assert.Equal(NodeCount, chainValues.Count);
        Assert.All(chainValues, value => Assert.InRange(value, MinValue, MaxValue));
        Assert.Single(chainValues, value => value == needleStart);
    }

    // The scenario word after `BuildNeedle` is deliberately not `Values`: a test name is addressed to the
    // longest method name it reads as, and `BuildNeedle_Values_` reads as the sibling `BuildNeedleValues`,
    // which would leave `BuildNeedle` itself addressed by no test at all.
    [Fact]
    public void BuildNeedle_ChainOfValues_CarriesThemInOrder()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues();
        var needle = LinkedListInBinaryTreeWorkloads.BuildNeedle(values);

        Assert.Equal(values, Values(needle));
    }

    [Fact]
    public void BuildNeedle_ChainOfValues_EndsAfterTheLastValue()
    {
        var values = LinkedListInBinaryTreeWorkloads.BuildNeedleValues();
        var needle = LinkedListInBinaryTreeWorkloads.BuildNeedle(values);

        Assert.Null(Nodes(needle)[^1].Next);
    }

    private static List<int> ChainValues(int nodeCount)
    {
        var values = new List<int>();

        for (BinaryTreeNode<int>? node = LinkedListInBinaryTreeWorkloads.BuildChain(nodeCount); node is not null; node = node.Right)
        {
            Assert.Null(node.Left);
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
