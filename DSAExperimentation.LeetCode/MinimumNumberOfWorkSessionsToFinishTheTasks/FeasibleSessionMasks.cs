using System.Numerics;

namespace DSAExperimentation.LeetCode.MinimumNumberOfWorkSessionsToFinishTheTasks;

// LC 1986's precomputed "which task subsets fit inside one session": one bit per
// task (tasks.Length <= 14), so mask -> does the total duration of the tasks whose
// bits are set stay within sessionTime. Built by sum-over-subsets - a mask's total
// is its lowest set bit's task plus the total of the mask without that bit, so all
// 2^n sums cost one addition each.
//
// It fixes THIS problem's content (a session budget over a specific task list) and
// answers nothing else, so it lives beside the solution rather than in Domain/
// (§17.3). It is also what both strategies' hoisted overload takes (§17.4): the
// benchmark builds it once in [GlobalSetup] so the table is not charged to the
// recurrence being measured, and it is deliberately not a collection type, so it
// can never be confused with the LeetCode-shaped `int[] tasks` overload.
internal sealed class FeasibleSessionMasks(bool[] fitsInOneSession, int fullMask)
{
    // Every task scheduled - the state the recurrence starts from.
    public int FullMask { get; } = fullMask;

    public static FeasibleSessionMasks Build(int[] tasks, int sessionTime)
    {
        var maskCount = 1 << tasks.Length;
        var totals = new int[maskCount];
        var fits = new bool[maskCount];

        for (var mask = 1; mask < maskCount; mask++)
        {
            var lowestBit = mask & -mask;
            var taskIndex = BitOperations.TrailingZeroCount(lowestBit);
            totals[mask] = totals[mask ^ lowestBit] + tasks[taskIndex];
            fits[mask] = totals[mask] <= sessionTime;
        }

        return new FeasibleSessionMasks(fits, maskCount - 1);
    }

    public bool FitsInOneSession(int mask) => fitsInOneSession[mask];
}
