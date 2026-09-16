using DSAExperimentation.LeetCode.ContainsDuplicateIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicateIII;

// Harness only: both strategies are ContainsDuplicateIIISolution's - this file
// just pins them to LeetCode's published examples, including the two unopenable
// INT_MIN/INT_MAX cases the bucketed strategy's long-keyed HashMap exists for.
public sealed class ContainsDuplicateIIITests
{
    public static TheoryData<NearbyAlmostDuplicateCase> Examples =>
        new()
        {
            { new NearbyAlmostDuplicateCase([1, 2, 3, 1], 3, 0, Expected: true) },
            { new NearbyAlmostDuplicateCase([1, 0, 1, 1], 1, 2, Expected: true) },
            { new NearbyAlmostDuplicateCase([1, 5, 9, 1, 5, 9], 2, 3, Expected: false) },
            { new NearbyAlmostDuplicateCase([-1, -1], 1, 0, Expected: true) },
            { new NearbyAlmostDuplicateCase([-1, 2147483647], 1, 2147483647, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsNearbyAlmostDuplicateByBucketedHashMap_LeetCodeExamples_ReturnsExpected(
        NearbyAlmostDuplicateCase example)
    {
        var actual = ContainsDuplicateIIISolution.ContainsNearbyAlmostDuplicateByBucketedHashMap(
            example.Nums, example.IndexDiff, example.ValueDiff);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsNearbyAlmostDuplicateBySlidingWindowBruteForce_LeetCodeExamples_ReturnsExpected(
        NearbyAlmostDuplicateCase example)
    {
        var actual = ContainsDuplicateIIISolution.ContainsNearbyAlmostDuplicateBySlidingWindowBruteForce(
            example.Nums, example.IndexDiff, example.ValueDiff);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the array, the index and value distances that must
    // separate two values, and whether such a pair exists. The expected value is
    // named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct NearbyAlmostDuplicateCase(int[] Nums, int IndexDiff, int ValueDiff, bool Expected);
}
