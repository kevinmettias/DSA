using DSAExperimentation.LeetCode.AddTwoNumbers;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.LeetCode.Tests.AddTwoNumbers;

// Harness only. Both strategies are AddTwoNumbersSolution's - this file pins them
// to LeetCode's published examples, stated once as digit arrays in the same
// least-significant-digit-first order LC 2's own lists use.
public sealed partial class AddTwoNumbersSolutionTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [2, 4, 3], [5, 6, 4], [7, 0, 8] },
            { [9, 9, 9], [1], [0, 0, 0, 1] },
            { [2, 4, 9], [5, 6], [7, 0, 0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByDigitwiseListWalk_LeetCodeExamples_ReturnsDigitwiseSumInReverseOrder(
        int[] first, int[] second, int[] expected)
    {
        var sum = AddTwoNumbersSolution.AddByDigitwiseListWalk(
            LeetCodeWireFormat.ToLinkedList(first), LeetCodeWireFormat.ToLinkedList(second));
        var actual = LeetCodeWireFormat.FromLinkedList(sum);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBigIntegerConvertAndBack_LeetCodeExamples_ReturnsDigitwiseSumInReverseOrder(
        int[] first, int[] second, int[] expected)
    {
        var sum = AddTwoNumbersSolution.AddByBigIntegerConvertAndBack(
            LeetCodeWireFormat.ToLinkedList(first), LeetCodeWireFormat.ToLinkedList(second));
        var actual = LeetCodeWireFormat.FromLinkedList(sum);

        Assert.Equal(expected, actual);
    }
}
