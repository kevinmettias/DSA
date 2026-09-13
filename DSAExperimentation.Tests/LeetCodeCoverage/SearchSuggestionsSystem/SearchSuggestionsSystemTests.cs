using DSAExperimentation.LeetCode.SearchSuggestionsSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchSuggestionsSystem;

// Harness only. Both the catalog rescan and the sort-once/binary-search strategy
// are SearchSuggestionsSystemSolution's; this file just pins them to LeetCode's
// published examples, including the case where no product carries the prefix at
// all and every keystroke reports an empty suggestion list.
public sealed class SearchSuggestionsSystemTests
{
    public static TheoryData<string[], string, string[][]> Examples =>
        new()
        {
            {
                ["mobile", "mouse", "moneypot", "monitor", "mousepad"],
                "mouse",
                [
                    ["mobile", "moneypot", "monitor"],
                    ["mobile", "moneypot", "monitor"],
                    ["mouse", "mousepad"],
                    ["mouse", "mousepad"],
                    ["mouse", "mousepad"],
                ]
            },
            {
                ["bags", "baggage", "banner", "box", "cloths"],
                "bags",
                [
                    ["baggage", "bags", "banner"],
                    ["baggage", "bags", "banner"],
                    ["baggage", "bags"],
                    ["bags"],
                ]
            },
            {
                ["havana"],
                "havana",
                [["havana"], ["havana"], ["havana"], ["havana"], ["havana"], ["havana"]]
            },
            {
                ["havana"],
                "tatiana",
                [[], [], [], [], [], [], []]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SuggestedProductsByCatalogScan_LeetCodeExamples_ReturnsUpToThreeSmallestPrefixMatchesPerCharacter(
        string[] products, string searchWord, string[][] expected) =>
        Assert.Equal(expected, SearchSuggestionsSystemSolution.SuggestedProductsByCatalogScan(products, searchWord));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SuggestedProductsBySortedPrefixSearch_LeetCodeExamples_ReturnsUpToThreeSmallestPrefixMatchesPerCharacter(
        string[] products, string searchWord, string[][] expected) =>
        Assert.Equal(expected, SearchSuggestionsSystemSolution.SuggestedProductsBySortedPrefixSearch(products, searchWord));
}
