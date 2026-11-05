using DSAExperimentation.LeetCode.Solutions.LongestIncreasingSubsequenceII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestIncreasingSubsequenceII;

// Harness only: both strategies are LongestIncreasingSubsequenceIISolution's. This
// pins a run the gap rule rejects at every step, a strictly decreasing run that no k
// can lengthen, and a single element. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class LongestIncreasingSubsequenceIISolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            // A run with gaps of two: k = 1 rejects every pair, leaving singletons.
            { [1, 3, 5, 7], 1, 1 },
            // Strictly decreasing, so no pair is increasing however wide k is.
            { [5, 4, 3, 2, 1], 10, 1 },
            { [10], 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LengthOfLisByDynamicProgramming_Cases_ReturnsLongestConstrainedLength(
        int[] nums, int maxGap, int expected)
    {
        var actual = LongestIncreasingSubsequenceIISolution.LengthOfLisByDynamicProgramming(nums, maxGap);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LengthOfLisBySegmentTreeValueWindow_Cases_ReturnsLongestConstrainedLength(
        int[] nums, int maxGap, int expected)
    {
        var actual = LongestIncreasingSubsequenceIISolution.LengthOfLisBySegmentTreeValueWindow(nums, maxGap);

        Assert.Equal(expected, actual);
    }
}
