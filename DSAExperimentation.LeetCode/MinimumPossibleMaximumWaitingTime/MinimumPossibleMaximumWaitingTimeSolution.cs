using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumPossibleMaximumWaitingTime;

// LeetCode 4009. Minimum Possible Maximum Waiting Time.
//
// Car i becomes allowed exactly when car i-1 starts, and a dispenser's fuel
// check only ever looks at cumulative demand assigned to it - never at
// timing - so which cars CAN be served is independent of the wait-time
// question entirely. The two objectives (maximize served count, then
// minimize the maximum wait among an assignment that does) are answered by
// the same left-to-right search over dispenser choices, so both live in one
// recurrence rather than two passes.
//
// The key simplification that keeps the state small: let g0/g1 be "how far
// dispenser 0/1's free time currently sits ahead of the next car's allowed
// time" (clamped at 0 - a dispenser already idle is just idle). Assigning
// car i to dispenser d resets g_d to exactly demand[i] (<= 20 by
// constraint) and shrinks the other dispenser's gap by max(0, g_d) before
// clamping - so g0 and g1 never leave [0, 20], regardless of how large the
// actual timestamps get. Combined with consumed0 (<= fuel[0] <= 50) and the
// car index (<= 50), the whole reachable state space is a few hundred
// thousand tuples at the problem's own worst case.
internal static class MinimumPossibleMaximumWaitingTimeSolution
{
    // Plain recursive search over every dispenser choice for every car,
    // recomputed from scratch on each call - no memoization, BCL only. The
    // arm the Memoizer-backed strategy below has to beat; only safe on the
    // modest input sizes the benchmark actually exercises, since revisiting
    // the same (index, consumed0, gap0, gap1) state repeatedly is exactly
    // what memoization exists to avoid.
    public static int MinWaitByRecursiveSearch(int[] demand, int[] fuel)
    {
        var prefix = PrefixDemand(demand);
        var search = new CarAssignmentSearch(demand, fuel, prefix);
        var outcome = search.Replay(new CarAssignmentState(0, 0, 0, 0), search);

        return outcome.Served == 0 ? LeetCodeAnswer.None : outcome.MaxWait;
    }

    // This repo's own Memoizer: the state is exactly the tuple the comment
    // above bounds (CarAssignmentState, a value type so the default
    // TState : notnull comparer's structural equality is already correct),
    // and the recurrence is the same named search below, this time with a
    // cache in front of it - every dispenser still able to take this car is
    // tried, and each state that results is remembered.
    public static int MinWaitByMemoizedSearch(int[] demand, int[] fuel)
    {
        var prefix = PrefixDemand(demand);

        var outcome = Memoizer.Memoize(
            new CarAssignmentState(0, 0, 0, 0),
            new CarAssignmentSearch(demand, fuel, prefix));

        return outcome.Served == 0 ? LeetCodeAnswer.None : outcome.MaxWait;
    }

    // The recurrence itself, shared by both arms above: `cars` is the car timeline -
    // the per-car demand plus the prefix sums over it, which every step reads together -
    // the state carries the car index and both dispensers' standing, `bestFrom` is the
    // recursion to continue through (the Memoizer's cached run, or the search's own
    // uncached self-recursion), and the two early returns are the only exits.
    private static Outcome Evaluate(
        (int[] Demand, int[] Prefix) cars, int[] fuel, CarAssignmentState state,
        IRecurrence<CarAssignmentState, Outcome> bestFrom)
    {
        if (state.Index == cars.Demand.Length)
        {
            return new Outcome(cars.Demand.Length, 0);
        }

        var consumed1 = cars.Prefix[state.Index] - state.Consumed0;
        var remaining0 = fuel[0] - state.Consumed0;
        var remaining1 = fuel[1] - consumed1;
        var need = cars.Demand[state.Index];

        if (remaining0 < need && remaining1 < need)
        {
            return new Outcome(state.Index, 0);
        }

        return BestAssignment(state, need, (remaining0, remaining1), bestFrom);
    }

    // Both dispensers are tried, dispenser 0 first, and each arm is skipped when
    // its remaining fuel falls short of what this car needs.
    private static Outcome BestAssignment(
        CarAssignmentState state, int need, (int Remaining0, int Remaining1) room,
        IRecurrence<CarAssignmentState, Outcome> bestFrom)
    {
        Outcome? best = null;

        if (room.Remaining0 >= need)
        {
            var child = ServeByDispenser0(state, need, bestFrom);
            best = Better(best, child);
        }

        if (room.Remaining1 >= need)
        {
            var child = ServeByDispenser1(state, need, bestFrom);
            best = Better(best, child);
        }

        return best!.Value;
    }

    // Car i goes to dispenser 0: dispenser 0's own gap resets to the car's demand,
    // dispenser 1's shrinks by max(0, gap0) before clamping, and the wait this car
    // incurs is whatever gap 0 had left ahead of it.
    private static Outcome ServeByDispenser0(
        CarAssignmentState state, int need, IRecurrence<CarAssignmentState, Outcome> bestFrom)
    {
        var wait = Math.Max(0, state.Gap0);
        var next = new CarAssignmentState(
            state.Index + 1, state.Consumed0 + need, need, Math.Max(0, state.Gap1 - Math.Max(0, state.Gap0)));
        var child = bestFrom.Replay(next, bestFrom);
        var maxWait = Math.Max(wait, child.MaxWait);

        return child with { MaxWait = maxWait };
    }

    // Lexicographic pick: more served cars wins outright; a tie goes to the
    // smaller maximum wait. Served count can only rise by continuing to
    // serve cars no other branch reaches, so this alone reproduces "maximize
    // served count, then minimize the maximum wait" without a separate pass.
    private static Outcome Better(Outcome? current, Outcome candidate)
    {
        if (current is null)
        {
            return candidate;
        }

        var champion = current.Value;

        return IsBetterOutcome(candidate, champion)
            ? candidate
            : champion;
    }

    // The same lexicographic pick, named: more served cars wins outright, and a
    // tie on served count goes to the smaller maximum wait.
    private static bool IsBetterOutcome(Outcome candidate, Outcome champion)
        => candidate.Served > champion.Served ||
            (candidate.Served == champion.Served && candidate.MaxWait < champion.MaxWait);

    // The same assignment to dispenser 1: its own gap resets to the car's demand
    // and dispenser 0's shrinks by max(0, gap1) instead.
    private static Outcome ServeByDispenser1(
        CarAssignmentState state, int need, IRecurrence<CarAssignmentState, Outcome> bestFrom)
    {
        var wait = Math.Max(0, state.Gap1);
        var next = new CarAssignmentState(
            state.Index + 1, state.Consumed0, Math.Max(0, state.Gap0 - Math.Max(0, state.Gap1)), need);
        var child = bestFrom.Replay(next, bestFrom);
        var maxWait = Math.Max(wait, child.MaxWait);

        return child with { MaxWait = maxWait };
    }

    // Prefix[i] is the total demand of cars 0..i-1 - since every served
    // car's demand is charged to exactly one dispenser, dispenser 1's own
    // consumption is always prefix[i] - consumed0, so it never needs its
    // own slot in the search state.
    private static int[] PrefixDemand(int[] demand)
    {
        var prefix = new int[demand.Length + 1];

        for (var i = 0; i < demand.Length; i++)
        {
            prefix[i + 1] = prefix[i] + demand[i];
        }

        return prefix;
    }

    // The recurrence, as a named type: what one car-and-dispenser state costs, with no
    // cache of its own. Both entry points run this same type - the memoized one behind
    // Memoizer's run, the plain one straight through this type's own self-recursion.
    private sealed class CarAssignmentSearch(int[] demand, int[] fuel, int[] prefix)
        : IRecurrence<CarAssignmentState, Outcome>
    {
        public Outcome Replay(CarAssignmentState state, IRecurrence<CarAssignmentState, Outcome> rest)
            => Evaluate((demand, prefix), fuel, state, rest);
    }

    // Index: the next car to assign. Consumed0: dispenser 0's cumulative
    // demand so far (dispenser 1's follows from Prefix[Index] - Consumed0).
    // Gap0/Gap1: each dispenser's free-time lead over the next car's allowed
    // time, always in [0, max(demand)] per this file's own opening comment.
    private readonly record struct CarAssignmentState(int Index, int Consumed0, int Gap0, int Gap1);

    // Served: how many of the leading cars this path serves. MaxWait: the
    // largest waiting time among them (meaningless, and left at 0, when
    // Served is 0).
    private readonly record struct Outcome(int Served, int MaxWait);
}
