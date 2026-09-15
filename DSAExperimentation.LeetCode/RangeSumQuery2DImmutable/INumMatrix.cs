namespace DSAExperimentation.LeetCode.RangeSumQuery2DImmutable;

// LeetCode's own NumMatrix operation, common to both strategies so a harness can hold either
// behind one type. Scoped to this problem alone - nothing else in the repo answers a 2D range-sum
// query.
internal interface INumMatrix
{
    int SumRegion(int row1, int col1, int row2, int col2);
}
