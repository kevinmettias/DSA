using DSAExperimentation.LeetCode.CountSubarraysWithFixedBounds;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubarraysWithFixedBounds;

// Harness only. Both counting strategies are CountSubarraysWithFixedBoundsSolution's
// - the triple-nested rescan that used to live untested as the benchmark baseline,
// and the SegmentTree min/max pair - pinned here to LeetCode's published examples
// plus the cases that exercise an out-of-range element splitting the array
// ([1, 5, 9, 1, 5], where the 9 blocks every subarray that spans it) and a bound
// that no element reaches at all.
public sealed class CountSubarraysWithFixedBoundsTests
{
    public static TheoryData<int[], int, int, long> Examples =>
        new()
        {
            { [1, 3, 5, 2, 7, 5], 1, 5, 2L },
            { [1, 1, 1, 1], 1, 1, 10L },
            { [2, 2, 2], 1, 5, 0L },
            { [1, 5, 1, 5], 1, 5, 6L },
            { [1, 5, 9, 1, 5], 1, 5, 2L },
            { [3, 3], 3, 3, 3L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountFixedBoundSubarraysByRescan_LeetCodeExamples_ReturnsQualifyingSubarrayCount(
        int[] nums, int minK, int maxK, long expected)
    {
        var actual = CountSubarraysWithFixedBoundsSolution.CountFixedBoundSubarraysByRescan(nums, minK, maxK);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountFixedBoundSubarraysBySegmentTreeQueries_LeetCodeExamples_ReturnsQualifyingSubarrayCount(
        int[] nums, int minK, int maxK, long expected)
    {
        var actual = CountSubarraysWithFixedBoundsSolution.CountFixedBoundSubarraysBySegmentTreeQueries(nums, minK, maxK);
        Assert.Equal(expected, actual);
    }
}
