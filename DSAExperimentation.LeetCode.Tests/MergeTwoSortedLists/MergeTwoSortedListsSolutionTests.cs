using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MergeTwoSortedLists;

namespace DSAExperimentation.LeetCode.Tests.MergeTwoSortedLists;

// Harness only. The one strategy is MergeTwoSortedListsSolution's - this file pins it
// to LeetCode's published examples, stated once as value arrays.
public sealed partial class MergeTwoSortedListsSolutionTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [1, 2, 4], [1, 3, 4], [1, 1, 2, 3, 4, 4] },
            { [], [5, 6], [5, 6] },
            { [], [], [] },
            { [], [0], [0] },

            // One list entirely precedes the other, so the merge is a splice
            // with no interleaving at all.
            { [1, 2, 3], [7, 8, 9], [1, 2, 3, 7, 8, 9] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByDummyHeadSplice_LeetCodeExamples_ReturnsOneInterleavedSortedList(
        int[] first, int[] second, int[] expected)
    {
        var firstList = LeetCodeWireFormat.ToLinkedList(first);
        var secondList = LeetCodeWireFormat.ToLinkedList(second);
        var merged = MergeTwoSortedListsSolution.MergeByDummyHeadSplice(firstList, secondList);
        var actual = LeetCodeWireFormat.FromLinkedList(merged);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByRecursiveSelection_LeetCodeExamples_ReturnsOneInterleavedSortedList(
        int[] first, int[] second, int[] expected)
    {
        var firstList = LeetCodeWireFormat.ToLinkedList(first);
        var secondList = LeetCodeWireFormat.ToLinkedList(second);
        var merged = MergeTwoSortedListsSolution.MergeByRecursiveSelection(firstList, secondList);
        var actual = LeetCodeWireFormat.FromLinkedList(merged);

        Assert.Equal(expected, actual);
    }
}
