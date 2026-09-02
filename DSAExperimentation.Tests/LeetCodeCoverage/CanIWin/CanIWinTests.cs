using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CanIWin;

// LeetCode 464. Can I Win: bitmask game-theory recursion over "which numbers have
// already been picked" via this repo's own Memoizer - the same shape
// NimGameTests.cs/ClimbingStairsTests.cs already use, just with an int bitmask (one
// bit per choosable integer) as the memo state instead of a bare integer. The first
// player can force a win iff SOME unpicked number either reaches desiredTotal outright
// or leaves the opponent facing a state where canWin is false.
public sealed class CanIWinTests
{
    [Theory]
    [InlineData(10, 11, false)]
    [InlineData(10, 0, true)]
    [InlineData(10, 1, true)]
    [InlineData(10, 40, false)]
    [InlineData(4, 11, false)]
    [InlineData(4, 6, true)]
    public void CanIWin_LeetCodeExamples_MatchesExpectedOutcome(int maxChoosableInteger, int desiredTotal, bool expected)
    {
        var canWin = CanIWin(maxChoosableInteger, desiredTotal);
        Assert.Equal(expected, canWin);
    }

    private static bool CanIWin(int maxChoosableInteger, int desiredTotal)
    {
        if (desiredTotal <= 0)
        {
            return true;
        }

        var maxSum = maxChoosableInteger * (maxChoosableInteger + 1) / 2;
        if (maxSum < desiredTotal)
        {
            return false;
        }

        return Memoizer.Memoize<int, bool>(0, (usedMask, canWin) =>
        {
            for (var i = 1; i <= maxChoosableInteger; i++)
            {
                if (CanWinByPicking(i, usedMask, canWin))
                {
                    return true;
                }
            }

            return false;

            bool CanWinByPicking(int pick, int mask, Func<int, bool> canWinFromNext)
            {
                var bit = 1 << (pick - 1);
                if ((mask & bit) != 0)
                {
                    return false;
                }

                var remaining = desiredTotal - SumChosen(mask | bit, maxChoosableInteger);
                return remaining <= 0 || !canWinFromNext(mask | bit);
            }
        });
    }

    private static int SumChosen(int mask, int maxChoosableInteger)
    {
        var sum = 0;
        for (var i = 1; i <= maxChoosableInteger; i++)
        {
            if ((mask & (1 << (i - 1))) != 0)
            {
                sum += i;
            }
        }

        return sum;
    }
}
