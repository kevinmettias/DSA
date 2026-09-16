using DSAExperimentation.LeetCode.ContainsDuplicateII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicateII;

// Harness only: both strategies live in ContainsDuplicateIISolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a failure
// names the strategy that broke.
public sealed partial class ContainsDuplicateIITests
{
    public static TheoryData<NearbyDuplicateCase> Examples =>
        new()
        {
            { new NearbyDuplicateCase([1, 2, 3, 1], 3, Expected: true) },
            { new NearbyDuplicateCase([1, 0, 1, 1], 1, Expected: true) },
            { new NearbyDuplicateCase([1, 2, 3, 1, 2, 3], 2, Expected: false) },
            { new NearbyDuplicateCase([1, 1], 1, Expected: true) },
            { new NearbyDuplicateCase([1, 1], 0, Expected: false) },
            { new NearbyDuplicateCase([1], 1, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasNearbyDuplicateByBruteForce_LeetCodeExamples_ReturnsExpected(
        NearbyDuplicateCase example)
    {
        var actual = ContainsDuplicateIISolution.HasNearbyDuplicateByBruteForce(
            example.Nums, example.IndexDiff);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasNearbyDuplicateByHashMap_LeetCodeExamples_ReturnsExpected(
        NearbyDuplicateCase example)
    {
        var actual = ContainsDuplicateIISolution.HasNearbyDuplicateByHashMap(
            example.Nums, example.IndexDiff);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the array, the index distance that must separate two equal
    // values, and whether such a pair exists. The expected value is named at every
    // construction site, so a row reads as the case it is rather than as a bare `true`
    // whose meaning is its position. Nested because it is only ever used inside this
    // test class - it is this harness's own vocabulary, not a type another file would
    // import.
    public readonly record struct NearbyDuplicateCase(int[] Nums, int IndexDiff, bool Expected);
}
