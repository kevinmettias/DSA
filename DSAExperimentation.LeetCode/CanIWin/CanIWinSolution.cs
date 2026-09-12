using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CanIWin;

// LeetCode 464. Can I Win: two players alternately pick from 1..maxChoosableInteger
// (each usable at most once) trying to be the first to reach a running total of at
// least desiredTotal. The first player can force a win iff SOME unpicked number
// either reaches desiredTotal outright or leaves the opponent facing a state where
// CanWin is false - bitmask game-theory recursion over "which numbers have already
// been picked" (one bit per choosable integer).
internal static class CanIWinSolution
{
    // The textbook baseline: the same recursion with no memoization at all,
    // re-exploring every permutation of picks - O(MaxChoosableInteger!) worst case.
    // This is the arm the memoized strategy below has to justify itself against.
    public static bool CanWinByBruteForceRecursion(int maxChoosableInteger, int desiredTotal)
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

        return CanWinFrom(0, desiredTotal);

        bool CanWinFrom(int usedMask, int remainingTotal)
        {
            for (var i = 1; i <= maxChoosableInteger; i++)
            {
                var bit = 1 << (i - 1);
                if ((usedMask & bit) != 0)
                {
                    continue;
                }

                if (i >= remainingTotal || !CanWinFrom(usedMask | bit, remainingTotal - i))
                {
                    return true;
                }
            }

            return false;
        }
    }

    // This repo's own Memoizer, keyed on the used-numbers bitmask - the same shape
    // NimGameTests/ClimbingStairsTests already use, just with an int bitmask instead
    // of a bare integer as the memo state. O(2^MaxChoosableInteger * MaxChoosableInteger)
    // states, each one computed once no matter how many times it's reached by a
    // different pick order.
    public static bool CanWinByMemoizedRecursion(int maxChoosableInteger, int desiredTotal)
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
                var bit = 1 << (i - 1);
                if ((usedMask & bit) != 0)
                {
                    continue;
                }

                var remaining = desiredTotal - SumChosen(usedMask | bit, maxChoosableInteger);
                if (remaining <= 0 || !canWin(usedMask | bit))
                {
                    return true;
                }
            }

            return false;
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
