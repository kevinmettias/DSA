using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumTimeToBreakLocksI;

// LeetCode 3376. Minimum Time to Break Locks I: energy factor x starts at 1
// and increases by k after every lock broken; breaking the i-th lock while
// still unbroken takes ceil(strength[i] / x) minutes at the current x.
// Minimize total minutes over every order the n <= 8 locks could be broken
// in - the DP state is exactly "which locks remain," since x is a pure
// function of how many are already broken.
internal static class MinimumTimeToBreakLocksISolution
{
    // The textbook baseline: try every permutation of lock-breaking order by
    // hand via plain recursion, summing ceil(strength[i]/x) as x grows -
    // deliberately without this repo's own DP primitive, the arm the
    // composed strategy below has to justify itself against.
    public static int FindMinimumTimeByPermutationBruteForce(int[] strength, int k)
    {
        var used = new bool[strength.Length];

        return SearchPermutations(strength, k, used, brokenCount: 0, factor: 1);
    }

    private static int SearchPermutations(int[] strength, int k, bool[] used, int brokenCount, int factor)
    {
        if (brokenCount == strength.Length)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var i = 0; i < strength.Length; i++)
        {
            if (used[i])
            {
                continue;
            }

            used[i] = true;
            var minutes = CeilDivide(strength[i], factor)
                + SearchPermutations(strength, k, used, brokenCount + 1, factor + k);
            used[i] = false;

            best = Math.Min(best, minutes);
        }

        return best;
    }

    private static int CeilDivide(int value, int divisor) => (value + divisor - 1) / divisor;

    // This repo's own Memoizer: the DP state is the bitmask of locks already
    // broken (TState : notnull, satisfied for free by int), and the
    // recurrence is exactly Memoizer's Y-combinator shape - try every
    // still-unbroken lock next, recurse on the smaller remaining set.
    public static int FindMinimumTimeByBitmaskMemo(int[] strength, int k)
    {
        var full = (1 << strength.Length) - 1;

        return Memoizer.Memoize<int, int>(0, (mask, minMinutesFrom) =>
        {
            if (mask == full)
            {
                return 0;
            }

            var factor = 1 + k * PopCount(mask);
            var best = int.MaxValue;

            for (var i = 0; i < strength.Length; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    continue;
                }

                var minutes = CeilDivide(strength[i], factor) + minMinutesFrom(mask | (1 << i));
                best = Math.Min(best, minutes);
            }

            return best;
        });
    }

    private static int PopCount(int mask) => System.Numerics.BitOperations.PopCount((uint)mask);
}
