using DSAExperimentation.LeetCode.NumberOfWaysToPaintN3Grid;

namespace DSAExperimentation.LeetCode.Tests.NumberOfWaysToPaintN3Grid;

// Harness only: both strategies live in NumberOfWaysToPaintN3GridSolution and
// are asserted against the same examples - LeetCode's two, the single-row base case
// and the n = 5000 bound, the small row counts with known totals, and a large input
// whose answer only comes out right if every running total is reduced mod 1e9+7.
public sealed partial class NumberOfWaysToPaintN3GridSolutionTests
{
    public static TheoryData<int, long> Examples =>
        new()
        {
            // LeetCode examples 1 and 2.
            { 1, 12 },
            { 5000, 30_228_214 },

            // A row is one of 6 two-colour (ABA) or 6 three-colour (ABC) patterns; below
            // an ABA row fit 3 ABA and 2 ABC rows, below an ABC row 2 of each. From 6 and
            // 6, the (ABA, ABC) counts run (30, 24), (138, 108), (630, 492): 54, 246 and
            // 1122 ways for two to four rows.
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
