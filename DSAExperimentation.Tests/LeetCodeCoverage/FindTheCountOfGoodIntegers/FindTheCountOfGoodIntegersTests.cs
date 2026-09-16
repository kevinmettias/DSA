using DSAExperimentation.LeetCode.FindTheCountOfGoodIntegers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheCountOfGoodIntegers;

// Harness only: both strategies live in FindTheCountOfGoodIntegersSolution - this
// file just pins them to LeetCode's published examples.
public sealed partial class FindTheCountOfGoodIntegersTests
{
    public static TheoryData<int, int, long> Examples =>
        new()
        {
            { 3, 5, 27 },
            { 1, 4, 2 },
            { 5, 6, 2468 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByPalindromeEnumeration_LeetCodeExamples_ReturnsGoodIntegerCount(
        int digitCount, int divisor, long expected)
    {
        var actual = FindTheCountOfGoodIntegersSolution.CountByPalindromeEnumeration(digitCount, divisor);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBacktrackEnumeration_LeetCodeExamples_ReturnsGoodIntegerCount(
        int digitCount, int divisor, long expected)
    {
        var actual = FindTheCountOfGoodIntegersSolution.CountByBacktrackEnumeration(digitCount, divisor);

        Assert.Equal(expected, actual);
    }
}
