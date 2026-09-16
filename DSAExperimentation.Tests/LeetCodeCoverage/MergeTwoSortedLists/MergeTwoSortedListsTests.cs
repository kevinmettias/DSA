using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MergeTwoSortedLists;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeTwoSortedLists;

// Harness only. The one strategy is MergeTwoSortedListsSolution's - this file pins it
// to LeetCode's published examples, stated once as value arrays.
public sealed partial class MergeTwoSortedListsTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [1, 2, 4], [1, 3, 4], [1, 1, 2, 3, 4, 4] },
            { [], [5, 6], [5, 6] },
            { [], [], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByDummyHeadSplice_LeetCodeExamples_ReturnsOneInterleavedSortedList(
        int[] first, int[] second, int[] expected)
    {
        var firstList = BuildList(first);
        var secondList = BuildList(second);
        var merged = MergeTwoSortedListsSolution.MergeByDummyHeadSplice(firstList, secondList);
        var actual = ToArray(merged);

        Assert.Equal(expected, actual);
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);
            head ??= node;
            AppendAfter(tail, node);
            tail = node;
        }

        return head;
    }

    // No previous node to link on the very first iteration (tail is still null) -
    // head itself becomes that first node instead, back in BuildList.
    private static void AppendAfter(SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int> node)
    {
        if (tail is not null)
        {
            tail.Next = node;
        }
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
