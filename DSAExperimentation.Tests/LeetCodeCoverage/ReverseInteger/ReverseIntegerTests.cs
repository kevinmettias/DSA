using DSAExperimentation.LeetCode.ReverseInteger;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseInteger;

// Harness only: both strategies live in ReverseIntegerSolution and are asserted
// against the same examples, including the two inputs whose reversal overflows a
// 32-bit int.
public sealed partial class ReverseIntegerTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 123, 321 },
            { -123, -321 },
            { 120, 21 },
            { 0, 0 },
            { 1534236469, 0 },
            { -1563847412, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseByArithmetic_LeetCodeExamples_ReturnsReversedDigits(int value, int expected) =>
        Assert.Equal(expected, ReverseIntegerSolution.ReverseByArithmetic(value));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseByDigitStack_LeetCodeExamples_ReturnsReversedDigits(int value, int expected) =>
        Assert.Equal(expected, ReverseIntegerSolution.ReverseByDigitStack(value));
}
