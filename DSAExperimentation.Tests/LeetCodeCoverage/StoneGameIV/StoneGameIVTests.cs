using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameIV;

// LeetCode 1510. Stone Game IV: AliceWins(n) = there exists a perfect square
// x (1 <= x <= n) that leaves the opponent facing a losing position - the
// same minimax-recurrence shape DivisorGameTests/StoneGameIIITests already
// use, memoized by this repo's own Memoizer<TState,TResult>.
public sealed class StoneGameIVTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(4, true)]
    [InlineData(7, false)]
    public void AliceWins_LeetCodeExamplesAndDeeperRecursion_MatchesExpectedOutcome(int n, bool expected)
        => Assert.Equal(expected, AliceWins(n));

    private static bool AliceWins(int n)
        => Memoizer.Memoize<int, bool>(n, (current, aliceWins) =>
        {
            for (var square = 1; square * square <= current; square++)
            {
                if (!aliceWins(current - square * square))
                {
                    return true;
                }
            }

            return false;
        });
}
