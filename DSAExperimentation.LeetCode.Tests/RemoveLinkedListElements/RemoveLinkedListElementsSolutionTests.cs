using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveLinkedListElements;

namespace DSAExperimentation.LeetCode.Tests.RemoveLinkedListElements;

// Harness only. Both strategies are RemoveLinkedListElementsSolution's - this file
// builds LeetCode's published examples as linked lists and checks the resulting
// list's values, including the case where val matches the head node itself.
public sealed partial class RemoveLinkedListElementsSolutionTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 6, 3, 4, 5, 6], 6, [1, 2, 3, 4, 5] }, // LC's example 1
            { [], 1, [] }, // LC's example 2
            { [7, 7, 7, 7], 7, [] }, // LC's example 3
            { [7, 1, 7, 2, 7], 7, [1, 2] }, // matches at the head too
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveElementsByArrayRebuild_LeetCodeExamples_RemovesMatchingValues(
        int[] values, int val, int[] expected)
    {
        var removed = RemoveLinkedListElementsSolution.RemoveElementsByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(values), val);
        var actual = LeetCodeWireFormat.FromLinkedList(removed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveElementsByDummyHeadSplice_LeetCodeExamples_RemovesMatchingValues(
        int[] values, int val, int[] expected)
    {
        var removed = RemoveLinkedListElementsSolution.RemoveElementsByDummyHeadSplice(
            LeetCodeWireFormat.ToLinkedList(values), val);
        var actual = LeetCodeWireFormat.FromLinkedList(removed);

        Assert.Equal(expected, actual);
    }
}
