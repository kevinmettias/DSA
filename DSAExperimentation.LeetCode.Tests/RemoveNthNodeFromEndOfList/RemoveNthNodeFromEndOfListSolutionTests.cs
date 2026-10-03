using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveNthNodeFromEndOfList;

namespace DSAExperimentation.LeetCode.Tests.RemoveNthNodeFromEndOfList;

// Harness only. Both strategies are RemoveNthNodeFromEndOfListSolution's - this
// file builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed partial class RemoveNthNodeFromEndOfListSolutionTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, [1, 2, 3, 5] },
            { [1], 1, [] },
            { [1, 2], 2, [2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByArrayRebuild_LeetCodeExamples_RemovesExpectedNode(
        int[] values, int positionsFromEnd, int[] expected)
    {
        var removed = RemoveNthNodeFromEndOfListSolution.RemoveByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(values), positionsFromEnd);
        var actual = LeetCodeWireFormat.FromLinkedList(removed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByTwoRunner_LeetCodeExamples_RemovesExpectedNode(
        int[] values, int positionsFromEnd, int[] expected)
    {
        var removed = RemoveNthNodeFromEndOfListSolution.RemoveByTwoRunner(
            LeetCodeWireFormat.ToLinkedList(values), positionsFromEnd);
        var actual = LeetCodeWireFormat.FromLinkedList(removed);

        Assert.Equal(expected, actual);
    }
}
