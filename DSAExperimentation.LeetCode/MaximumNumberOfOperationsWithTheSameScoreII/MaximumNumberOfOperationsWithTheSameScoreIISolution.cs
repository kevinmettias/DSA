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
        best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, 2, n - 1, nums[0] + nums[1]));
        best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, 0, n - 3, nums[n - 1] + nums[n - 2]));
        best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, 1, n - 2, nums[0] + nums[n - 1]));
        return best;
    }

    private static int WindowOperationsByDictionaryMemo(int[] nums, int left, int right, int target) =>
        WindowOperationsByDictionaryMemo(nums, left, right, target, new Dictionary<(int Left, int Right), int>());

    private static int WindowOperationsByDictionaryMemo(
        int[] nums, int left, int right, int target, Dictionary<(int Left, int Right), int> memo)
    {
        if (left >= right)
        {
            return 0;
        }

        if (memo.TryGetValue((left, right), out var cached))
        {
            return cached;
        }

        var best = 0;

        if (nums[left] + nums[left + 1] == target)
        {
            best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, left + 2, right, target, memo));
        }

        if (nums[right] + nums[right - 1] == target)
        {
            best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, left, right - 2, target, memo));
        }

        if (nums[left] + nums[right] == target)
        {
            best = Math.Max(best, 1 + WindowOperationsByDictionaryMemo(nums, left + 1, right - 1, target, memo));
        }

        memo[(left, right)] = best;
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
            (left, right), (state, recurse) => WindowOperations(nums, state, target, recurse));

    private static int WindowOperations(
        int[] nums, (int Left, int Right) state, int target, Func<(int Left, int Right), int> recurse)
    {
        var (left, right) = state;

        if (left >= right)
        {
            return 0;
        }

        var best = 0;

        if (nums[left] + nums[left + 1] == target)
        {
            best = Math.Max(best, 1 + recurse((left + 2, right)));
        }

        if (nums[right] + nums[right - 1] == target)
        {
            best = Math.Max(best, 1 + recurse((left, right - 2)));
        }

        if (nums[left] + nums[right] == target)
        {
            best = Math.Max(best, 1 + recurse((left + 1, right - 1)));
        }

        return best;
    }
}
