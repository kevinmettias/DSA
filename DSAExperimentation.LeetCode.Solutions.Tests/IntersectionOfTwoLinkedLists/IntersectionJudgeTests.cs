using DSAExperimentation.LeetCode.Solutions.IntersectionOfTwoLinkedLists;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IntersectionOfTwoLinkedLists;

// LC 160's judge as IntersectionJudge builds it: listB joined into listA's tail so that one node stands at
// listA's index skipA and listB's index skipB, or listB left its own when intersectVal is 0; and the node
// returned printed as its value, 0 for none.
public sealed partial class IntersectionJudgeTests
{
    private const int NodeValue = 8;

    // LeetCode's first example: intersectVal 8, listA [4,1,8,4,5], listB [5,6,1,8,4,5], skipA 2, skipB 3.
    // listB's fourth node is listA's third, and listB reads [5,6,1] and then listA's tail.
    [Fact]
    public void HeadBJoinedToA_ListsThatMeet_SharesTheNodeAtEachSkip()
    {
        var listA = LeetCodeWireFormat.ToLinkedList([4, 1, 8, 4, 5]);
        var listB = LeetCodeWireFormat.ToLinkedList([5, 6, 1, 8, 4, 5]);

        var headB = IntersectionJudge.HeadBJoinedToA(intersectVal: 8, listA, listB, (SkipA: 2, SkipB: 3));

        Assert.Same(listA?.Next?.Next, headB?.Next?.Next?.Next);
    }

    // skipB 0: listB is listA's tail from its very head, its own nodes all dropped.
    [Fact]
    public void HeadBJoinedToA_SkipBZero_IsListAsNodeAtSkipA()
    {
        var listA = LeetCodeWireFormat.ToLinkedList([1, 9, 1, 2, 4]);
        var listB = LeetCodeWireFormat.ToLinkedList([2, 4]);

        var headB = IntersectionJudge.HeadBJoinedToA(intersectVal: 2, listA, listB, (SkipA: 3, SkipB: 0));

        Assert.Same(listA?.Next?.Next?.Next, headB);
    }

    // LeetCode's third example: intersectVal 0, so listB stays the list it was, every node its own.
    [Fact]
    public void HeadBJoinedToA_NoIntersection_LeavesListBAsItWas()
    {
        var listA = LeetCodeWireFormat.ToLinkedList([2, 6, 4]);
        var listB = LeetCodeWireFormat.ToLinkedList([1, 5]);

        var headB = IntersectionJudge.HeadBJoinedToA(intersectVal: 0, listA, listB, (SkipA: 3, SkipB: 2));
        var valuesB = LeetCodeWireFormat.FromLinkedList(headB);

        Assert.Same(listB, headB);
        Assert.Equal([1, 5], valuesB);
    }

    [Fact]
    public void Printed_NoNode_IsZero() =>
        Assert.Equal(IntersectionJudge.NoIntersection, IntersectionJudge.Printed(null));

    [Fact]
    public void Printed_ANode_IsItsValue()
    {
        var node = LeetCodeWireFormat.ToLinkedList([NodeValue]);

        Assert.Equal(NodeValue, IntersectionJudge.Printed(node));
    }
}
