using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReverseNodesInKGroup;

namespace DSAExperimentation.LeetCode.Tests.ReverseNodesInKGroup;

// Harness only. Both strategies are ReverseNodesInKGroupSolution's - this file
// pins them to LeetCode's published examples, including groupSize = 1 (a no-op) and a
// single-node list (always a short final group).
public sealed partial class ReverseNodesInKGroupSolutionTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, [2, 1, 4, 3, 5] },
            { [1, 2, 3, 4, 5], 3, [3, 2, 1, 4, 5] },
            { [1, 2, 3, 4, 5], 1, [1, 2, 3, 4, 5] },
            { [1], 1, [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseKGroupByPointerReversal_LeetCodeExamples_ReversesOnlyCompleteGroups(
        int[] values, int groupSize, int[] expected)
    {
        var reversed = ReverseNodesInKGroupSolution.ReverseKGroupByPointerReversal(
            LeetCodeWireFormat.ToLinkedList(values), groupSize);
        var actual = LeetCodeWireFormat.FromLinkedList(reversed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseKGroupByArrayReverse_LeetCodeExamples_ReversesOnlyCompleteGroups(
        int[] values, int groupSize, int[] expected)
    {
        var reversed = ReverseNodesInKGroupSolution.ReverseKGroupByArrayReverse(
            LeetCodeWireFormat.ToLinkedList(values), groupSize);
        var actual = LeetCodeWireFormat.FromLinkedList(reversed);

        Assert.Equal(expected, actual);
    }
}
