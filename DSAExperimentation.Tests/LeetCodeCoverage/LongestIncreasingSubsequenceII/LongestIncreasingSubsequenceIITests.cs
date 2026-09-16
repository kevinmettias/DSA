using DSAExperimentation.LeetCode.LongestIncreasingSubsequenceII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestIncreasingSubsequenceII;

// Harness only: both strategies are LongestIncreasingSubsequenceIISolution's. Beyond
// LeetCode's three published examples this pins the two ends of the k constraint - a
// run the gap rule admits whole, and a run it rejects at every step.
public sealed partial class LongestIncreasingSubsequenceIITests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [4, 2, 1, 4, 3, 4, 5, 8, 15], 3, 5 },
            { [7, 4, 5, 1, 8, 12, 4, 7], 5, 4 },
            { [1, 5], 1, 1 },
            // Consecutive values, so a gap of one admits the whole run.
            { [1, 2, 3, 4, 5], 1, 5 },
            // The same run with gaps of two: k rejects every pair, leaving singletons.
            { [1, 3, 5, 7], 1, 1 },
            // Strictly decreasing, so no pair is increasing however wide k is.
            { [5, 4, 3, 2, 1], 10, 1 },
            { [10], 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthOfLisByDynamicProgramming_LeetCodeExamples_ReturnsLongestConstrainedLength(
        int[] nums, int maxGap, int expected)
    {
        var actual = LongestIncreasingSubsequenceIISolution.LengthOfLisByDynamicProgramming(nums, maxGap);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthOfLisBySegmentTreeValueWindow_LeetCodeExamples_ReturnsLongestConstrainedLength(
        int[] nums, int maxGap, int expected)
    {
        var actual = LongestIncreasingSubsequenceIISolution.LengthOfLisBySegmentTreeValueWindow(nums, maxGap);

        Assert.Equal(expected, actual);
    }
}
