using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CherryPickup;

// LeetCode 741. Cherry Pickup: the round trip (0,0)->(n-1,n-1)->(0,0) reframes as two
// people walking (0,0)->(n-1,n-1) simultaneously, right/down only - a forward walk and
// its reverse cover the same cells either way. Memoizer caches the (Row1, Col1, Col2)
// state (Row2 is derived as Row1+Col1-Col2, since both paths always take the same
// number of steps) - the same grid recurrence UniquePaths/
// LongestIncreasingPathInAMatrix already use, closed over four simultaneous move
// combinations instead of one. The induced state graph is well-founded for Memoizer's
// own precondition: Row1+Col1 strictly increases on every recursive call, so no state
// can ever recur on its own path.
public sealed partial class CherryPickupTests
{
    private const int Blocked = int.MinValue;

    [Fact]
    public void CherryPickup_ClassicExampleWithAReachablePath_ReturnsMaxCherries()
    {
        int[,] grid =
        {
            { 0, 1, -1 },
            { 1, 0, -1 },
            { 1, 1, 1 },
        };

        Assert.Equal(5, CherryPickup(grid));
    }

    [Fact]
    public void CherryPickup_NoRoundTripExists_ReturnsZero()
    {
        int[,] grid =
        {
            { 1, 1, -1 },
            { 1, -1, 1 },
            { -1, 1, 1 },
        };

        Assert.Equal(0, CherryPickup(grid));
    }

    [Fact]
    public void CherryPickup_SingleCell_ReturnsItsOwnValue()
    {
        int[,] grid = { { 1 } };

        Assert.Equal(1, CherryPickup(grid));
    }

    private static int CherryPickup(int[,] grid)
    {
        var n = grid.GetLength(0);

        var result = Memoizer.Memoize<(int Row1, int Col1, int Col2), int>((0, 0, 0), CherriesFrom);
        return Math.Max(0, result);

        int CherriesFrom(
            (int Row1, int Col1, int Col2) state,
            Func<(int Row1, int Col1, int Col2), int> cherriesFrom)
        {
            var (row1, col1, col2) = state;
            var row2 = row1 + col1 - col2;

            if (row1 >= n || col1 >= n || row2 < 0 || row2 >= n || col2 < 0 || col2 >= n
                || grid[row1, col1] == -1 || grid[row2, col2] == -1)
            {
                return Blocked;
            }

            if (row1 == n - 1 && col1 == n - 1)
            {
                return grid[row1, col1];
            }

            var picked = grid[row1, col1] + (col1 == col2 ? 0 : grid[row2, col2]);

            var bestNext = Math.Max(
                Math.Max(cherriesFrom((row1 + 1, col1, col2 + 1)), cherriesFrom((row1 + 1, col1, col2))),
                Math.Max(cherriesFrom((row1, col1 + 1, col2 + 1)), cherriesFrom((row1, col1 + 1, col2))));

            return bestNext == Blocked ? Blocked : picked + bestNext;
        }
    }
}
