using DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfSubarraysThatMatchAPatternI;

// Harness only. Both strategies live in
// NumberOfSubarraysThatMatchAPatternISolution - this file just pins them to
// LeetCode's published examples.
public sealed class NumberOfSubarraysThatMatchAPatternITests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, 6], [1, 1], 4 },
            { [1, 4, 4, 1, 3, 5, 5, 3], [1, 0, -1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchesByBruteForce_LeetCodeExamples_ReturnsSubarrayCount(int[] nums, int[] pattern, int expected) =>
        Assert.Equal(expected, NumberOfSubarraysThatMatchAPatternISolution.CountMatchesByBruteForce(nums, pattern));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchesByPrefixFunctionSearch_LeetCodeExamples_ReturnsSubarrayCount(int[] nums, int[] pattern, int expected) =>
        Assert.Equal(expected, NumberOfSubarraysThatMatchAPatternISolution.CountMatchesByPrefixFunctionSearch(nums, pattern));
}
