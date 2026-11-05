using DSAExperimentation.LeetCode.Solutions.ContainsDuplicateIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ContainsDuplicateIII;

// Harness only: both strategies are ContainsDuplicateIIISolution's - this file
// just pins them to two edge cases of the bucketing: equal negative values, which
// land in a floored bucket below zero, and a value distance of int.MaxValue, whose
// bucket width only fits in the long-keyed HashMap the bucketed strategy uses.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ContainsDuplicateIIISolutionTests
{
    public static TheoryData<NearbyAlmostDuplicateCase> Cases =>
        new()
        {
            { new NearbyAlmostDuplicateCase([-1, -1], 1, 0, Expected: true) },
            { new NearbyAlmostDuplicateCase([-1, 2147483647], 1, 2147483647, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasNearbyAlmostDuplicateByBucketedHashMap_Cases_ReturnsExpected(
        NearbyAlmostDuplicateCase testCase)
    {
        var actual = ContainsDuplicateIIISolution.HasNearbyAlmostDuplicateByBucketedHashMap(
            testCase.Nums, testCase.IndexDiff, testCase.ValueDiff);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasNearbyAlmostDuplicateBySlidingWindowBruteForce_Cases_ReturnsExpected(
        NearbyAlmostDuplicateCase testCase)
    {
        var actual = ContainsDuplicateIIISolution.HasNearbyAlmostDuplicateBySlidingWindowBruteForce(
            testCase.Nums, testCase.IndexDiff, testCase.ValueDiff);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the array, the index and value distances that must
    // separate two values, and whether such a pair exists. The expected value is
    // named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct NearbyAlmostDuplicateCase(int[] Nums, int IndexDiff, int ValueDiff, bool Expected);
}
