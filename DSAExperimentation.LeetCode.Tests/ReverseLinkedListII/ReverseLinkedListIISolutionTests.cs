using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReverseLinkedListII;

namespace DSAExperimentation.LeetCode.Tests.ReverseLinkedListII;

// Harness only. Both strategies are ReverseLinkedListIISolution's - this file
// builds LeetCode's published examples as linked lists and checks the resulting
// list's values.
public sealed partial class ReverseLinkedListIISolutionTests
{
    public static TheoryData<int[], int, int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, 4, [1, 4, 3, 2, 5] },
            { [5], 1, 1, [5] },
            { [1, 2, 3, 4, 5], 1, 5, [5, 4, 3, 2, 1] },
            { [1, 2, 3, 4, 5], 1, 3, [3, 2, 1, 4, 5] },
            { [1, 2, 3, 4, 5], 3, 5, [1, 2, 5, 4, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseBetweenByArrayRebuild_LeetCodeExamples_ReversesClosedRange(
        int[] values, int left, int right, int[] expected)
    {
        var reversed = ReverseLinkedListIISolution.ReverseBetweenByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(values), left, right);
        var actual = LeetCodeWireFormat.FromLinkedList(reversed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseBetweenByHeadInsertion_LeetCodeExamples_ReversesClosedRange(
        int[] values, int left, int right, int[] expected)
    {
        var reversed = ReverseLinkedListIISolution.ReverseBetweenByHeadInsertion(
            LeetCodeWireFormat.ToLinkedList(values), left, right);
        var actual = LeetCodeWireFormat.FromLinkedList(reversed);

        Assert.Equal(expected, actual);
    }
}
