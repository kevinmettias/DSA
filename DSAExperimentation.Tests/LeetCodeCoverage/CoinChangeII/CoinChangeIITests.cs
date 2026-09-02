using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CoinChangeII;

// LeetCode 518. Coin Change II: order-independent combination-count recurrence
// f(index, remaining) = f(index+1, remaining) [skip the coin at index] +
//                        f(index, remaining - coins[index]) [reuse the coin at index]
// via this repo's own Memoizer (CombinationSumIV precedent for the counting shape,
// DistinctSubsequences precedent for the 2-D tuple state) instead of the textbook
// un-memoized exponential recursion. Walking the coin index forward-only (a skipped
// coin is never revisited) is what keeps this a COMBINATION count - CombinationSumIV's
// near-identical recurrence tries every num at every remaining amount instead, which
// is what makes IT an order-sensitive PERMUTATION count for the same style of problem.
public sealed partial class CoinChangeIITests
{
    [Theory]
    [InlineData(5, new[] { 1, 2, 5 }, 4)]
    [InlineData(3, new[] { 2 }, 0)]
    [InlineData(10, new[] { 10 }, 1)]
    public void CountChangeCombinations_LeetCodeExamples_ReturnsCombinationCount(int amount, int[] coins, int expected)
    {
        var actual = CountCombinations(amount, coins);
        Assert.Equal(expected, actual);
    }

    private static int CountCombinations(int amount, int[] coins)
    {
        return Memoizer.Memoize<(int Index, int Remaining), int>((0, amount), WaysFor);

        int WaysFor((int Index, int Remaining) state, Func<(int Index, int Remaining), int> ways)
        {
            var (index, remaining) = state;

            if (remaining == 0)
            {
                return 1;
            }

            if (index == coins.Length)
            {
                return 0;
            }

            var total = ways((index + 1, remaining));
            if (coins[index] <= remaining)
            {
                total += ways((index, remaining - coins[index]));
            }

            return total;
        }
    }
}
