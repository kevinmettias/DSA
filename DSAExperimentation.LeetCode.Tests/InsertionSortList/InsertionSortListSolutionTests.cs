using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.InsertionSortList;

namespace DSAExperimentation.LeetCode.Tests.InsertionSortList;

// Harness only. The one strategy is InsertionSortListSolution's - this file builds
// LeetCode's published examples as linked lists and checks the resulting list's
// values.
public sealed partial class InsertionSortListSolutionTests
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
    public void SortByDummyHeadInsertion_LeetCodeExamples_SortsListAscending(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                InsertionSortListSolution.SortByDummyHeadInsertion(LeetCodeWireFormat.ToLinkedList(values))));
}
