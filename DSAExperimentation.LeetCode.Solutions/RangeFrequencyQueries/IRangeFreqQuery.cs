namespace DSAExperimentation.LeetCode.Solutions.RangeFrequencyQueries;

// LeetCode's own RangeFreqQuery operation, common to every strategy so a harness can replay a call script on
// any of them. Scoped to this problem alone - no other problem asks for query's count of one value in a
// window.
internal interface IRangeFreqQuery
{
    int Query(int left, int right, int value);
}
