using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximumNumberOfOperationsWithTheSameScoreII;

// LeetCode 3040. Maximum Number of Operations With the Same Score II: each
// operation deletes the first two remaining elements, the last two, or the
// first and last, scoring their sum - every operation across the whole run must
// score the same. Only the FIRST operation actually chooses that score, and it
// is one of at most three values (front pair, back pair, front-and-back), so
// trying each as a fixed target and searching the remaining [left, right]
// window bounds the whole problem to three O(n^2) interval searches.
//
// Both strategies share the same three-move recurrence over a (left, right)
// window - advance past a front pair, retreat past a back pair, or close in
// from both ends - differing only in how each window's memo table is kept.
internal static class MaximumNumberOfOperationsWithTheSameScoreIISolution
{
    // The textbook form: recursion over hand-rolled Dictionary memo tables, one
    // fresh table per candidate target. The arm the repo's own Memoizer-based
    // strategy below has to beat.
    public static int MaxOperationsByBruteForceDp(int[] nums)
    {
        var n = nums.Length;

        if (n < 2)
        {
            return 0;
        }

        var best = 1;
        best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, (2, n - 1), nums[0] + nums[1]));
        best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, (0, n - 3), nums[n - 1] + nums[n - 2]));
        best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, (1, n - 2), nums[0] + nums[n - 1]));
        return best;
    }

    private static int WindowOperationsByDictionaryMemo(int[] nums, (int Left, int Right) state, int target) =>
        WindowOperationsByDictionaryMemo(
            nums, state, target, new Dictionary<(int Left, int Right), int>());

    private static int WindowOperationsByDictionaryMemo(
        int[] nums, (int Left, int Right) state, int target, Dictionary<(int Left, int Right), int> memo)
    {
        var (left, right) = state;

        if (left >= right)
        {
            return 0;
        }

        if (memo.TryGetValue(state, out var cached))
        {
            return cached;
        }

        var best = BestMoveByDictionaryMemo(nums, state, target, memo);
        memo[state] = best;

        return best;
    }

    // One point for each move LegalMoves allows off this window, plus the best score the
    // memo table this arm keeps by hand reports for the window that move leaves behind.
    // Split out of WindowOperationsByDictionaryMemo so the memo bookkeeping above reads
    // as one thing.
    private static int BestMoveByDictionaryMemo(
        int[] nums, (int Left, int Right) state, int target, Dictionary<(int Left, int Right), int> memo)
    {
        var best = 0;

        foreach (var next in LegalMoves(nums, state, target))
        {
            best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, next, target, memo));
        }

        return best;
    }

    // This repo's own Memoizer: the state is just the (left, right) window
    // (nums and target are closed over), and the three moves are the identical
    // recurrence WindowOperationsByDictionaryMemo hand-rolls above - the same
    // "tuple state through Memoizer.Memoize<TState, TResult>" composition
    // MinimumNumberOfOperationsToMakeXAndYEqualSolution.MinOperationsByMemoizedReduce
    // already proves out for an unrelated recurrence.
    public static int MaxOperationsByMemoizedTwoPointer(int[] nums)
    {
        var n = nums.Length;

        if (n < 2)
        {
            return 0;
        }

        var best = 1;
        best = Math.Max(best, 1 + WindowOperationsByMemoizedReduce(nums, 2, n - 1, nums[0] + nums[1]));
        best = Math.Max(best, 1 + WindowOperationsByMemoizedReduce(nums, 0, n - 3, nums[n - 1] + nums[n - 2]));
        best = Math.Max(best, 1 + WindowOperationsByMemoizedReduce(nums, 1, n - 2, nums[0] + nums[n - 1]));
        return best;
    }

    private static int WindowOperationsByMemoizedReduce(int[] nums, int left, int right, int target) =>
        Memoizer.Memoize<(int Left, int Right), int>(
            (left, right), new WindowOperationsFrom(nums, target));

    // The same three moves and the same recursion as BestMoveByDictionaryMemo above,
    // through Memoizer's own handle on the rule instead of a hand-rolled table - the only
    // thing the two strategies differ in.
    private static int WindowOperations(
        int[] nums,
        (int Left, int Right) state,
        int target,
        IRecurrence<(int Left, int Right), int> rest)
    {
        if (state.Left >= state.Right)
        {
            return 0;
        }

        var best = 0;

        foreach (var next in LegalMoves(nums, state, target))
        {
            best = Math.Max(best, 1 + rest.Replay(next, rest));
        }

        return best;
    }

    // LC 3040's three legal moves over one window - take the front pair, take the back
    // pair, or close in from both ends - each offered only when the pair it removes
    // scores the run's fixed target, paired with the window that move leaves behind.
    // Shared by both arms: the moves are the same recurrence either way.
    private static IEnumerable<(int Left, int Right)> LegalMoves(
        int[] nums, (int Left, int Right) state, int target)
    {
        var (left, right) = state;

        if (nums[left] + nums[left + 1] == target)
        {
            yield return (left + 2, right);
        }

        if (nums[right] + nums[right - 1] == target)
        {
            yield return (left, right - 2);
        }

        if (nums[left] + nums[right] == target)
        {
            yield return (left + 1, right - 1);
        }
    }

    // The three-move rule, named: the most operations still available from one (left,
    // right) window is one point for the move taken plus whatever the rule reports for the
    // window that move leaves behind. The array and the run's fixed target score arrive
    // once through the primary constructor; `rest` is the memo run's own handle on this
    // rule, so each move below recurses through a call on a named type.
    private sealed class WindowOperationsFrom(int[] nums, int target)
        : IRecurrence<(int Left, int Right), int>
    {
        /// <inheritdoc/>
        public int Replay((int Left, int Right) state, IRecurrence<(int Left, int Right), int> rest)
            => WindowOperations(nums, state, target, rest);
    }
}
