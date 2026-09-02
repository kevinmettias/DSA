using DSAExperimentation.LeetCode.FindTheCountOfGoodIntegers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheCountOfGoodIntegers;

// Harness only: both strategies live in FindTheCountOfGoodIntegersSolution - this
// file just pins them to LeetCode's published examples.
public sealed class FindTheCountOfGoodIntegersTests
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
    public void CountByPalindromeEnumeration_LeetCodeExamples_ReturnsGoodIntegerCount(int n, int k, long expected) =>
        Assert.Equal(expected, FindTheCountOfGoodIntegersSolution.CountByPalindromeEnumeration(n, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBacktrackEnumeration_LeetCodeExamples_ReturnsGoodIntegerCount(int n, int k, long expected) =>
        Assert.Equal(expected, FindTheCountOfGoodIntegersSolution.CountByBacktrackEnumeration(n, k));
}
