using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DivisorGame;

// LeetCode 1025. Divisor Game: AliceWins(n) = there exists a divisor x of n
// (0 < x < n) that leaves the opponent facing a losing position - natural-looking
// recursion via this repo's own Memoizer, the same shape NimGameTests/
// StoneGameTests already use for their own game-theory recurrences. The recursion
// reduces to the well-known "n is even" closed form, which the benchmark compares
// against.
public sealed class DivisorGameTests
{
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    public void AliceWins_LeetCodeExamples_MatchesExpectedOutcome(int n, bool expected)
        => Assert.Equal(expected, AliceWins(n));

    private static bool AliceWins(int n)
        => Memoizer.Memoize<int, bool>(n, (current, aliceWins) =>
        {
            for (var x = 1; x < current; x++)
            {
                if (current % x == 0 && !aliceWins(current - x))
                {
                    return true;
                }
            }

            return false;
        });
}
