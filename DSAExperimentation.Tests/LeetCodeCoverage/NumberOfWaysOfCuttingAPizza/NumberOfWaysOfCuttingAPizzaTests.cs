using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysOfCuttingAPizza;

// LeetCode 1444. Number of Ways of Cutting a Pizza: a plain suffix-sum grid (apples[r,c]
// = apple count in the surviving bottom-right rectangle from (r,c) onward, the same
// running-total-array technique RangeSumQuery2DImmutable already uses) turns "does this
// slice have an apple" into an O(1) subtraction, and Memoizer caches the (Row, Col,
// RemainingCuts) state - the same CherryPickup/BurstBalloons shape, closed over every
// horizontal/vertical cut position instead of a fixed set of moves. The induced state
// graph is well-founded for Memoizer's own precondition: RemainingCuts strictly
// decreases on every recursive call, so no state can ever recur on its own path.
public sealed partial class NumberOfWaysOfCuttingAPizzaTests
{
    private const int Modulus = 1_000_000_007;

    [Fact]
    public void Ways_ClassicExampleWithThreePieces_ReturnsThreeWays()
    {
        string[] pizza = ["A..", "AAA", "..."];

        var result = Ways(pizza, k: 3);

        Assert.Equal(3, result);
    }

    [Fact]
    public void Ways_SinglePieceRequiresOnlyTheWholePizzaToHaveAnApple_ReturnsOne()
    {
        string[] pizza = ["A..", "AAA", "..."];

        var result = Ways(pizza, k: 1);

        Assert.Equal(1, result);
    }

    [Fact]
    public void Ways_FewerApplesThanRequestedPieces_ReturnsZero()
    {
        // Only 2 apples total, but 3 non-overlapping pieces would each need >= 1 -
        // impossible by a plain counting argument, independent of arrangement.
        string[] pizza = ["..", "AA"];

        var result = Ways(pizza, k: 3);

        Assert.Equal(0, result);
    }

    private readonly record struct AppleGrid(int[,] Apples, int Rows, int Cols);

    private static int Ways(string[] pizza, int k)
    {
        var grid = new AppleGrid(BuildAppleSuffixSums(pizza), pizza.Length, pizza[0].Length);

        return Memoizer.Memoize<(int Row, int Col, int RemainingCuts), int>(
            (0, 0, k - 1),
            (state, waysFrom) => WaysFrom(grid, state, waysFrom));
    }

    private static int[,] BuildAppleSuffixSums(string[] pizza)
    {
        var rows = pizza.Length;
        var cols = pizza[0].Length;
        var apples = new int[rows + 1, cols + 1];

        for (var row = rows - 1; row >= 0; row--)
        {
            for (var col = cols - 1; col >= 0; col--)
            {
                apples[row, col] = (pizza[row][col] == 'A' ? 1 : 0)
                    + apples[row + 1, col] + apples[row, col + 1] - apples[row + 1, col + 1];
            }
        }

        return apples;
    }

    private static int WaysFrom(
        AppleGrid grid,
        (int Row, int Col, int RemainingCuts) state,
        Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
    {
        var (row, col, remainingCuts) = state;

        if (grid.Apples[row, col] == 0)
        {
            return 0;
        }

        if (remainingCuts == 0)
        {
            return 1;
        }

        var horizontalWays = SumHorizontalCuts(grid, state, waysFrom);
        var verticalWays = SumVerticalCuts(grid, state, waysFrom);

        return (horizontalWays + verticalWays) % Modulus;
    }

    private static int SumHorizontalCuts(
        AppleGrid grid,
        (int Row, int Col, int RemainingCuts) state,
        Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
    {
        var (row, col, remainingCuts) = state;

        return SumCutsAlongAxis(
            grid.Apples[row, col],
            (row + 1, grid.Rows),
            nextRow => grid.Apples[nextRow, col],
            nextRow => waysFrom((nextRow, col, remainingCuts - 1)));
    }

    private static int SumVerticalCuts(
        AppleGrid grid,
        (int Row, int Col, int RemainingCuts) state,
        Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
    {
        var (row, col, remainingCuts) = state;

        return SumCutsAlongAxis(
            grid.Apples[row, col],
            (col + 1, grid.Cols),
            nextCol => grid.Apples[row, nextCol],
            nextCol => waysFrom((row, nextCol, remainingCuts - 1)));
    }

    private static int SumCutsAlongAxis(int baseline, (int Start, int Bound) range, Func<int, int> valueAt, Func<int, int> waysAt)
    {
        var total = 0;

        for (var next = range.Start; next < range.Bound; next++)
        {
            if (baseline - valueAt(next) > 0)
            {
                total = (total + waysAt(next)) % Modulus;
            }
        }

        return total;
    }
}
