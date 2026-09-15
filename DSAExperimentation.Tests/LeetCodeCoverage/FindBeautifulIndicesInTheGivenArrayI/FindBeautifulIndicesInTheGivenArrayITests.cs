using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindBeautifulIndicesInTheGivenArrayI;

// Harness only: the algorithms live in
// FindBeautifulIndicesInTheGivenArrayISolution. One test method per strategy over
// one shared set of LeetCode's own examples, so a failure names the strategy that
// broke.
public sealed class FindBeautifulIndicesInTheGivenArrayITests
{
    public static TheoryData<string, string, string, int, int[]> Examples =>
        new()
        {
            { "isawsquirrelnearmysquirrelhouseohmy", "my", "squirrel", 15, [16, 33] },
            { "abcd", "a", "a", 4, [0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindBeautifulIndicesByBruteForce_LeetCodeExamples_ReturnsSortedBeautifulIndices(
        string s, string a, string b, int k, int[] expected) =>
        Assert.Equal(
            expected,
            FindBeautifulIndicesInTheGivenArrayISolution.FindBeautifulIndicesByBruteForce(
                new FindBeautifulIndicesInTheGivenArrayISolution.SearchedText(s),
                new FindBeautifulIndicesInTheGivenArrayISolution.AnchorPattern(a),
                new FindBeautifulIndicesInTheGivenArrayISolution.NearbyPattern(b),
                k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindBeautifulIndicesByPrefixFunctionSearch_LeetCodeExamples_ReturnsSortedBeautifulIndices(
        string s, string a, string b, int k, int[] expected) =>
        Assert.Equal(
            expected,
            FindBeautifulIndicesInTheGivenArrayISolution.FindBeautifulIndicesByPrefixFunctionSearch(
                new FindBeautifulIndicesInTheGivenArrayISolution.SearchedText(s),
                new FindBeautifulIndicesInTheGivenArrayISolution.AnchorPattern(a),
                new FindBeautifulIndicesInTheGivenArrayISolution.NearbyPattern(b),
                k));
}
