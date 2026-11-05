using DSAExperimentation.LeetCode.Solutions.SearchSuggestionsSystem;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchSuggestionsSystem;

// Harness only. Both the catalog rescan and the sort-once/binary-search strategy
// are SearchSuggestionsSystemSolution's; this file just pins them to two examples
// an earlier version of LeetCode's statement published: a search word whose
// suggestions narrow from three to one as it is typed, and the case where no
// product carries the prefix at all and every keystroke reports an empty suggestion
// list. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SearchSuggestionsSystemSolutionTests
{
    public static TheoryData<string[], string, string[][]> Cases =>
        new()
        {
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
                "tatiana",
                [[], [], [], [], [], [], []]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SuggestedProductsByCatalogScan_Cases_ReturnsUpToThreeSmallestPrefixMatchesPerCharacter(
        string[] products, string searchWord, string[][] expected)
    {
        var actual = SearchSuggestionsSystemSolution.SuggestedProductsByCatalogScan(products, searchWord);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SuggestedProductsBySortedPrefixSearch_Cases_ReturnsUpToThreeSmallestPrefixMatchesPerCharacter(
        string[] products, string searchWord, string[][] expected)
    {
        var actual = SearchSuggestionsSystemSolution.SuggestedProductsBySortedPrefixSearch(products, searchWord);

        Assert.Equal(expected, actual);
    }
}
