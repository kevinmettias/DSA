using DSAExperimentation.LeetCode.CountNoZeroPairsThatSumToN;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNoZeroPairsThatSumToN;

// Harness only. CountNoZeroPairsThatSumToNSolution owns both the brute-force
// baseline and the memoized digit-DP recurrence; this file pins them to
// LeetCode's three published examples.
public sealed partial class CountNoZeroPairsThatSumToNTests
{
    public static TheoryData<long, long> Examples =>
        new()
        {
            { 2L, 1L },
            { 3L, 2L },
            { 11L, 8L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBruteForce_LeetCodeExamples_ReturnsNoZeroPairCount(long targetSum, long expected) =>
        Assert.Equal(expected, CountNoZeroPairsThatSumToNSolution.CountPairsByBruteForce(targetSum));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByMemoizedDigitDp_LeetCodeExamples_ReturnsNoZeroPairCount(long targetSum, long expected) =>
        Assert.Equal(expected, CountNoZeroPairsThatSumToNSolution.CountPairsByMemoizedDigitDp(targetSum));
}
