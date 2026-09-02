using DSAExperimentation.LeetCode.DivideTwoIntegers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DivideTwoIntegers;

// Harness only: both strategies live in DivideTwoIntegersSolution and are
// asserted against the same published examples, including the two int32-overflow
// edge cases LeetCode calls out explicitly.
public sealed class DivideTwoIntegersTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 10, 3, 3 },
            { 7, -3, -2 },
            { int.MinValue, -1, int.MaxValue },
            { int.MinValue, 1, int.MinValue },
            { int.MaxValue, 7, 306_783_378 },
            { int.MinValue, 2, -1_073_741_824 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DivideByBuiltInDivision_LeetCodeExamples_TruncatesTowardZero(
        int dividend, int divisor, int expected) =>
        Assert.Equal(expected, DivideTwoIntegersSolution.DivideByBuiltInDivision(dividend, divisor));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DivideByBinarySearchProduct_LeetCodeExamples_TruncatesTowardZero(
        int dividend, int divisor, int expected) =>
        Assert.Equal(expected, DivideTwoIntegersSolution.DivideByBinarySearchProduct(dividend, divisor));
}
