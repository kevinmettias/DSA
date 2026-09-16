using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumIncompatibility;

// LeetCode 1681. Minimum Incompatibility: split nums into exactly `groupCount` subsets
// of equal size, none containing a repeated value, minimizing the total of each
// subset's (max - min); report -1 when no such split exists.
//
// The search is a bitmask recursion over "which elements still need a group". Only
// the set of remaining elements matters, never the order the earlier groups were
// formed in, so each step canonicalizes by always putting the lowest still-
// ungrouped index into the group being built - that collapses the `groupCount`!
// orderings of one grouping down to a single path.
//
// The same remaining-mask is still reachable through more than one grouping, which
// is the whole difference between the two strategies: the baseline re-derives each
// state from scratch every time it is reached, while the composed strategy caches
// it once through this repo's own Memoizer<TState,TResult> - the same memoize-or-
// not pairing StoneGameVSolution runs, keyed on a subset mask instead of a range.
internal static class MinimumIncompatibilitySolution
{
    // Halves int.MaxValue for a "no valid grouping from here" sentinel that still
    // tolerates a real group cost being added on top without overflowing.
    private const int InfeasibleDivisor = 2;
    private const int Infeasible = int.MaxValue / InfeasibleDivisor;

    // The textbook baseline: plain bitmask recursion with no caching, so a
    // remaining-mask reachable through several grouping orders is recomputed once
    // per path that reaches it. Deliberately written without this repo's primitives
    // - it is the arm the memoized strategy has to justify itself against.
    public static int MinimumIncompatibilityByUnmemoizedRecursion(
        int[] nums, int groupCount)
    {
        var plan = new GroupingPlan<HashSet<int>>(
            nums, nums.Length / groupCount, HashSetNovelty.Instance);
        var fullMask = (1 << nums.Length) - 1;
        var best = BestGrouping(plan, fullMask);

        return best >= Infeasible ? LeetCodeAnswer.None : best;
    }

    // Composed: this repo's own Memoizer<TState,TResult> supplies the cache, keyed
    // on the exact remaining-mask the recurrence branches on, so each of the 2^n
    // reachable states is evaluated once however many grouping orders reach it.
    public static int MinimumIncompatibilityByMemoizedRecursion(
        int[] nums, int groupCount)
    {
        var plan = new GroupingPlan<Set<int>>(
            nums, nums.Length / groupCount, SetNovelty.Instance);
        var fullMask = (1 << nums.Length) - 1;

        var best = Memoizer.Memoize(fullMask, new BestGroupingFromMask<Set<int>>(plan));

        return best >= Infeasible ? LeetCodeAnswer.None : best;
    }

    // The memoized recurrence, as a named type: `remaining` is the set of elements still
    // waiting for a group, and the lowest still-ungrouped one always joins the group
    // being built - which is what collapses the `groupCount`! orderings of one grouping
    // into one path - leaving every legal companion subset of the rest to be tried.
    private sealed class BestGroupingFromMask<TSeen>(GroupingPlan<TSeen> plan) : IRecurrence<int, int>
    {
        public int Replay(int remaining, IRecurrence<int, int> rest)
        {
            if (remaining == 0)
            {
                return 0;
            }

            var lowestBit = remaining & -remaining;
            var others = remaining & ~lowestBit;
            var bestForState = Infeasible;

            for (var sub = others; ; sub = (sub - 1) & others)
            {
                bestForState = BestGroupingWithMemoized(rest, remaining, sub | lowestBit, bestForState);

                if (sub == 0)
                {
                    break;
                }
            }

            return bestForState;
        }

        private int BestGroupingWithMemoized(
            IRecurrence<int, int> rest, int remaining, int group, int bestForState)
        {
            if (PopCount(group) != plan.GroupSize || !TryGroupCost(plan, group, out var cost))
            {
                return bestForState;
            }

            var total = cost + rest.Replay(remaining & ~group, rest);

            return Math.Min(bestForState, total);
        }
    }

    private static int BestGrouping<TSeen>(GroupingPlan<TSeen> plan, int remaining)
    {
        if (remaining == 0)
        {
            return 0;
        }

        var lowestBit = remaining & -remaining;
        var others = remaining & ~lowestBit;
        var bestForState = Infeasible;

        for (var sub = others; ; sub = (sub - 1) & others)
        {
            bestForState = BestGroupingWith(plan, remaining, sub | lowestBit, bestForState);

            if (sub == 0)
            {
                break;
            }
        }

        return bestForState;
    }

    // Scores one candidate group - the lowest still-ungrouped element plus a chosen
    // subset of the rest - and recurses on what it leaves behind.
    private static int BestGroupingWith<TSeen>(GroupingPlan<TSeen> plan, int remaining, int group, int bestForState)
    {
        if (PopCount(group) != plan.GroupSize || !TryGroupCost(plan, group, out var cost))
        {
            return bestForState;
        }

        var total = cost + BestGrouping(plan, remaining & ~group);

        return Math.Min(bestForState, total);
    }

    private static int PopCount(int value)
    {
        var count = 0;

        while (value != 0)
        {
            value &= value - 1;
            count++;
        }

        return count;
    }

    // A group is legal only when its values are distinct, so the walk rejects the
    // group the moment the plan's novelty set says a value has come round again.
    private static bool TryGroupCost<TSeen>(GroupingPlan<TSeen> plan, int group, out int cost)
    {
        if (!TryGroupBounds(plan, group, out var min, out var max))
        {
            cost = 0;
            return false;
        }

        cost = max - min;
        return true;
    }

    // The walk over one candidate group's members, on whichever novelty set the plan
    // carries: a repeated value rejects the group on the spot, and the extremes of the
    // values it did see become that group's cost. Both arms run this same walk - only
    // the set behind it differs.
    private static bool TryGroupBounds<TSeen>(GroupingPlan<TSeen> plan, int group, out int min, out int max)
    {
        var seen = plan.Novelty.Fresh();
        min = int.MaxValue;
        max = int.MinValue;

        for (var i = 0; i < plan.Nums.Length; i++)
        {
            if ((group & (1 << i)) == 0)
            {
                continue;
            }

            if (!plan.Novelty.TryAdmit(seen, plan.Nums[i]))
            {
                return false;
            }

            min = Math.Min(min, plan.Nums[i]);
            max = Math.Max(max, plan.Nums[i]);
        }

        return true;
    }

    // Bundles what stays fixed for the whole search - the array, the group size, and
    // the strategy that answers "has this value been seen already" - so the recurrence
    // helpers take a plan and a mask instead of threading all three through every
    // frame. The strategy rather than a set is what keeps the two arms' walks a single
    // method: every candidate group gets a set of its own.
    private readonly record struct GroupingPlan<TSeen>(
        int[] Nums, int GroupSize, INoveltySet<TSeen> Novelty);
}
