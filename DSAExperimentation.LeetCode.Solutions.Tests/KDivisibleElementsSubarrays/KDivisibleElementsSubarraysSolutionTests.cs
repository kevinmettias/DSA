using DSAExperimentation.LeetCode.Solutions.KDivisibleElementsSubarrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KDivisibleElementsSubarrays;

// Harness only. Both strategies are KDivisibleElementsSubarraysSolution's, and
// pinning them to the same cases is what finally puts the HashSet baseline -
// previously a private helper in the benchmark, asserted by nothing - under test
// alongside the Set-backed dedupe it is measured against.
public sealed partial class KDivisibleElementsSubarraysSolutionTests
{
    public static TheoryData<int[], int, int, int> Cases =>
        new()
        {
            // maxDivisibleCount = 0 with every element divisible by divisor: no subarray
            // qualifies at all.
            { [2, 2, 2], 0, 2, 0 },

            // The mirror of the previous case: nothing is divisible, so
            // maxDivisibleCount = 0 admits everything and only distinctness thins the six
            // candidates down to five.
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
    [MemberData(nameof(Cases))]
    public void CountDistinctByHashSetDedupe_Cases_ReturnsDistinctQualifyingSubarrayCount(
        int[] nums, int maxDivisibleCount, int divisor, int expected)
    {
        var actual = KDivisibleElementsSubarraysSolution.CountDistinctByHashSetDedupe(nums, maxDivisibleCount, divisor);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDistinctBySetDedupe_Cases_ReturnsDistinctQualifyingSubarrayCount(
        int[] nums, int maxDivisibleCount, int divisor, int expected)
    {
        var actual = KDivisibleElementsSubarraysSolution.CountDistinctBySetDedupe(nums, maxDivisibleCount, divisor);

        Assert.Equal(expected, actual);
    }
}
