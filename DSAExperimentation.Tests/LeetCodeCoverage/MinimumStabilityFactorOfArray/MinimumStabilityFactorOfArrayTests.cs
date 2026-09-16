using DSAExperimentation.LeetCode.MinimumStabilityFactorOfArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumStabilityFactorOfArray;

// Harness only. Both strategies are MinimumStabilityFactorOfArraySolution's - this
// file just pins them to LeetCode's published examples, including the two
// separate stable runs in Example 3 that one modification cannot both break.
public sealed partial class MinimumStabilityFactorOfArrayTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [3, 5, 10], 1, 1 },
            { [2, 6, 8], 2, 1 },
            { [2, 4, 9, 6], 1, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinStabilityByBruteForceGcdScan_LeetCodeExamples_ReturnsMinimumAchievableStabilityFactor(
        int[] nums, int maxC, int expected)
    {
        var actual = MinimumStabilityFactorOfArraySolution.MinStabilityByBruteForceGcdScan(nums, maxC);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinStabilityBySegmentTreeGcd_LeetCodeExamples_ReturnsMinimumAchievableStabilityFactor(
        int[] nums, int maxC, int expected)
    {
        var actual = MinimumStabilityFactorOfArraySolution.MinStabilityBySegmentTreeGcd(nums, maxC);
        Assert.Equal(expected, actual);
    }
}
