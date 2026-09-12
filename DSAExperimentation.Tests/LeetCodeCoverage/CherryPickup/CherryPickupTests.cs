using DSAExperimentation.LeetCode.CherryPickup;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CherryPickup;

// Harness only. Both strategies are CherryPickupSolution's - this file just pins
// them to LeetCode's published examples, including the no-round-trip case that
// exercises the Blocked propagation all the way back to the root call.
public sealed class CherryPickupTests
{
    public static TheoryData<int[,], int> Examples =>
        new()
        {
            {
                new[,]
                {
                    { 0, 1, -1 },
                    { 1, 0, -1 },
                    { 1, 1, 1 },
                },
                5
            },
            {
                new[,]
                {
                    { 1, 1, -1 },
                    { 1, -1, 1 },
                    { -1, 1, 1 },
                },
                0
            },
            { new[,] { { 1 } }, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCherriesByUnmemoizedRecursion_LeetCodeExamples_ReturnsMaxCherries(
        int[,] grid, int expected) =>
        Assert.Equal(expected, CherryPickupSolution.MaxCherriesByUnmemoizedRecursion(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCherriesByMemoizedRecursion_LeetCodeExamples_ReturnsMaxCherries(
        int[,] grid, int expected) =>
        Assert.Equal(expected, CherryPickupSolution.MaxCherriesByMemoizedRecursion(grid));
}
