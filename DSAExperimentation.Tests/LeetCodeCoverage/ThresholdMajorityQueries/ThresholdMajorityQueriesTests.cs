using DSAExperimentation.LeetCode.ThresholdMajorityQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThresholdMajorityQueries;

// Harness only. Both query strategies are ThresholdMajorityQueriesSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class ThresholdMajorityQueriesTests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            {
                [1, 1, 2, 2, 1, 1],
                [[0, 5, 4], [0, 3, 3], [2, 3, 2]],
                [1, -1, 2]
            },
            {
                [3, 2, 3, 2, 3, 2, 3],
                [[0, 6, 4], [1, 5, 2], [2, 4, 1], [3, 3, 1]],
                [3, 2, 3, 2]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubarrayMajorityByBruteForce_LeetCodeExamples_ReturnsHighestFrequencyElementMeetingThreshold(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = ThresholdMajorityQueriesSolution.SubarrayMajorityByBruteForce(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubarrayMajorityByBlockMode_LeetCodeExamples_ReturnsHighestFrequencyElementMeetingThreshold(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = ThresholdMajorityQueriesSolution.SubarrayMajorityByBlockMode(nums, queries);

        Assert.Equal(expected, actual);
    }
}
