using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Each child is recorded under the digit it was added for, as GetOrAddChildFor returns it, so a
// child read back is checked against that record by reference rather than through the store's own
// lookup, and the order the store ranks its children in is checked against the digits, 0 before 1.
public sealed partial class BinarySlotsTests
{
    private const int Zero = 0;
    private const int One = 1;

    public static TheoryData<int[], int[]> DigitsAndRankOrder => new()
    {
        { [], [] },
        { [One], [One] },
        { [Zero], [Zero] },
        { [One, Zero], [Zero, One] },
        { [Zero, One, Zero], [Zero, One] },
    };

    [Fact]
    public void Empty_HasNoChildren()
    {
        var slots = BinarySlots<BitTrieNode>.Empty();

        Assert.Equal((0, null, null), (slots.Count, slots.ChildFor(Zero), slots.ChildFor(One)));
    }

    [Theory]
    [MemberData(nameof(DigitsAndRankOrder))]
    public void Count_CountsDistinctDigits(int[] digits, int[] rankOrder) =>
        Assert.Equal(rankOrder.Length, NodeWithChildren(digits).Node.Children.Count);

    [Theory]
    [MemberData(nameof(DigitsAndRankOrder))]
    public void ChildOfRank_ListsChildrenZeroBeforeOne(int[] digits, int[] rankOrder)
    {
        var (node, added) = NodeWithChildren(digits);

        Assert.All(Enumerable.Range(0, rankOrder.Length), rank => Assert.Same(added[rankOrder[rank]], node.Children.ChildOfRank(rank)));
    }

    [Fact]
    public void ChildOfRank_AtTheCount_Throws()
    {
        var node = NodeWithChildren([One]).Node;

        Assert.Throws<ArgumentOutOfRangeException>(() => node.Children.ChildOfRank(1));
    }

    [Fact]
    public void ChildFor_DigitWithAChild_FindsThatDigitsChild()
    {
        var (node, added) = NodeWithChildren([One, Zero]);

        Assert.Same(added[Zero], node.Children.ChildFor(Zero));
        Assert.Same(added[One], node.Children.ChildFor(One));
    }

    [Fact]
    public void ChildFor_DigitWithNoChild_ReturnsNull() => Assert.Null(NodeWithChildren([One]).Node.Children.ChildFor(Zero));

    [Fact]
    public void GetOrAddChildFor_DigitWithAChild_ReturnsTheSameChild()
    {
        var node = new BitTrieNode();
        var first = node.Children.GetOrAddChildFor(One);

        Assert.Same(first, node.Children.GetOrAddChildFor(One));
    }

    [Fact]
    public void ChildFor_NotADigit_Throws() =>
        Assert.Throws<IndexOutOfRangeException>(() => new BitTrieNode().Children.ChildFor(2));

    private static NodeAndChildren NodeWithChildren(int[] digits)
    {
        var node = new BitTrieNode();
        var added = new Dictionary<int, BitTrieNode>();

        foreach (var digit in digits)
        {
            added[digit] = node.Children.GetOrAddChildFor(digit);
        }

        return new NodeAndChildren(node, added);
    }

    // A node and each child it was given, under the digit it was added for.
    private readonly record struct NodeAndChildren(BitTrieNode Node, Dictionary<int, BitTrieNode> Added);
}
