using DSAExperimentation.LeetCode.Solutions.MaximumStrongPairXORII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumStrongPairXORII;

// Harness only. Both strategies live in MaximumStrongPairXORIISolution - this
// file just pins them to [5, 6, 25, 30], the third example LC 2932 publishes for
// the identical question at a smaller bound; this problem publishes a different
// third example. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximumStrongPairXORIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [5, 6, 25, 30], 7 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumStrongPairXorByBruteForce_Cases_ReturnsMaxStrongPairXor(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumStrongPairXORIISolution.MaximumStrongPairXorByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumStrongPairXorByBitTrieBuckets_Cases_ReturnsMaxStrongPairXor(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumStrongPairXORIISolution.MaximumStrongPairXorByBitTrieBuckets(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumStrongPairXorBySlidingWindow_Cases_ReturnsMaxStrongPairXor(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumStrongPairXORIISolution.MaximumStrongPairXorBySlidingWindow(nums));
}
