using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NimGame;

// LeetCode 292. Nim Game: canWin(n) = there exists a move of 1-3 stones that leaves
// the opponent facing a losing position - natural-looking recursion via this repo's
// Memoizer, no hand-rolled cache, the same shape ClimbingStairsTests.cs/
// HouseRobberTests.cs already use for their own recurrences. The recursion reduces
// to the well-known n % 4 != 0 closed form, which the benchmark compares against.
public sealed class NimGameTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(7, true)]
    [InlineData(8, false)]
    public void CanWinNim_LeetCodeExamples_MatchesExpectedOutcome(int n, bool expected)
        => Assert.Equal(expected, CanWinNim(n));

    private static bool CanWinNim(int n)
        => Memoizer.Memoize<int, bool>(n, (stones, canWin) => stones switch
        {
            <= 0 => false,
            _ => !canWin(stones - 1)
                || (stones >= 2 && !canWin(stones - 2))
                || (stones >= 3 && !canWin(stones - 3)),
        });
}
