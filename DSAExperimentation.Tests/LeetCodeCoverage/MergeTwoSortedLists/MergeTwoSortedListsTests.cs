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

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByRecursiveSelection_LeetCodeExamples_ReturnsOneInterleavedSortedList(
        int[] first, int[] second, int[] expected)
    {
        var firstList = BuildList(first);
        var secondList = BuildList(second);
        var merged = MergeTwoSortedListsSolution.MergeByRecursiveSelection(firstList, secondList);
        var actual = ToArray(merged);

        Assert.Equal(expected, actual);
    }

    // The two arms are competing strategies for one question, so they must produce the same list
    // from the same inputs - including the empty-list cases, where the recursive arm's two base
    // cases have to hand back the other list unexamined.
    [Theory]
    [MemberData(nameof(Examples))]
    public void Merge_AgreeOnEveryExample(int[] first, int[] second, int[] expected)
    {
        var bySplice = ToArray(MergeTwoSortedListsSolution.MergeByDummyHeadSplice(
            BuildList(first), BuildList(second)));
        var byRecursion = ToArray(MergeTwoSortedListsSolution.MergeByRecursiveSelection(
            BuildList(first), BuildList(second)));

        Assert.Equal(bySplice, byRecursion);
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
