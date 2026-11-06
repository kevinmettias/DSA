namespace DSAExperimentation.LeetCode.Solutions.PrefixAndSuffixSearch;

// LeetCode's own WordFilter operation, common to every strategy so a harness can replay a call script on
// any of them. LeetCode's f is Search here, taking each half of the query in its own role type;
// PrefixAndSuffixSearchCalls maps the one onto the other. Scoped to this problem alone - no other problem
// asks for f's largest matching index.
internal interface IWordFilter
{
    int Search(SearchPrefix prefix, SearchSuffix suffix);
}
