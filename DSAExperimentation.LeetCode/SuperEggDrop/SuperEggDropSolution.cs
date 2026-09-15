using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.SuperEggDrop;

// LeetCode 887. Super Egg Drop: minimize the worst-case number of trial drops
// needed to find the critical floor with a fixed number of eggs. Both strategies
// walk the same minimax recurrence over an (eggs, floors) state - drop from
// `trial`, and the adversary picks whichever of "breaks" (eggs - 1, trial - 1) or
// "survives" (eggs, floors - trial) costs more.
//
// They differ only in how the next trial floor is chosen: an exhaustive scan of
// every candidate floor - O(eggs * floors^2) - or a binary search for the floor
// where the two branches cross, since WorstCaseOverTrials(trial) is monotonic in trial
// (raising the trial floor can only help the "breaks" branch and hurt the
// "survives" one) - O(eggs * floors * log(floors)).
internal static class SuperEggDropSolution
{

    // The textbook answer: the same minimax recurrence, every candidate trial floor
    // scanned, cached in a plain BCL dictionary. Deliberately written without this
    // repo's primitives - it is the arm the binary-searching strategy below has to
    // justify itself against.
    public static int MinMovesByLinearScan(int eggs, int floors) =>
        LinearScanMoves(eggs, floors, new Dictionary<(int Eggs, int Floors), int>());

    // This repo's own Memoizer<TState,TResult> supplies the (eggs, floors) cache -
    // the same 2-tuple-state shape GuessNumberHigherOrLowerII already uses for its
    // own minimax DP - and each state bisects for its trial floor instead of
    // scanning every one of them.
    public static int MinMovesByBinarySearch(int eggs, int floors) =>
        Memoizer.Memoize<(int Eggs, int Floors), int>((eggs, floors), new WorstCaseOverTrials());

    // The recurrence, as a named type: the minimax worst-case move count for one
    // (eggs, floors) state, with the memoized continuation arriving as `rest` rather
    // than as an anonymous delegate.
    private sealed class WorstCaseOverTrials : IRecurrence<(int Eggs, int Floors), int>
    {
        public int Replay(
            (int Eggs, int Floors) state, IRecurrence<(int Eggs, int Floors), int> rest)
        {
            var (eggs, floors) = state;

            if (floors == 0)
            {
                return 0;
            }

            if (eggs == 1)
            {
                return floors;
            }

            var range = new TrialRange(1, floors);
            var best = int.MaxValue;

            while (range.Low <= range.High)
            {
                (range, best) = NarrowTrialRange(range, best, state, rest);
            }

            return best;
        }
    }

    // One bisection probe: score the window's midpoint as the next drop floor, then
    // narrow toward whichever side - breaks vs. survives - currently costs more.
    private static (TrialRange Range, int Best) NarrowTrialRange(
        TrialRange range,
        int best,
        (int Eggs, int Floors) state,
        IRecurrence<(int Eggs, int Floors), int> rest)
    {
        var (eggs, floors) = state;
        var trial = (range.Low + range.High) / AlgorithmConstants.HalvingFactor;
        var breaks = rest.Replay((eggs - 1, trial - 1), rest);
        var survives = rest.Replay((eggs, floors - trial), rest);
        best = Math.Min(best, 1 + Math.Max(breaks, survives));

        return breaks < survives
            ? WindowAboveTrial(range, best, trial)
            : WindowBelowTrial(range, best, trial);
    }

    private static (TrialRange Range, int Best) WindowAboveTrial(TrialRange range, int best, int trial) =>
        (range with { Low = trial + 1 }, best);

    private static (TrialRange Range, int Best) WindowBelowTrial(TrialRange range, int best, int trial) =>
        (range with { High = trial - 1 }, best);

    private static int LinearScanMoves(int eggs, int floors, Dictionary<(int Eggs, int Floors), int> cache)
    {
        if (floors == 0)
        {
            return 0;
        }

        if (eggs == 1)
        {
            return floors;
        }

        if (cache.TryGetValue((eggs, floors), out var cached))
        {
            return cached;
        }

        var best = BestTrialFloorScan(eggs, floors, cache);

        cache[(eggs, floors)] = best;
        return best;
    }

    // The exhaustive arm's one step: score every candidate trial floor and keep the
    // cheapest worst case.
    private static int BestTrialFloorScan(int eggs, int floors, Dictionary<(int Eggs, int Floors), int> cache)
    {
        var best = int.MaxValue;

        for (var trial = 1; trial <= floors; trial++)
        {
            var breaks = LinearScanMoves(eggs - 1, trial - 1, cache);
            var survives = LinearScanMoves(eggs, floors - trial, cache);
            best = Math.Min(best, 1 + Math.Max(breaks, survives));
        }

        return best;
    }

    // The [low, high] trial-floor window WorstCaseOverTrials bisects each step - bundled
    // so NarrowTrialRange stays within the 4 value-parameter limit.
    private readonly record struct TrialRange(int Low, int High);
}
