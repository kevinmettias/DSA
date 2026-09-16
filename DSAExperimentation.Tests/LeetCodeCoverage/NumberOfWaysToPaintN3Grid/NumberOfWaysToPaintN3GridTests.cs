using DSAExperimentation.LeetCode.NumberOfWaysToPaintN3Grid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToPaintN3Grid;

// Harness only: both strategies live in NumberOfWaysToPaintN3GridSolution and
// are asserted against the same examples - the single-row base case, the small
// row counts with known totals, and the large input whose answer only comes out
// right if every running total is reduced mod 1e9+7.
public sealed partial class NumberOfWaysToPaintN3GridTests
{
    public static TheoryData<int, long> Examples =>
        new()
        {
            { 1, 12 },
            { 2, 54 },
            { 3, 246 },
            { 4, 1_122 },
            { 300, 748_221_310 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByTabulation_LeetCodeExamples_ReturnsColoringCount(int rowCount, long expected) =>
        Assert.Equal(expected, NumberOfWaysToPaintN3GridSolution.CountWaysByTabulation(rowCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByMemoizedRecurrence_LeetCodeExamples_ReturnsColoringCount(int rowCount, long expected) =>
        Assert.Equal(expected, NumberOfWaysToPaintN3GridSolution.CountWaysByMemoizedRecurrence(rowCount));
}
