using DSAExperimentation.LeetCode.MaximizeSubarrayGCDScore;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeSubarrayGCDScore;

// Harness only. Both strategies are MaximizeSubarrayGCDScoreSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class MaximizeSubarrayGCDScoreTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [2, 4], 1, 8L },
            { [3, 5, 7], 2, 14L },
            { [5, 5, 5], 1, 15L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreByBruteForce_LeetCodeExamples_ReturnsMaximumScore(int[] nums, int k, long expected) =>
        Assert.Equal(expected, MaximizeSubarrayGCDScoreSolution.MaxScoreByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreByBottleneckGcdScan_LeetCodeExamples_ReturnsMaximumScore(int[] nums, int k, long expected) =>
        Assert.Equal(expected, MaximizeSubarrayGCDScoreSolution.MaxScoreByBottleneckGcdScan(nums, k));
}
