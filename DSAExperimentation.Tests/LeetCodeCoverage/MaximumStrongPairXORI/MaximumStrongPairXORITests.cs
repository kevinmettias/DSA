using DSAExperimentation.LeetCode.MaximumStrongPairXORI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumStrongPairXORI;

// Harness only. Both strategies live in MaximumStrongPairXORISolution - this
// file just pins them to LeetCode's published examples, including the case
// where no strong pair exists at all (nums = [10, 100]).
public sealed partial class MaximumStrongPairXORITests
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
        Assert.Equal(expected, MaximumStrongPairXORISolution.MaximumStrongPairXorByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumStrongPairXorByBitTrieBuckets_LeetCodeExamples_ReturnsMaxStrongPairXor(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumStrongPairXORISolution.MaximumStrongPairXorByBitTrieBuckets(nums));
}
