using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReverseLinkedList;

namespace DSAExperimentation.LeetCode.Tests.ReverseLinkedList;

// Harness only. The single strategy is ReverseLinkedListSolution's - this file
// builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed partial class ReverseLinkedListSolutionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], [5, 4, 3, 2, 1] },
            { [1, 2], [2, 1] },
            { [], [] },
            { [1], [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseListByIterativeRewire_LeetCodeExamples_ReversesPointers(int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                ReverseLinkedListSolution.ReverseListByIterativeRewire(LeetCodeWireFormat.ToLinkedList(values))));
}
