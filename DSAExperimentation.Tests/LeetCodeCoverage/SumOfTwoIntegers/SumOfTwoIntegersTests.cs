using DSAExperimentation.LeetCode.SumOfTwoIntegers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfTwoIntegers;

// Harness only: both strategies live in SumOfTwoIntegersSolution and are
// asserted against the same published examples.
public sealed class SumOfTwoIntegersTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 1, 2, 3 },
            { 2, 3, 5 },
            { -2, 3, 1 },
            { -1, -1, -2 },
            { 0, 0, 0 },
            { int.MaxValue, 0, int.MaxValue },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetSumByBuiltInAddition_LeetCodeExamples_ReturnsArithmeticSum(
        int a, int b, int expected)
    {
        var actual = SumOfTwoIntegersSolution.GetSumByBuiltInAddition(a, b);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetSumByBitwiseCarryLoop_LeetCodeExamples_ReturnsArithmeticSum(
        int a, int b, int expected)
    {
        var actual = SumOfTwoIntegersSolution.GetSumByBitwiseCarryLoop(a, b);

        Assert.Equal(expected, actual);
    }
}
