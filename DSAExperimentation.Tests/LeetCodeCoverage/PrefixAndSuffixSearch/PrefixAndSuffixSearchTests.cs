using DSAExperimentation.LeetCode.PrefixAndSuffixSearch;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrefixAndSuffixSearch;

// Harness only. Both search strategies are PrefixAndSuffixSearchSolution's - this
// file just pins them to LeetCode's published examples, including the shared-match
// tie-break that always resolves to the largest word index.
public sealed class PrefixAndSuffixSearchTests
{
    public static TheoryData<string[], string, string, int> Examples =>
        new()
        {
            { ["apple"], "a", "e", 0 },
            { ["apple"], "b", "e", -1 },
            { ["apple", "orange", "apricot"], "ap", "t", 2 },
            { ["apple", "orange", "apricot"], "app", "e", 0 },
            { ["apple", "orange", "apricot"], "or", "t", -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByLinearScan_LeetCodeExamples_ReturnsLargestMatchingIndex(
        string[] words, string prefix, string suffix, int expected) =>
        Assert.Equal(
            expected,
            PrefixAndSuffixSearchSolution.SearchByLinearScan(words, new SearchPrefix(prefix), new SearchSuffix(suffix)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByPrecomputedHashMap_LeetCodeExamples_ReturnsLargestMatchingIndex(
        string[] words, string prefix, string suffix, int expected) =>
        Assert.Equal(
            expected,
            PrefixAndSuffixSearchSolution.SearchByPrecomputedHashMap(
                words, new SearchPrefix(prefix), new SearchSuffix(suffix)));
}
