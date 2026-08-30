using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GuessNumberHigherOrLowerII;

// LeetCode 375. Guess Number Higher or Lower II: minimax interval DP over (low, high)
// bounds - for each range, the guesser picks k to minimize the worst-case money the
// adversary can force by revealing the wrong half, and that worst case is itself the
// max of the two sub-range costs. This repo's own Memoizer<TState,TResult> supplies
// the cache, keyed by that pair - the same (Left, Right)-state shape BurstBalloons
// already uses for its own interval DP.
public sealed partial class GuessNumberHigherOrLowerIITests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(10, 16)]
    public void GetMoneyAmount_LeetCodeExamples_ReturnsMinimumGuaranteedMoney(int n, int expected)
        => Assert.Equal(expected, GetMoneyAmount(n));

    private static int GetMoneyAmount(int n)
        => Memoizer.Memoize<(int Low, int High), int>((1, n), WorstCaseCost);

    private static int WorstCaseCost((int Low, int High) range, Func<(int Low, int High), int> costFor)
    {
        var (low, high) = range;
        if (low >= high)
        {
            return 0;
        }

        var best = int.MaxValue;
        for (var guess = low; guess <= high; guess++)
        {
            var worstHalf = Math.Max(costFor((low, guess - 1)), costFor((guess + 1, high)));
            best = Math.Min(best, guess + worstHalf);
        }

        return best;
    }
}
