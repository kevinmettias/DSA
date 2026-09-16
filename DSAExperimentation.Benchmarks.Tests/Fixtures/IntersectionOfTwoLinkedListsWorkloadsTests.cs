using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for IntersectionOfTwoLinkedListsWorkloads (ARCHITECTURE 17.7). The reading
// depends on LC 160's two lists actually intersecting after different-length prefixes, which is what
// the two-pointer walk exists to detect.
public sealed partial class IntersectionOfTwoLinkedListsWorkloadsTests
{
    private const int PrefixLength = 8;
    private const int HeadBPrefixMultiplier = 2; // the workload gives HeadB twice HeadA's prefix
    private const int SharedTailNodeCount = 1;
    private const int SharedTailValue = -1;

    [Fact]
    public void Build_HeadA_LeadsToTheSharedTailThroughOnePrefixNodePerStep()
    {
        var (headA, _) = IntersectionOfTwoLinkedListsWorkloads.Build(PrefixLength);

        Assert.Equal(PrefixLength + SharedTailNodeCount, Nodes(headA).Count);
    }

    [Fact]
    public void Build_HeadB_LeadsToTheSharedTailThroughTwiceAsManyPrefixNodes()
    {
        var (_, headB) = IntersectionOfTwoLinkedListsWorkloads.Build(PrefixLength);

        Assert.Equal((PrefixLength * HeadBPrefixMultiplier) + SharedTailNodeCount, Nodes(headB).Count);
    }

    // The intersection is the scenario: the two lists have to converge on one and the same node
    // object, which is what the two-pointer walk confirms by reference identity.
    [Fact]
    public void Build_BothHeads_ConvergeOnTheSameSharedTailNode()
    {
        var (headA, headB) = IntersectionOfTwoLinkedListsWorkloads.Build(PrefixLength);

        Assert.Same(Nodes(headA)[^1], Nodes(headB)[^1]);
        Assert.Equal(SharedTailValue, Nodes(headA)[^1].Value);
    }

    // The fixture's own note is that the two-pointer walk's cost depends on each list's length and
    // not on its node values, so the order the prefix positions were prepended in is not part of what
    // the walk reads: the prefix holds each of its own positions exactly once, in whatever order.
    [Fact]
    public void Build_EveryPrefixNode_HoldsOneOfItsOwnPrefixPositionsAsItsValue()
    {
        var (headA, _) = IntersectionOfTwoLinkedListsWorkloads.Build(PrefixLength);

        Assert.Equal(
            Enumerable.Range(0, PrefixLength),
            Nodes(headA).Take(PrefixLength).Select(node => node.Value).Order());
    }

    [Fact]
    public void Build_SamePrefixLength_ReturnsTheSameShape()
    {
        var (headA, headB) = IntersectionOfTwoLinkedListsWorkloads.Build(PrefixLength);
        var (repeatHeadA, repeatHeadB) = IntersectionOfTwoLinkedListsWorkloads.Build(PrefixLength);

        Assert.Equal(Values(headA), Values(repeatHeadA));
        Assert.Equal(Values(headB), Values(repeatHeadB));
    }

    private static List<SinglyLinkedListNode<int>> Nodes(SinglyLinkedListNode<int> head)
    {
        var nodes = new List<SinglyLinkedListNode<int>>();

        for (SinglyLinkedListNode<int>? node = head; node is not null; node = node.Next)
        {
            nodes.Add(node);
        }

        return nodes;
    }

    private static List<int> Values(SinglyLinkedListNode<int> head) =>
        [.. Nodes(head).Select(node => node.Value)];
}
