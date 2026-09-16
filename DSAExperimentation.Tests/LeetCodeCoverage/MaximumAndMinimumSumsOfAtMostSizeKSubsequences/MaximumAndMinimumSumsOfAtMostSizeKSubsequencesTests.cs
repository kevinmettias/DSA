using DSAExperimentation.LeetCode.MaximumAndMinimumSumsOfAtMostSizeKSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumAndMinimumSumsOfAtMostSizeKSubsequences;

// Harness only. Both strategies are
// MaximumAndMinimumSumsOfAtMostSizeKSubsequencesSolution's - this file just pins
// them to LeetCode's published examples, including the all-duplicates case that
// exercises the max/min tables identically.
public sealed class MaximumAndMinimumSumsOfAtMostSizeKSubsequencesTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 2, 3], 2, 24 },
            { [5, 0, 6], 1, 22 },
            { [1, 1, 1], 2, 12 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumByPascalTriangle_LeetCodeExamples_ReturnsMaxPlusMinSum(
        int[] nums, int maxLength, long expected)
    {
        var actual =
            MaximumAndMinimumSumsOfAtMostSizeKSubsequencesSolution.SumByPascalTriangle(nums, maxLength);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumByFactorialCombinatorics_LeetCodeExamples_ReturnsMaxPlusMinSum(
        int[] nums, int maxLength, long expected)
    {
        var actual =
            MaximumAndMinimumSumsOfAtMostSizeKSubsequencesSolution.SumByFactorialCombinatorics(nums, maxLength);

        Assert.Equal(expected, actual);
    }
}
