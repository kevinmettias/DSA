using DSAExperimentation.LeetCode.Solutions.ContainsDuplicateII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ContainsDuplicateII;

// Harness only: both strategies live in ContainsDuplicateIISolution. One test
// method per strategy over one shared set of cases - the shortest array that holds a
// nearby duplicate and a single element with nothing to pair - so a failure names the
// strategy that broke. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ContainsDuplicateIISolutionTests
{
    public static TheoryData<NearbyDuplicateCase> Cases =>
        new()
        {
            { new NearbyDuplicateCase([1, 1], 1, Expected: true) },
            { new NearbyDuplicateCase([1], 1, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasNearbyDuplicateByBruteForce_Cases_ReturnsExpected(
        NearbyDuplicateCase testCase)
    {
        var actual = ContainsDuplicateIISolution.HasNearbyDuplicateByBruteForce(
            testCase.Nums, testCase.IndexDiff);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasNearbyDuplicateByHashMap_Cases_ReturnsExpected(
        NearbyDuplicateCase testCase)
    {
        var actual = ContainsDuplicateIISolution.HasNearbyDuplicateByHashMap(
            testCase.Nums, testCase.IndexDiff);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the array, the index distance that must separate two equal
    // values, and whether such a pair exists. The expected value is named at every
    // construction site, so a row reads as the case it is rather than as a bare `true`
    // whose meaning is its position. Nested because it is only ever used inside this
    // test class - it is this harness's own vocabulary, not a type another file would
    // import.
    public readonly record struct NearbyDuplicateCase(int[] Nums, int IndexDiff, bool Expected);
}
