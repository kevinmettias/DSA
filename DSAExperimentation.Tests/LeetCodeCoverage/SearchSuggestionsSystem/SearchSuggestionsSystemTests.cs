using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchSuggestionsSystem;

// LeetCode 1268. Search Suggestions System: this repo's own
// Algorithms.Sorting.MergeSort.Sort<string, ArrayIndexedSequence<string>> sorts
// products once, then Algorithms.Searching.BinarySearch.LowerBound<string,
// ArraySequence<string>> locates where each growing prefix would sit in that
// sorted order per typed character - a linear scan from there (capped at 3, and
// stopping the instant a candidate no longer has the prefix) collects the
// lexicographically-smallest matches. Same two-primitive composition
// FindFirstAndLastPositionOfElementInSortedArrayTests/SortAnArrayTests already use
// individually, here chained together.
public sealed class SearchSuggestionsSystemTests
{
    [Fact]
    public void SuggestedProducts_ClassicExample_ReturnsUpToThreeSmallestPrefixMatchesPerCharacter()
    {
        string[] products = ["mobile", "mouse", "moneypot", "monitor", "mousepad"];

        var suggestions = SuggestedProducts(products, "mouse");

        Assert.Equal(
            [
                new[] { "mobile", "moneypot", "monitor" },
                new[] { "mobile", "moneypot", "monitor" },
                new[] { "mouse", "mousepad" },
                new[] { "mouse", "mousepad" },
                new[] { "mouse", "mousepad" },
            ],
            suggestions);
    }

    [Fact]
    public void SuggestedProducts_PrefixNarrowsOutEarlierMatches_DropsThemFromLaterSteps()
    {
        string[] products = ["bags", "baggage", "banner", "box", "cloths"];

        var suggestions = SuggestedProducts(products, "bags");

        Assert.Equal(
            [
                new[] { "baggage", "bags", "banner" },
                new[] { "baggage", "bags", "banner" },
                new[] { "baggage", "bags" },
                new[] { "bags" },
            ],
            suggestions);
    }

    [Fact]
    public void SuggestedProducts_SingleProductMatchingEveryPrefix_RepeatsItForEveryCharacter()
    {
        string[] products = ["havana"];

        var suggestions = SuggestedProducts(products, "havana");

        Assert.Equal(Enumerable.Repeat(new[] { "havana" }, 6), suggestions);
    }

    private static List<string[]> SuggestedProducts(string[] products, string searchWord)
    {
        var sorted = (string[])products.Clone();
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(sorted), StringComparer.Ordinal);

        var sequence = new ArraySequence<string>(sorted);
        var result = new List<string[]>();
        var prefix = string.Empty;

        foreach (var ch in searchWord)
        {
            prefix += ch;
            result.Add(MatchesForPrefix(sorted, sequence, prefix));
        }

        return result;
    }

    private static string[] MatchesForPrefix(string[] sorted, ArraySequence<string> sequence, string prefix)
    {
        var start = BinarySearch.LowerBound(sequence, prefix, StringComparer.Ordinal);
        var matches = new List<string>();

        for (var i = start; i < sorted.Length && matches.Count < 3; i++)
        {
            if (!sorted[i].StartsWith(prefix, StringComparison.Ordinal))
            {
                break;
            }

            matches.Add(sorted[i]);
        }

        return [.. matches];
    }
}
