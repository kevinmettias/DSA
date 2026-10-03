using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RotateList;

namespace DSAExperimentation.LeetCode.Tests.RotateList;

// Harness only. Both strategies are RotateListSolution's - this file builds
// LeetCode's published examples as linked lists and checks the resulting list's
// values.
public sealed partial class RotateListSolutionTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, [4, 5, 1, 2, 3] },
            { [0, 1, 2], 4, [2, 0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateRightByArrayRebuild_LeetCodeExamples_RotatesList(
        int[] values, int rotationCount, int[] expected)
    {
        var rotated = RotateListSolution.RotateRightByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(values), rotationCount);
        var actual = LeetCodeWireFormat.FromLinkedList(rotated);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateRightByPointerRewire_LeetCodeExamples_RotatesList(
        int[] values, int rotationCount, int[] expected)
    {
        var rotated = RotateListSolution.RotateRightByPointerRewire(
            LeetCodeWireFormat.ToLinkedList(values), rotationCount);
        var actual = LeetCodeWireFormat.FromLinkedList(rotated);

        Assert.Equal(expected, actual);
    }
}
