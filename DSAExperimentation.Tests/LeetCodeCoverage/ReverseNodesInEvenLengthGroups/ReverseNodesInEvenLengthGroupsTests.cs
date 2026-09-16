using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.ReverseNodesInEvenLengthGroups;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseNodesInEvenLengthGroups;

// Harness only. Both strategies are ReverseNodesInEvenLengthGroupsSolution's -
// this file pins them to LeetCode's published examples plus the boundary cases the
// growing group size creates: a single node (no group after the head at all), a
// truncated odd final group, and a truncated final group that comes out even and
// therefore does reverse.
public sealed partial class ReverseNodesInEvenLengthGroupsTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [5, 2, 6, 3, 9, 1, 7, 3, 8, 4], [5, 6, 2, 3, 9, 1, 4, 8, 3, 7] },
            { [1, 1, 0, 6], [1, 0, 1, 6] },
            { [1, 1, 0, 6, 5], [1, 0, 1, 5, 6] },
            { [8], [8] },
            { [1, 2], [1, 2] },
            { [1, 2, 3], [1, 3, 2] },
            { [1, 2, 3, 4, 5, 6, 7, 8], [1, 3, 2, 4, 5, 6, 8, 7] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseEvenLengthGroupsByPointerReversal_LeetCodeExamples_ReversesOnlyEvenActualLengthGroups(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                ReverseNodesInEvenLengthGroupsSolution.ReverseEvenLengthGroupsByPointerReversal(
                    LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseEvenLengthGroupsByArrayReverse_LeetCodeExamples_ReversesOnlyEvenActualLengthGroups(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                ReverseNodesInEvenLengthGroupsSolution.ReverseEvenLengthGroupsByArrayReverse(
                    LeetCodeWireFormat.ToLinkedList(values))));
}
