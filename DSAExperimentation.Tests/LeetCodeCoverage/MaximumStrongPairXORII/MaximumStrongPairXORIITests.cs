using DSAExperimentation.LeetCode.MaximumStrongPairXORII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumStrongPairXORII;

// Harness only. Both strategies live in MaximumStrongPairXORIISolution - this
// file just pins them to LeetCode's published examples, the same ones LC 2932
// publishes for the identical question at a smaller bound, including the case
// where no strong pair exists at all (nums = [10, 100]).
public sealed class MaximumStrongPairXORIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 7 },
            { [10, 100], 0 },
            { [5, 6, 25, 30], 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumStrongPairXorByBruteForce_LeetCodeExamples_ReturnsMaxStrongPairXor(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumStrongPairXORIISolution.MaximumStrongPairXorByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumStrongPairXorByBitTrieBuckets_LeetCodeExamples_ReturnsMaxStrongPairXor(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumStrongPairXORIISolution.MaximumStrongPairXorByBitTrieBuckets(nums));
}
