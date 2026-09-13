using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchSuggestionsSystem;

// LeetCode 1268. Search Suggestions System: after each character typed of
// searchWord, report the three lexicographically smallest products that carry the
// typed prefix.
//
// Both strategies answer the same question and differ only in how they find the
// prefix's matches: rescan the whole catalog per keystroke, keeping the smallest
// three seen, or sort the catalog once and then let a binary search jump straight
// to where each growing prefix begins.
internal static class SearchSuggestionsSystemSolution
{
    // LC 1268 asks for at most three suggestions per keystroke.
    private const int MaxSuggestions = 3;

    // The textbook baseline: for every prefix, walk every product and keep an
    // insertion-sorted top three. Deliberately plain BCL - no sort, no search
    // primitive - since it is the arm the composed strategy below has to beat.
    public static List<string[]> SuggestedProductsByCatalogScan(string[] products, string searchWord)
    {
        var result = new List<string[]>(searchWord.Length);
        var prefix = string.Empty;

        foreach (var character in searchWord)
        {
            prefix += character;
            result.Add(SmallestThreeMatches(products, prefix));
        }

        return result;
    }

    private static string[] SmallestThreeMatches(string[] products, string prefix)
    {
        var top = new List<string>(MaxSuggestions);

        foreach (var product in products)
        {
            if (product.StartsWith(prefix, StringComparison.Ordinal))
            {
                InsertIfAmongSmallestThree(top, product);
            }
        }

        return [.. top];
    }

    private static void InsertIfAmongSmallestThree(List<string> top, string candidate)
    {
        var insertAt = top.Count;

        while (insertAt > 0 && string.CompareOrdinal(top[insertAt - 1], candidate) > 0)
        {
            insertAt--;
        }

        if (insertAt >= MaxSuggestions)
        {
            return;
        }

        top.Insert(insertAt, candidate);

        if (top.Count > MaxSuggestions)
        {
            top.RemoveAt(MaxSuggestions);
        }
    }

    // This repo's own two primitives chained: MergeSort.Sort orders the catalog
    // once, and BinarySearch.LowerBound then locates where each growing prefix
    // would sit in that order - a short forward scan from there collects the
    // matches, stopping the instant a candidate no longer carries the prefix.
    // The same composition FindFirstAndLastPositionOfElementInSortedArray and
    // SortAnArray use individually.
    public static List<string[]> SuggestedProductsBySortedPrefixSearch(string[] products, string searchWord)
    {
        var sorted = (string[])products.Clone();
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(
            new ArrayIndexedSequence<string>(sorted), StringComparer.Ordinal);

        var sequence = new ArraySequence<string>(sorted);
        var result = new List<string[]>(searchWord.Length);
        var prefix = string.Empty;

        foreach (var character in searchWord)
        {
            prefix += character;
            result.Add(MatchesFromLowerBound(sorted, sequence, prefix));
        }

        return result;
    }

    private static string[] MatchesFromLowerBound(string[] sorted, ArraySequence<string> sequence, string prefix)
    {
        var start = BinarySearch.LowerBound(sequence, prefix, StringComparer.Ordinal);
        var matches = new List<string>(MaxSuggestions);

        for (var i = start; i < sorted.Length && matches.Count < MaxSuggestions; i++)
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
