using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.IntersectionOfTwoLinkedLists;

namespace DSAExperimentation.LeetCode.Tests.IntersectionOfTwoLinkedLists;

// Harness only. The one strategy is IntersectionOfTwoLinkedListsSolution's own
// two-pointer walk; this file just pins it to LeetCode's published examples. A row
// is the custom judge's own five inputs - intersectVal, listA, listB, skipA, skipB -
// and the structure is built the way the judge builds it: listA's nodes from skipA
// on are the shared tail, listB's first skipB values lead into that same tail, and
// the answer is the tail's first node (null when intersectVal is 0).
public sealed partial class IntersectionOfTwoLinkedListsSolutionTests
{
    // LeetCode's intersectVal for two lists that never meet.
    private const int NoIntersection = 0;

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
        Assert.Equal(input.IntersectVal, shared?.Value ?? NoIntersection);
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
