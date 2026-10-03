using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReorderList;

namespace DSAExperimentation.LeetCode.Tests.ReorderList;

// Harness only. The single strategy is ReorderListSolution's - this file
// builds LeetCode's published examples as linked lists, reorders in place,
// and checks the resulting list's values.
public sealed partial class ReorderListSolutionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4], [1, 4, 2, 3] },
            { [1, 2, 3, 4, 5], [1, 5, 2, 4, 3] },
            { [1], [1] },
            { [1, 2], [1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReorderByReverseAndMergeInPlace_LeetCodeExamples_ReordersInPlace(int[] values, int[] expected)
    {
        var head = LeetCodeWireFormat.ToLinkedList(values);

        ReorderListSolution.ReorderByReverseAndMergeInPlace(head);

        Assert.Equal(expected, LeetCodeWireFormat.FromLinkedList(head));
    }
}
