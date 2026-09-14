using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumIncompatibility;

// LeetCode 1681. Minimum Incompatibility: split nums into exactly k subsets of
// equal size, none containing a repeated value, minimizing the total of each
// subset's (max - min); report -1 when no such split exists.
//
// The search is a bitmask recursion over "which elements still need a group". Only
// the set of remaining elements matters, never the order the earlier groups were
// formed in, so each step canonicalizes by always putting the lowest still-
// ungrouped index into the group being built - that collapses the k! orderings of
// one grouping down to a single path.
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
    public static int MinimumIncompatibilityByUnmemoizedRecursion(int[] nums, int k)
    {
        var plan = new GroupingPlan(nums, nums.Length / k);
        var fullMask = (1 << nums.Length) - 1;
        var best = BestGrouping(plan, fullMask);

        return best >= Infeasible ? LeetCodeAnswer.None : best;
    }

    private static int BestGrouping(GroupingPlan plan, int remaining)
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
    private static int BestGroupingWith(GroupingPlan plan, int remaining, int group, int bestForState)
    {
        if (PopCount(group) != plan.GroupSize || !TryGroupCost(plan.Nums, group, out var cost))
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

    // A group is legal only when its values are distinct, so a BCL HashSet both
    // enforces that rule and stops the walk the moment it is broken.
    private static bool TryGroupCost(int[] nums, int group, out int cost)
    {
        var seen = new HashSet<int>();
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((group & (1 << i)) == 0)
            {
                continue;
            }

            if (!seen.Add(nums[i]))
            {
                cost = 0;
                return false;
            }

            min = Math.Min(min, nums[i]);
            max = Math.Max(max, nums[i]);
        }

        cost = max - min;
        return true;
    }

    // Bundles the two values that stay fixed for the whole search, so the
    // recurrence helpers take a plan and a mask instead of threading the array and
    // the group size through every frame.
    private readonly record struct GroupingPlan(int[] Nums, int GroupSize);

    // Composed: this repo's own Memoizer<TState,TResult> supplies the cache, keyed
    // on the exact remaining-mask the recurrence branches on, so each of the 2^n
    // reachable states is evaluated once however many grouping orders reach it.
    public static int MinimumIncompatibilityByMemoizedRecursion(int[] nums, int k)
    {
        var groupSize = nums.Length / k;
        var fullMask = (1 << nums.Length) - 1;

        var best = Memoizer.Memoize<int, int>(
            fullMask,
            (remaining, rest) => BestGroupingMemoized(new MemoizedGroupingPlan(nums, groupSize, rest), remaining));

        return best >= Infeasible ? LeetCodeAnswer.None : best;
    }

    private static int BestGroupingMemoized(MemoizedGroupingPlan plan, int remaining)
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
            bestForState = BestGroupingWithMemoized(plan, remaining, sub | lowestBit, bestForState);

            if (sub == 0)
            {
                break;
            }
        }

        return bestForState;
    }

    private static int BestGroupingWithMemoized(
        MemoizedGroupingPlan plan, int remaining, int group, int bestForState)
    {
        if (PopCount(group) != plan.GroupSize || !TryGroupCostBySet(plan.Nums, group, out var cost))
        {
            return bestForState;
        }

        var total = cost + plan.Rest(remaining & ~group);

        return Math.Min(bestForState, total);
    }

    // This repo's own Set<int> plays the same "reject a repeated value" role the
    // baseline's HashSet does, on the arm that is composed from this library.
    private static bool TryGroupCostBySet(int[] nums, int group, out int cost)
    {
        var seen = new Set<int>();
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((group & (1 << i)) == 0)
            {
                continue;
            }

            if (!seen.TryAdd(nums[i]))
            {
                cost = 0;
                return false;
            }

            min = Math.Min(min, nums[i]);
            max = Math.Max(max, nums[i]);
        }

        cost = max - min;
        return true;
    }

    // The memoized plan carries the memoized recursive call itself alongside the
    // fixed inputs, which is the one thing the baseline's plan has no use for.
    private readonly record struct MemoizedGroupingPlan(int[] Nums, int GroupSize, Func<int, int> Rest);
}
