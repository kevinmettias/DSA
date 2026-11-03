using DSAExperimentation.LeetCode.Solutions.Conventions;
using DSAExperimentation.LeetCode.Solutions.DeleteNodesFromLinkedListPresentInArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DeleteNodesFromLinkedListPresentInArray;

// Harness only. Both strategies are
// DeleteNodesFromLinkedListPresentInArraySolution's - this file pins them to
// LeetCode's published examples.
public sealed partial class DeleteNodesFromLinkedListPresentInArraySolutionTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3], [1, 2, 3, 4, 5], [4, 5] },
            { [1], [1, 2, 1, 2, 1, 2], [2, 2, 2] },
            { [5], [1, 2, 3, 4], [1, 2, 3, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifiedListByArrayScan_LeetCodeExamples_RemovesNodesWhoseValueIsInNums(
        int[] nums, int[] headValues, int[] expected)
    {
        var list = DeleteNodesFromLinkedListPresentInArraySolution.ModifiedListByArrayScan(
            nums, LeetCodeWireFormat.ToLinkedList(headValues));
        var actual = LeetCodeWireFormat.FromLinkedList(list);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifiedListBySetFilter_LeetCodeExamples_RemovesNodesWhoseValueIsInNums(
        int[] nums, int[] headValues, int[] expected)
    {
        var list = DeleteNodesFromLinkedListPresentInArraySolution.ModifiedListBySetFilter(
            nums, LeetCodeWireFormat.ToLinkedList(headValues));
        var actual = LeetCodeWireFormat.FromLinkedList(list);
        Assert.Equal(expected, actual);
    }
}
