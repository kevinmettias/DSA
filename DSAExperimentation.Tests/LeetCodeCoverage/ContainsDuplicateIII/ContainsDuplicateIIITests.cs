using DSAExperimentation.LeetCode.ContainsDuplicateIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicateIII;

// Harness only: both strategies are ContainsDuplicateIIISolution's - this file
// just pins them to LeetCode's published examples, including the two unopenable
// INT_MIN/INT_MAX cases the bucketed strategy's long-keyed HashMap exists for.
public sealed class ContainsDuplicateIIITests
{
    public static TheoryData<int[], int, int, bool> Examples =>
        new()
        {
            { [1, 2, 3, 1], 3, 0, true },
            { [1, 0, 1, 1], 1, 2, true },
            { [1, 5, 9, 1, 5, 9], 2, 3, false },
            { [-1, -1], 1, 0, true },
            { [-1, 2147483647], 1, 2147483647, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsNearbyAlmostDuplicateByBucketedHashMap_LeetCodeExamples_ReturnsExpected(
        int[] nums, int indexDiff, int valueDiff, bool expected) =>
        Assert.Equal(
            expected,
            ContainsDuplicateIIISolution.ContainsNearbyAlmostDuplicateByBucketedHashMap(nums, indexDiff, valueDiff));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsNearbyAlmostDuplicateBySlidingWindowBruteForce_LeetCodeExamples_ReturnsExpected(
        int[] nums, int indexDiff, int valueDiff, bool expected) =>
        Assert.Equal(
            expected,
            ContainsDuplicateIIISolution.ContainsNearbyAlmostDuplicateBySlidingWindowBruteForce(nums, indexDiff, valueDiff));
}
