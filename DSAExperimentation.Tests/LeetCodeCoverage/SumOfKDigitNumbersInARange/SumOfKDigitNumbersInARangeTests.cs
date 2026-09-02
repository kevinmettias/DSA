using DSAExperimentation.LeetCode.SumOfKDigitNumbersInARange;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfKDigitNumbersInARange;

// Harness only: both strategies live in SumOfKDigitNumbersInARangeSolution and
// are asserted against the same examples, so a failure names the strategy that
// broke.
public sealed class SumOfKDigitNumbersInARangeTests
{
    public static TheoryData<int, int, int, long> Examples =>
        new()
        {
            { 1, 2, 2, 66 },
            { 0, 1, 3, 444 },
            { 5, 5, 10, 555_555_520 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfKDigitNumbersByBruteForceEnumeration_LeetCodeExamples_ReturnsSumModulo(
        int l, int r, int k, long expected) =>
        Assert.Equal(expected, SumOfKDigitNumbersInARangeSolution.SumOfKDigitNumbersByBruteForceEnumeration(l, r, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfKDigitNumbersByModularRepunit_LeetCodeExamples_ReturnsSumModulo(
        int l, int r, int k, long expected) =>
        Assert.Equal(expected, SumOfKDigitNumbersInARangeSolution.SumOfKDigitNumbersByModularRepunit(l, r, k));
}
