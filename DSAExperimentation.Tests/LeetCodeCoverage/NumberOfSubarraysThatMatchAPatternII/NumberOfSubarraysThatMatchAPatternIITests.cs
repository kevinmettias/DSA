using DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfSubarraysThatMatchAPatternII;

// Harness only. Both strategies live in
// NumberOfSubarraysThatMatchAPatternIISolution - this file just pins them to
// LeetCode's published examples (identical to 3034's, since 3036 restates
// the same problem at a larger bound).
public sealed class NumberOfSubarraysThatMatchAPatternIITests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, 6], [1, 1], 4 },
            { [1, 4, 4, 1, 3, 5, 5, 3], [1, 0, -1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchesByBruteForce_LeetCodeExamples_ReturnsSubarrayCount(int[] nums, int[] pattern, int expected)
    {
        var actual = NumberOfSubarraysThatMatchAPatternIISolution.CountMatchesByBruteForce(nums, pattern);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchesByZFunction_LeetCodeExamples_ReturnsSubarrayCount(
        int[] nums, int[] pattern, int expected)
    {
        var actual = NumberOfSubarraysThatMatchAPatternIISolution.CountMatchesByZFunction(nums, pattern);

        Assert.Equal(expected, actual);
    }
}
