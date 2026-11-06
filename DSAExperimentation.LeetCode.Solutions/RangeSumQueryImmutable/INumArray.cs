namespace DSAExperimentation.LeetCode.Solutions.RangeSumQueryImmutable;

// LeetCode's own NumArray operation, common to every strategy so a harness can replay a call script on any
// of them. Scoped to this problem alone - LC 307's NumArray also updates, which this one never does.
internal interface INumArray
{
    int SumRange(int left, int right);
}
