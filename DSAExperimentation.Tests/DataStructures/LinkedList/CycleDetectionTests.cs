using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.LinkedList;
using DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.LinkedList;

// Floyd's walk over every configuration of the one linked node: it reads only the forward link, so
// the singly linked, doubly linked and random-pointer nodes answer alike, and a doubly linked list
// whose back links happen to close a ring is still acyclic going forward.
public sealed partial class CycleDetectionTests
{
    // Every list of up to this many nodes, acyclic or with its tail rejoining at each position.
    private const int LongestChain = 9;

    // 1 -> 2 -> 3 -> 4 -> back to 2: the tail rejoins mid-list, not at the head.
    private const int RingChainLength = 4;
    private const int RingEntry = 1;

    private const int AcyclicChainLength = 5;

    public static TheoryData<string> Configurations =>
    [
        LinkedNodeConfigurations.Singly,
        LinkedNodeConfigurations.Doubly,
        LinkedNodeConfigurations.Random,
    ];

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasCycle_AcyclicList_ReturnsFalse(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);

        Assert.False(view.HasCycle(view.Chain(AcyclicChainLength, cycleEntry: null)[0]));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasCycle_NullHead_ReturnsFalse(string configuration) =>
        Assert.False(LinkedNodeConfigurations.Of(configuration).HasCycle(null));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasCycle_ListWithCycle_ReturnsTrue(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);

        Assert.True(view.HasCycle(view.Chain(RingChainLength, RingEntry)[0]));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasCycle_SingleNodeSelfCycle_ReturnsTrue(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);

        Assert.True(view.HasCycle(view.Chain(1, cycleEntry: 0)[0]));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void HasCycle_EveryLengthAndEntry_AgreesWithTheChainsShape(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);
        var (expected, actual) = (new List<bool>(), new List<bool>());

        foreach (var (count, entry) in Shapes())
        {
            expected.Add(entry is not null);
            actual.Add(view.HasCycle(view.Chain(count, entry)[0]));
        }

        Assert.Equal(expected, actual);
    }

    // The back links of a doubly linked chain rejoin its head, but its forward links end, so the
    // walk, which reads the forward link alone, finds no cycle.
    [Fact]
    public void HasCycle_DoublyLinkedChainWhoseBackLinksFormARing_ReturnsFalse()
    {
        var nodes = LinkedNodeConfigurations.Of(LinkedNodeConfigurations.Doubly)
            .Chain(AcyclicChainLength, cycleEntry: null)
            .Cast<DoublyLinkedListNode<int>>()
            .ToArray();
        LinkBackwardsInARing(nodes);

        Assert.False(CycleDetection.HasCycle(nodes[0]));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void FindCycleStart_AcyclicList_ReturnsNull(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);

        Assert.Null(view.FindCycleStart(view.Chain(AcyclicChainLength, cycleEntry: null)[0]));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void FindCycleStart_NullHead_ReturnsNull(string configuration) =>
        Assert.Null(LinkedNodeConfigurations.Of(configuration).FindCycleStart(null));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void FindCycleStart_ListWithCycle_ReturnsTheCycleEntryNode(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);
        var nodes = view.Chain(RingChainLength, RingEntry);

        Assert.Same(nodes[RingEntry], view.FindCycleStart(nodes[0]));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void FindCycleStart_SingleNodeSelfCycle_ReturnsThatNode(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);
        var nodes = view.Chain(1, cycleEntry: 0);

        Assert.Same(nodes[0], view.FindCycleStart(nodes[0]));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void FindCycleStart_EveryLengthAndEntry_ReturnsTheNodeTheTailRejoins(
        string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);
        var (expected, actual) = (new List<object?>(), new List<object?>());

        foreach (var (count, entry) in Shapes())
        {
            var nodes = view.Chain(count, entry);
            expected.Add(EntryNodeOf(nodes, entry));
            actual.Add(view.FindCycleStart(nodes[0]));
        }

        Assert.Equal(expected, actual, ReferenceEqualityComparer.Instance);
    }

    // Every chain length up to LongestChain, each acyclic and with its tail rejoining every node.
    private static IEnumerable<(int Count, int? Entry)> Shapes() =>
        Enumerable.Range(1, LongestChain)
            .SelectMany(count => EntriesOf(count).Select(entry => (count, entry)));

    // No entry, for the acyclic chain, then each node of a chain this long.
    private static IEnumerable<int?> EntriesOf(int count) =>
        Enumerable.Range(0, count).Select(at => (int?)at).Prepend(null);

    // The node a chain's tail rejoins, or null for an acyclic chain.
    private static object? EntryNodeOf(IReadOnlyList<object> nodes, int? entry)
    {
        if (entry is not { } at)
        {
            return null;
        }

        return nodes[at];
    }

    // Each node's back link points at the node before it, and the head's at the last node.
    private static void LinkBackwardsInARing(DoublyLinkedListNode<int>[] nodes)
    {
        for (var index = 0; index < nodes.Length; index++)
        {
            nodes[index].Previous = nodes[(index + nodes.Length - 1) % nodes.Length];
        }
    }
}
