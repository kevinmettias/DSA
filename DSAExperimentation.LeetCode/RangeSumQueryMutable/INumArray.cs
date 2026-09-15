namespace DSAExperimentation.LeetCode.RangeSumQueryMutable;

// LeetCode's own NumArray operations, common to both strategies so a harness can hold either behind
// one type. Scoped to this problem alone - nothing else in the repo answers a mutable range-sum
// query with this exact signature.
internal interface INumArray
{
    void Update(int index, int val);

    int SumRange(int left, int right);
}
