using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindBeautifulIndicesInTheGivenArrayII;

// Harness only: the algorithms live in
// FindBeautifulIndicesInTheGivenArrayIISolution. #3008 publishes the same two
// examples as #3006 - only the constraints differ - so this file pins both
// strategies to that shared set. One test method per strategy, so a failure names
// the strategy that broke.
public sealed class FindBeautifulIndicesInTheGivenArrayIITests
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
            FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByBruteForce(
                new FindBeautifulIndicesInTheGivenArrayIISolution.Haystack(s),
                new FindBeautifulIndicesInTheGivenArrayIISolution.PrefixPattern(a),
                new FindBeautifulIndicesInTheGivenArrayIISolution.NearbyPattern(b),
                k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindBeautifulIndicesByZFunction_LeetCodeExamples_ReturnsSortedBeautifulIndices(
        string s, string a, string b, int k, int[] expected) =>
        Assert.Equal(
            expected,
            FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByZFunction(
                new FindBeautifulIndicesInTheGivenArrayIISolution.Haystack(s),
                new FindBeautifulIndicesInTheGivenArrayIISolution.PrefixPattern(a),
                new FindBeautifulIndicesInTheGivenArrayIISolution.NearbyPattern(b),
                k));
}
