using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.IntersectionOfTwoLinkedLists;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IntersectionOfTwoLinkedLists;

// Harness only. The one strategy is IntersectionOfTwoLinkedListsSolution's own
// two-pointer walk; this file just pins it to LeetCode's published examples -
// two intersecting shapes plus the no-intersection case, each identified by the
// values unique to A, the values unique to B, and the shared tail (empty means
// no intersection).
public sealed partial class IntersectionOfTwoLinkedListsTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [4, 1], [5, 6, 1], [8, 4, 5] },
            { [1, 9, 1, 2], [3], [4, 8] },
            { [2, 6, 4], [1, 5], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetIntersectionNodeByTwoPointerWalk_LeetCodeExamples_ReturnsSharedNodeOrNull(
        int[] onlyInA, int[] onlyInB, int[] sharedTail)
    {
        var shared = BuildChain(sharedTail, tail: null);
        var headA = BuildChain(onlyInA, shared);
        var headB = BuildChain(onlyInB, shared);

        var actual = IntersectionOfTwoLinkedListsSolution.GetIntersectionNodeByTwoPointerWalk(headA, headB);

        Assert.Same(shared, actual);
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
}
