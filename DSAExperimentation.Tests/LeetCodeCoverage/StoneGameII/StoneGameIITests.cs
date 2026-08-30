using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameII;

// LeetCode 1140. Stone Game II: the player to move from piles[index:] with
// window bound M gets suffixSum[index] minus whatever the opponent can force
// from the state that follows - the same minimax-recurrence shape
// StoneGameTests/PredictTheWinnerTests use, just keyed on (Index, M) instead
// of (Left, Right) and with an extra loop over how many piles (X in
// [1, 2M]) to take this turn, memoized by this repo's own
// Memoizer<TState,TResult>. suffixSum itself is plain precomputed array
// arithmetic, not a repo primitive - the same role a running sum already
// plays in SumOfSubarrayMinimumsTests.
public sealed partial class StoneGameIITests
{
    [Fact]
    public void StoneGameII_LeetCodeExampleOne_ReturnsTen()
        => Assert.Equal(10, MaxAliceStones([2, 7, 9, 4, 4]));

    [Fact]
    public void StoneGameII_LeetCodeExampleTwo_ReturnsOneHundredFour()
        => Assert.Equal(104, MaxAliceStones([1, 2, 3, 4, 5, 100]));

    [Fact]
    public void StoneGameII_SinglePile_TakesTheWholePile()
        => Assert.Equal(7, MaxAliceStones([7]));

    private static int MaxAliceStones(int[] piles)
    {
        var n = piles.Length;
        var suffixSum = new int[n + 1];
        for (var i = n - 1; i >= 0; i--)
        {
            suffixSum[i] = suffixSum[i + 1] + piles[i];
        }

        return Memoizer.Memoize<(int Index, int M), int>((0, 1), Best);

        int Best((int Index, int M) state, Func<(int Index, int M), int> best)
        {
            var (index, m) = state;
            if (index + (2 * m) >= n)
            {
                return suffixSum[index];
            }

            var result = 0;
            for (var x = 1; x <= 2 * m; x++)
            {
                result = Math.Max(result, suffixSum[index] - best((index + x, Math.Max(m, x))));
            }

            return result;
        }
    }
}
