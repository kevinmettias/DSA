using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ConvertBinaryNumberInALinkedListToInteger;

namespace DSAExperimentation.LeetCode.Tests.ConvertBinaryNumberInALinkedListToInteger;

// Harness only. Both strategies are ConvertBinaryNumberInALinkedListToIntegerSolution's -
// this file states LeetCode's examples once as the list's bits plus the decimal
// value they encode, and asserts each strategy against them.
public sealed partial class ConvertBinaryNumberInALinkedListToIntegerSolutionTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 0, 1], 5 },
            { [0], 0 },
            { [1], 1 },
            { [1, 1, 1, 1, 1], 31 },
            { [1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0], 18_880 },
            { [0, 0], 0 },
            { [0, 1, 1], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetDecimalValueBySinglePassShift_LeetCodeExamples_ReturnsDecimalValue(int[] bits, int expected) =>
        Assert.Equal(
            expected,
            ConvertBinaryNumberInALinkedListToIntegerSolution.GetDecimalValueBySinglePassShift(
                LeetCodeWireFormat.ToLinkedList(bits)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetDecimalValueByCollectThenFold_LeetCodeExamples_ReturnsDecimalValue(int[] bits, int expected) =>
        Assert.Equal(
            expected,
            ConvertBinaryNumberInALinkedListToIntegerSolution.GetDecimalValueByCollectThenFold(
                LeetCodeWireFormat.ToLinkedList(bits)));
}
