using DSAExperimentation.LeetCode.Solutions.ReverseInteger;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReverseInteger;

// Harness only: both strategies live in ReverseIntegerSolution and are asserted
// against the same cases. They are read by hand from the digits, against int's range
// [-2147483648, 2147483647]:
// - -1563847412 reverses to -2147483651, which leaves the range, so it answers 0.
// - 1463847412 reverses to 2147483641 and -1463847412 to -2147483641: both fit, and
//   both reach int's headroom (214748364 and -214748364) before their last digit, so
//   the guard must let a small enough final digit through at exactly that bound.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ReverseIntegerSolutionTests
{
    public static TheoryData<int, int> Cases =>
        new()
        {
            { -1563847412, 0 },
            { 1463847412, 2147483641 },
            { -1463847412, -2147483641 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReverseByArithmetic_Cases_ReturnsReversedDigits(int value, int expected) =>
        Assert.Equal(expected, ReverseIntegerSolution.ReverseByArithmetic(value));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReverseByDigitStack_Cases_ReturnsReversedDigits(int value, int expected) =>
        Assert.Equal(expected, ReverseIntegerSolution.ReverseByDigitStack(value));
}
