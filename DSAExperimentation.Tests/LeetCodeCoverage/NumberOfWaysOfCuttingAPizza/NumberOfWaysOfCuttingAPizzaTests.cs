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

    private static int Ways(string[] pizza, int k)
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

        return Memoizer.Memoize<(int Row, int Col, int RemainingCuts), int>((0, 0, k - 1), WaysFrom);

        int WaysFrom(
            (int Row, int Col, int RemainingCuts) state,
            Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
        {
            var (row, col, remainingCuts) = state;

            if (apples[row, col] == 0)
            {
                return 0;
            }

            if (remainingCuts == 0)
            {
                return 1;
            }

            var total = 0;

            for (var nextRow = row + 1; nextRow < rows; nextRow++)
            {
                if (apples[row, col] - apples[nextRow, col] > 0)
                {
                    total = (total + waysFrom((nextRow, col, remainingCuts - 1))) % Modulus;
                }
            }

            for (var nextCol = col + 1; nextCol < cols; nextCol++)
            {
                if (apples[row, col] - apples[row, nextCol] > 0)
                {
                    total = (total + waysFrom((row, nextCol, remainingCuts - 1))) % Modulus;
                }
            }

            return total;
        }
    }
}
