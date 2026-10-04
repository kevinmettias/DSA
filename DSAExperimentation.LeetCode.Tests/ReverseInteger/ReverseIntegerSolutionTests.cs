using DSAExperimentation.LeetCode.ReverseInteger;

namespace DSAExperimentation.LeetCode.Tests.ReverseInteger;

// Harness only: both strategies live in ReverseIntegerSolution and are asserted
// against the same examples. The first three rows are LeetCode's published examples.
// The rest are read by hand from the digits, against int's range
// [-2147483648, 2147483647]:
// - 0 has no digits to move and stays 0.
// - 1534236469 reverses to 9646324351, and -1563847412 to -2147483651: both leave the
//   range, so both answer 0.
// - 1463847412 reverses to 2147483641 and -1463847412 to -2147483641: both fit, and
//   both reach int's headroom (214748364 and -214748364) before their last digit, so
//   the guard must let a small enough final digit through at exactly that bound.
// - int.MaxValue reverses to 7463847412 and int.MinValue to -8463847412: both leave
//   the range. int.MinValue is also the one input whose absolute value an int cannot
//   hold, so it shows a strategy never negates its way through the positive range.
public sealed partial class ReverseIntegerSolutionTests
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
            { 1463847412, 2147483641 },
            { -1463847412, -2147483641 },
            { int.MaxValue, 0 },
            { int.MinValue, 0 },
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
