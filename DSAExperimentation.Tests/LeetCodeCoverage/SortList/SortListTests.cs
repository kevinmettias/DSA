using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SortList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortList;

// Harness only. The one strategy is SortListSolution's - this file builds
// LeetCode's published examples as linked lists and checks the resulting list's
// values.
public sealed partial class SortListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [4, 2, 1, 3], [1, 2, 3, 4] },
            { [-1, 5, 3, 4, 0], [-1, 0, 3, 4, 5] },
            { [], [] },
            { [1], [1] },
            { [2, 2, 1, 1], [1, 1, 2, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortByMergeSortOverSequence_LeetCodeExamples_SortsListAscending(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(SortListSolution.SortByMergeSortOverSequence(BuildList(values))));

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }
}
