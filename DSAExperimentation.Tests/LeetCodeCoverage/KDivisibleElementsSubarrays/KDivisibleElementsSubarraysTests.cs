using DSAExperimentation.LeetCode.KDivisibleElementsSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KDivisibleElementsSubarrays;

// Harness only. Both strategies are KDivisibleElementsSubarraysSolution's, and
// pinning them to the same examples is what finally puts the HashSet baseline -
// previously a private helper in the benchmark, asserted by nothing - under test
// alongside the Set-backed dedupe it is measured against.
public sealed class KDivisibleElementsSubarraysTests
{
    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            // LeetCode example 1: [2,3,3,2,2] with k = 2, p = 2. Sixteen candidate
            // subarrays survive the divisible-count bound, eleven of them distinct.
            { [2, 3, 3, 2, 2], 2, 2, 11 },

            // LeetCode example 2: every element is divisible by 1 and k covers the
            // whole array, so all 4+3+2+1 subarrays qualify and none repeat.
            { [1, 2, 3, 4], 4, 1, 10 },

            // k = 0 with every element divisible by p: no subarray qualifies at all.
            { [2, 2, 2], 0, 2, 0 },

            // The mirror of the previous case: nothing is divisible, so k = 0 admits
            // everything and only distinctness thins the six candidates down to five.
            { [1, 3, 1], 0, 2, 5 },

            // Deduplication carries the whole answer here - nine candidates, three
            // distinct signatures.
            { [1, 1, 1], 3, 2, 3 },

            // The bound truncates every start after one element, leaving the three
            // singletons.
            { [2, 4, 6], 1, 2, 3 },

            // A single non-divisible element is still one qualifying subarray.
            { [5], 0, 2, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDistinctByHashSetDedupe_LeetCodeExamples_ReturnsDistinctQualifyingSubarrayCount(
        int[] nums, int k, int p, int expected)
    {
        var actual = KDivisibleElementsSubarraysSolution.CountDistinctByHashSetDedupe(nums, k, p);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDistinctBySetDedupe_LeetCodeExamples_ReturnsDistinctQualifyingSubarrayCount(
        int[] nums, int k, int p, int expected)
    {
        var actual = KDivisibleElementsSubarraysSolution.CountDistinctBySetDedupe(nums, k, p);

        Assert.Equal(expected, actual);
    }
}
