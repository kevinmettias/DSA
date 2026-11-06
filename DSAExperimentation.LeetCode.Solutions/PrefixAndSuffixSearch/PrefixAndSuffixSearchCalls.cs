namespace DSAExperimentation.LeetCode.Solutions.PrefixAndSuffixSearch;

// LeetCode's one call on a WordFilter, which IWordFilter answers under a whole name and with the query's
// halves in their own role types (ARCHITECTURE 17.13): f is Search, its pref a SearchPrefix and its suff a
// SearchSuffix.
internal static class PrefixAndSuffixSearchCalls
{
    public static int F(IWordFilter filter, string pref, string suff) => filter.Search(new SearchPrefix(pref), new SearchSuffix(suff));
}
