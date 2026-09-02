using System.Numerics;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ConcatenatedDivisibility;

// LeetCode 3533. Concatenated Divisibility: order nums so their decimal
// concatenation is divisible by k, and among every order that works, return the
// one that is lexicographically smallest AS A LIST OF INTEGERS (compare 3 vs 12 by
// value, never by the string "3" vs "12"). Return [] if no order works.
//
// Both strategies pick candidates in ascending nums-value order, at every
// position, and take the first one whose remaining choices still admit a
// completion - the standard "build the lexicographically smallest feasible
// sequence" recipe. They differ only in how "still admits a completion" gets
// decided: SmallestPermutationByBacktracking finds out by actually trying every
// remaining order (Backtrack.TrySearch's own use-case, per BacktrackingSteps' own
// doc comment: "find one and quit"); SmallestPermutationByBitmaskMemo instead
// answers it as a bitmask-over-remainder DP (n <= 13 is exactly a "fits in an int
// mask" bound), memoized once via Algorithms.DynamicProgramming.Memoizer so every
// (usedMask, remainder) state is solved at most once for the whole search tree.
internal static class ConcatenatedDivisibilitySolution
{
    // The textbook answer: real depth-first search over permutations, each
    // candidate tried smallest-value-first, backtracking the instant a candidate
    // does not lead to a complete divisible concatenation. No mod-arithmetic
    // shortcut - the concatenation is built and parsed as an actual BigInteger,
    // exactly what "divisible by k" means taken literally.
    public static IList<int> SmallestPermutationByBacktracking(int[] nums, int k)
    {
        var order = SortIndicesByValue(nums);
        var state = new BacktrackState(nums, k, order);

        // Unchoose runs on every candidate's way back out of TrySearch - including
        // the winning one, since backtracking has no other way to know the search
        // is done until OnSolution has already returned - so by the time TrySearch
        // itself returns, state.Path has been unwound back to empty regardless of
        // outcome. The winning order has to be snapshotted from inside OnSolution,
        // the only moment it is both complete and still on the board.
        Backtrack.TrySearch(state, new BacktrackingSteps<BacktrackState, int>(
            IsSolution: s => s.Path.Count == s.Nums.Length,
            Candidates: s => s.Order.Where(i => !s.Used[i]),
            Choose: Choose,
            Unchoose: Unchoose,
            OnSolution: OnSolution));

        return state.Result is null ? [] : [.. state.Result.Select(i => state.Nums[i])];
    }

    private static bool OnSolution(BacktrackState state)
    {
        if (BigInteger.Parse(state.Concatenation) % state.K != 0)
        {
            return false;
        }

        state.Result = [.. state.Path];
        return true;
    }

    private static void Choose(BacktrackState state, int index)
    {
        state.Used[index] = true;
        state.Path.Add(index);
        state.Concatenation += state.Nums[index];
    }

    private static void Unchoose(BacktrackState state, int index)
    {
        state.Used[index] = false;
        state.Path.RemoveAt(state.Path.Count - 1);
        var digits = state.Nums[index].ToString().Length;
        state.Concatenation = state.Concatenation[..^digits];
    }

    // reach(mask, remainder) = "is there an order of the numbers left in mask that
    // carries remainder to 0 mod k" is a bitmask DP; Memoizer turns that
    // feasibility recurrence into a *reconstruction* one for free by returning the
    // winning suffix itself (or null) instead of a bool, trying candidates
    // smallest-first so the first non-null result found at the root is already the
    // answer - no separate greedy replay pass needed.
    public static IList<int> SmallestPermutationByBitmaskMemo(int[] nums, int k)
    {
        var order = SortIndicesByValue(nums);
        var pow10ModK = BuildPow10ModK(nums, k);
        var fullMask = (1 << nums.Length) - 1;

        var suffix = Memoizer.Memoize<(int Mask, int Remainder), List<int>?>(
            (fullMask, 0),
            (state, recurse) => BestSuffix(state, nums, order, pow10ModK, k, recurse));

        return suffix is null ? [] : [.. suffix.Select(i => nums[i])];
    }

    private static List<int>? BestSuffix(
        (int Mask, int Remainder) state,
        int[] nums,
        int[] order,
        int[] pow10ModK,
        int k,
        Func<(int Mask, int Remainder), List<int>?> recurse)
    {
        if (state.Mask == 0)
        {
            return state.Remainder == 0 ? [] : null;
        }

        foreach (var i in order)
        {
            var bit = 1 << i;

            if ((state.Mask & bit) == 0)
            {
                continue;
            }

            var nextRemainder = (state.Remainder * pow10ModK[i] + nums[i]) % k;
            var rest = recurse((state.Mask & ~bit, nextRemainder));

            if (rest is not null)
            {
                return [i, .. rest];
            }
        }

        return null;
    }

    // Ascending by value once, up front: both strategies only ever need to offer
    // candidates in this order to guarantee lexicographically-smallest-by-value
    // output, so the sort is shared rather than repeated per strategy.
    private static int[] SortIndicesByValue(int[] nums)
    {
        var order = Enumerable.Range(0, nums.Length).ToArray();
        Array.Sort(order, (x, y) => nums[x].CompareTo(nums[y]));
        return order;
    }

    // 10^(decimal digit count of nums[i]), mod k - the factor a running remainder
    // must be multiplied by to make room for nums[i]'s own digits when it is
    // appended next.
    private static int[] BuildPow10ModK(int[] nums, int k)
    {
        var pow10ModK = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            var digits = nums[i].ToString().Length;
            var value = 1;

            for (var d = 0; d < digits; d++)
            {
                value = value * 10 % k;
            }

            pow10ModK[i] = value;
        }

        return pow10ModK;
    }

    // The mutable board Backtrack.TrySearch threads Choose/Unchoose through -
    // Order is fixed for the whole search, Used/Path/Concatenation are what those
    // two steps mutate and restore.
    private sealed class BacktrackState(int[] nums, int k, int[] order)
    {
        public int[] Nums { get; } = nums;
        public int K { get; } = k;
        public int[] Order { get; } = order;
        public bool[] Used { get; } = new bool[nums.Length];
        public List<int> Path { get; } = [];
        public string Concatenation { get; set; } = string.Empty;
        public List<int>? Result { get; set; }
    }
}
