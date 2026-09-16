using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindBeautifulIndicesInTheGivenArrayII;

// Harness only: the algorithms live in
// FindBeautifulIndicesInTheGivenArrayIISolution. #3008 publishes the same two
// examples as #3006 - only the constraints differ - so this file pins both
// strategies to that shared set. One test method per strategy, so a failure names
// the strategy that broke.
public sealed class FindBeautifulIndicesInTheGivenArrayIITests
{
    public static TheoryData<BeautifulIndicesCase> Examples =>
        new()
        {
            { new BeautifulIndicesCase(S: "isawsquirrelnearmysquirrelhouseohmy", A: "my", B: "squirrel", K: 15, Expected: [16, 33]) },
            { new BeautifulIndicesCase(S: "abcd", A: "a", B: "a", K: 4, Expected: [0]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindBeautifulIndicesByBruteForce_LeetCodeExamples_ReturnsSortedBeautifulIndices(
        BeautifulIndicesCase example)
    {
        var actual = FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByBruteForce(
            new FindBeautifulIndicesInTheGivenArrayIISolution.Haystack(example.S),
            new FindBeautifulIndicesInTheGivenArrayIISolution.PrefixPattern(example.A),
            new FindBeautifulIndicesInTheGivenArrayIISolution.NearbyPattern(example.B),
            example.K);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindBeautifulIndicesByZFunction_LeetCodeExamples_ReturnsSortedBeautifulIndices(
        BeautifulIndicesCase example)
    {
        var actual = FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByZFunction(
            new FindBeautifulIndicesInTheGivenArrayIISolution.Haystack(example.S),
            new FindBeautifulIndicesInTheGivenArrayIISolution.PrefixPattern(example.A),
            new FindBeautifulIndicesInTheGivenArrayIISolution.NearbyPattern(example.B),
            example.K);

        Assert.Equal(example.Expected, actual);
    }

    // One published example: the text to search, the pattern whose occurrences are the
    // candidates, the pattern each candidate must sit near, how near, and the indices
    // LeetCode says qualify. The three strings are the same type and none of them is
    // interchangeable with another, so the row names which is which. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct BeautifulIndicesCase(
        string S, string A, string B, int K, int[] Expected);
}
