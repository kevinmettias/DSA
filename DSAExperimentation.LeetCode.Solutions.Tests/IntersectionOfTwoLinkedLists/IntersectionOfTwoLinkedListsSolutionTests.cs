using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Solutions.IntersectionOfTwoLinkedLists;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IntersectionOfTwoLinkedLists;

// Harness only. The one strategy is IntersectionOfTwoLinkedListsSolution's own
// two-pointer walk; this file just pins it to LeetCode's published examples. A row
// is the custom judge's own five inputs - intersectVal, listA, listB, skipA, skipB -
// and the structure is built the way the judge builds it: listA's nodes from skipA
// on are the shared tail, listB's first skipB values lead into that same tail, and
// the answer is the tail's first node (null when intersectVal is 0). The catalog
// judges LeetCode's shape on the examples by value alone; the first theory asks for
// the node itself, and the second runs LeetCode's shape on judge inputs the catalog
// does not hold.
public sealed partial class IntersectionOfTwoLinkedListsSolutionTests
{
    public static TheoryData<JudgeInput> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { new JudgeInput(IntersectVal: 8, ListA: [4, 1, 8, 4, 5], ListB: [5, 6, 1, 8, 4, 5], SkipA: 2, SkipB: 3) },
            { new JudgeInput(IntersectVal: 2, ListA: [1, 9, 1, 2, 4], ListB: [3, 2, 4], SkipA: 3, SkipB: 1) },
            { new JudgeInput(IntersectVal: 0, ListA: [2, 6, 4], ListB: [1, 5], SkipA: 3, SkipB: 2) },

            // Example 2 with the shared tail extended by an 8 and moved one node later.
            { new JudgeInput(IntersectVal: 4, ListA: [1, 9, 1, 2, 4, 8], ListB: [3, 4, 8], SkipA: 4, SkipB: 1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetIntersectionNodeByTwoPointerWalk_LeetCodeExamples_ReturnsSharedNodeOrNull(JudgeInput input)
    {
        var shared = BuildChain(input.ListA[input.SkipA..], tail: null);
        var headA = BuildChain(input.ListA[..input.SkipA], shared);
        var headB = BuildChain(input.ListB[..input.SkipB], shared);

        var actual = IntersectionOfTwoLinkedListsSolution.GetIntersectionNodeByTwoPointerWalk(headA, headB);

        Assert.Same(shared, actual);
        Assert.Equal(input.IntersectVal, shared?.Value ?? IntersectionJudge.NoIntersection);
    }

    // Judge inputs beside LeetCode's examples, each printing intersectVal: the fourth
    // row above, listB being the shared tail from its head (skipB 0), listA being it
    // (skipA 0), and two lists ending in the same values on nodes of their own, which
    // never meet.
    public static TheoryData<JudgeInput> JudgeInputsBeyondTheExamples =>
        new()
        {
            { new JudgeInput(IntersectVal: 4, ListA: [1, 9, 1, 2, 4, 8], ListB: [3, 4, 8], SkipA: 4, SkipB: 1) },
            { new JudgeInput(IntersectVal: 2, ListA: [1, 9, 1, 2, 4], ListB: [2, 4], SkipA: 3, SkipB: 0) },
            { new JudgeInput(IntersectVal: 5, ListA: [5, 6], ListB: [7, 8, 9, 5, 6], SkipA: 0, SkipB: 3) },
            { new JudgeInput(IntersectVal: 0, ListA: [1, 2, 3], ListB: [4, 2, 3], SkipA: 3, SkipB: 3) },
        };

    [Theory]
    [MemberData(nameof(JudgeInputsBeyondTheExamples))]
    public void GetIntersectionNodeByTwoPointerWalk_LeetCodeJudgeInputs_PrintsTheSharedNodesValue(JudgeInput input)
    {
        var listA = LeetCodeWireFormat.ToLinkedList(input.ListA);
        var listB = LeetCodeWireFormat.ToLinkedList(input.ListB);

        var printed = IntersectionOfTwoLinkedListsSolution.GetIntersectionNodeByTwoPointerWalk(
            input.IntersectVal, listA, listB, (input.SkipA, input.SkipB));

        Assert.Equal(input.IntersectVal, printed);
    }

    private static SinglyLinkedListNode<int>? BuildChain(int[] values, SinglyLinkedListNode<int>? tail)
    {
        var head = tail;

        for (var i = values.Length - 1; i >= 0; i--)
        {
            head = new SinglyLinkedListNode<int>(values[i]) { Next = head };
        }

        return head;
    }

    // One custom-judge case, named field by field as LeetCode lists them: the two
    // skips are counts into the two different lists, and a bare `2, 3` would not say
    // which is which.
    public readonly record struct JudgeInput(int IntersectVal, int[] ListA, int[] ListB, int SkipA, int SkipB);
}
