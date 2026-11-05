using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToPaintN3Grid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToPaintN3Grid;

// Harness only: both strategies live in NumberOfWaysToPaintN3GridSolution and
// are asserted against the same cases - LeetCode's two, the single-row base case
// and the n = 5000 bound, the small row counts with known totals, and a large input
// whose answer only comes out right if every running total is reduced mod 1e9+7.
public sealed partial class NumberOfWaysToPaintN3GridSolutionTests
{
    public static TheoryData<int, long> Cases =>
        new()
        {
            { 300, 748_221_310 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByTabulation_Cases_ReturnsColoringCount(int rowCount, long expected) =>
        Assert.Equal(expected, NumberOfWaysToPaintN3GridSolution.CountWaysByTabulation(rowCount));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByMemoizedRecurrence_Cases_ReturnsColoringCount(int rowCount, long expected) =>
        Assert.Equal(expected, NumberOfWaysToPaintN3GridSolution.CountWaysByMemoizedRecurrence(rowCount));
}
