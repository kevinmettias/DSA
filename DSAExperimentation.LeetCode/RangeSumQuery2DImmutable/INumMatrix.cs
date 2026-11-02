namespace DSAExperimentation.LeetCode.RangeSumQuery2DImmutable;

// LeetCode's own NumMatrix operation, common to every strategy so a harness can hold any of them
// behind one type. Scoped to this problem alone - no other problem asks for NumMatrix's operation.
internal interface INumMatrix
{
    int SumRegion(int row1, int col1, int row2, int col2);
}
