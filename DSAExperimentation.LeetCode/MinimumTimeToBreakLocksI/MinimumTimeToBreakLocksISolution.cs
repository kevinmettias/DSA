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
    public static int FindMinimumTimeByPermutationBruteForce(int[] strength, int energyStep)
    {
        var used = new bool[strength.Length];

        return SearchPermutations(strength, energyStep, used, brokenCount: 0);
    }

    // This repo's own Memoizer: the DP state is the bitmask of locks already
    // broken (TState : notnull, satisfied for free by int), and the recurrence
    // is a named type - try every still-unbroken lock next, recurse on the
    // smaller remaining set.
    public static int FindMinimumTimeByBitmaskMemo(int[] strength, int energyStep)
        => Memoizer.Memoize<int, int>(0, new MinimumMinutesFromMask(strength, energyStep));

    private static int SearchPermutations(int[] strength, int energyStep, bool[] used, int brokenCount)
    {
        if (brokenCount == strength.Length)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var i = 0; i < strength.Length; i++)
        {
            var candidate = MinutesBreakingNext(strength, energyStep, (used, brokenCount), i);
            best = Math.Min(best, candidate);
        }

        return best;
    }

    // What it costs to break lock `index` next and then break the rest optimally - or
    // int.MaxValue when that lock is already broken, which the caller's minimum never
    // picks. The lock is marked only for the duration of the recursive call, so the
    // smaller set that call explores can never choose it again.
    //
    // The factor is a pure function of how many locks are already broken - the same
    // 1 + k * brokenCount the memo arm reads off its mask - so it is derived here rather
    // than carried alongside as a redundant argument.
    private static int MinutesBreakingNext(
        int[] strength, int energyStep, (bool[] Used, int BrokenCount) progress, int index)
    {
        if (progress.Used[index])
        {
            return int.MaxValue;
        }

        var factor = 1 + energyStep * progress.BrokenCount;
        progress.Used[index] = true;
        var minutes = CeilDivide(strength[index], factor)
            + SearchPermutations(strength, energyStep, progress.Used, progress.BrokenCount + 1);
        progress.Used[index] = false;

        return minutes;
    }

    private static int CeilDivide(int value, int divisor) => (value + divisor - 1) / divisor;

    private static int PopCount(int mask) => System.Numerics.BitOperations.PopCount((uint)mask);

    // The recurrence, as a named type: the minutes still owed from a mask of broken
    // locks is the best of breaking any one still-unbroken lock next - that lock's own
    // cost at the current energy factor, plus the minutes owed from the mask it leaves.
    private sealed class MinimumMinutesFromMask(int[] strength, int energyStep)
        : IRecurrence<int, int>
    {
        public int Replay(int state, IRecurrence<int, int> rest)
        {
            var allBroken = (1 << strength.Length) - 1;

            if (state == allBroken)
            {
                return 0;
            }

            var factor = 1 + energyStep * PopCount(state);
            var best = int.MaxValue;

            for (var i = 0; i < strength.Length; i++)
            {
                if ((state & (1 << i)) != 0)
                {
                    continue;
                }

                var minutes = CeilDivide(strength[i], factor) + rest.Replay(state | (1 << i), rest);
                best = Math.Min(best, minutes);
            }

            return best;
        }
    }
}
