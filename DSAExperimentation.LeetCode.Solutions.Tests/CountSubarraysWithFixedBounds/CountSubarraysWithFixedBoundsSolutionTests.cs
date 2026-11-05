using DSAExperimentation.LeetCode.Solutions.CountSubarraysWithFixedBounds;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountSubarraysWithFixedBounds;

// Harness only. Both counting strategies are CountSubarraysWithFixedBoundsSolution's
// - the triple-nested rescan that used to live untested as the benchmark baseline,
// and the SegmentTree min/max pair - pinned here to cases that exercise a bound that
// no element reaches at all, an array made only of the two bounds, an out-of-range
// element splitting the array ([1, 5, 9, 1, 5], where the 9 blocks every subarray
// that spans it), and equal bounds. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class CountSubarraysWithFixedBoundsSolutionTests
{
    public static TheoryData<int[], int, int, long> Cases =>
        new()
        {
            { [2, 2, 2], 1, 5, 0L },
            { [1, 5, 1, 5], 1, 5, 6L },
            { [1, 5, 9, 1, 5], 1, 5, 2L },
            { [3, 3], 3, 3, 3L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountFixedBoundSubarraysByRescan_Cases_ReturnsQualifyingSubarrayCount(
        int[] nums, int minK, int maxK, long expected)
    {
        var actual = CountSubarraysWithFixedBoundsSolution.CountFixedBoundSubarraysByRescan(nums, minK, maxK);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountFixedBoundSubarraysBySegmentTreeQueries_Cases_ReturnsQualifyingSubarrayCount(
        int[] nums, int minK, int maxK, long expected)
    {
        var actual = CountSubarraysWithFixedBoundsSolution.CountFixedBoundSubarraysBySegmentTreeQueries(nums, minK, maxK);
        Assert.Equal(expected, actual);
    }
}
