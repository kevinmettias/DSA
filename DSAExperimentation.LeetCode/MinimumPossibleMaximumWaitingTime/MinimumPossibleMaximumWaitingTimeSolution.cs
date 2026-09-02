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
        var outcome = SearchAssignments(demand, fuel, prefix, index: 0, consumed0: 0, gap0: 0, gap1: 0);

        return outcome.Served == 0 ? LeetCodeAnswer.None : outcome.MaxWait;
    }

    private static Outcome SearchAssignments(
        int[] demand, int[] fuel, int[] prefix, int index, int consumed0, int gap0, int gap1)
    {
        if (index == demand.Length)
        {
            return new Outcome(demand.Length, 0);
        }

        var consumed1 = prefix[index] - consumed0;
        var remaining0 = fuel[0] - consumed0;
        var remaining1 = fuel[1] - consumed1;
        var need = demand[index];

        if (remaining0 < need && remaining1 < need)
        {
            return new Outcome(index, 0);
        }

        Outcome? best = null;

        if (remaining0 >= need)
        {
            var wait = Math.Max(0, gap0);
            var child = SearchAssignments(
                demand, fuel, prefix, index + 1, consumed0 + need, need, Math.Max(0, gap1 - Math.Max(0, gap0)));
            best = Better(best, child with { MaxWait = Math.Max(wait, child.MaxWait) });
        }

        if (remaining1 >= need)
        {
            var wait = Math.Max(0, gap1);
            var child = SearchAssignments(
                demand, fuel, prefix, index + 1, consumed0, Math.Max(0, gap0 - Math.Max(0, gap1)), need);
            best = Better(best, child with { MaxWait = Math.Max(wait, child.MaxWait) });
        }

        return best!.Value;
    }

    // This repo's own Memoizer: the state is exactly the tuple the comment
    // above bounds (CarAssignmentState, a value type so the default
    // TState : notnull comparer's structural equality is already correct),
    // and the recurrence is Memoizer's usual Y-combinator shape - try every
    // dispenser still able to take this car, recurse on the state that
    // results.
    public static int MinWaitByMemoizedSearch(int[] demand, int[] fuel)
    {
        var prefix = PrefixDemand(demand);

        var outcome = Memoizer.Memoize<CarAssignmentState, Outcome>(
            new CarAssignmentState(0, 0, 0, 0),
            (state, bestFrom) => Evaluate(demand, fuel, prefix, state, bestFrom));

        return outcome.Served == 0 ? LeetCodeAnswer.None : outcome.MaxWait;
    }

    private static Outcome Evaluate(
        int[] demand, int[] fuel, int[] prefix, CarAssignmentState state, Func<CarAssignmentState, Outcome> bestFrom)
    {
        if (state.Index == demand.Length)
        {
            return new Outcome(demand.Length, 0);
        }

        var consumed1 = prefix[state.Index] - state.Consumed0;
        var remaining0 = fuel[0] - state.Consumed0;
        var remaining1 = fuel[1] - consumed1;
        var need = demand[state.Index];

        if (remaining0 < need && remaining1 < need)
        {
            return new Outcome(state.Index, 0);
        }

        Outcome? best = null;

        if (remaining0 >= need)
        {
            var wait = Math.Max(0, state.Gap0);
            var next = new CarAssignmentState(
                state.Index + 1, state.Consumed0 + need, need, Math.Max(0, state.Gap1 - Math.Max(0, state.Gap0)));
            var child = bestFrom(next);
            best = Better(best, child with { MaxWait = Math.Max(wait, child.MaxWait) });
        }

        if (remaining1 >= need)
        {
            var wait = Math.Max(0, state.Gap1);
            var next = new CarAssignmentState(
                state.Index + 1, state.Consumed0, Math.Max(0, state.Gap0 - Math.Max(0, state.Gap1)), need);
            var child = bestFrom(next);
            best = Better(best, child with { MaxWait = Math.Max(wait, child.MaxWait) });
        }

        return best!.Value;
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

        return candidate.Served > champion.Served ||
            (candidate.Served == champion.Served && candidate.MaxWait < champion.MaxWait)
                ? candidate
                : champion;
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
