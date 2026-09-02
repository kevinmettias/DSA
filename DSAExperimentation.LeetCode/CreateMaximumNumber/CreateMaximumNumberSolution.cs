using MaxDigitsStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.CreateMaximumNumber;

// LeetCode 321. Create Maximum Number: for every valid split of k digits between
// the two arrays, pull the largest length-i (resp. length-(k-i)) subsequence out of
// each, greedily merge the two subsequences preserving each one's relative order,
// and keep the lexicographically largest merged result across every split.
//
// The two strategies share that split/merge orchestration - it is identical either
// way - and differ only in how "largest subsequence of length L" is pulled out of a
// single array: a naive O(n * L) repeated scan of the still-eligible window, or this
// repo's own O(n) Stack<int> monotonic sweep (the LargestRectangleInHistogram/
// MultiplyStrings precedent).
internal static class CreateMaximumNumberSolution
{
    // The textbook O(n * length) baseline: repeatedly scan the still-eligible
    // window for its largest digit (leftmost on ties), then continue past it.
    // Deliberately written without this repo's primitives - it is the arm the
    // monotonic-stack strategy has to justify itself against.
    public static int[] MaxNumberByNaiveScan(int[] nums1, int[] nums2, int k) =>
        MaxNumber(nums1, nums2, k, NaiveMaxSubsequence);

    public static int[] MaxNumberByMonotonicStack(int[] nums1, int[] nums2, int k) =>
        MaxNumber(nums1, nums2, k, StackMaxSubsequence);

    private static int[] MaxNumber(int[] nums1, int[] nums2, int k, Func<int[], int, int[]> maxSubsequence)
    {
        var best = Array.Empty<int>();

        var lowI = Math.Max(0, k - nums2.Length);
        var highI = Math.Min(k, nums1.Length);

        for (var i = lowI; i <= highI; i++)
        {
            var subsequence1 = maxSubsequence(nums1, i);
            var subsequence2 = maxSubsequence(nums2, k - i);
            var candidate = MergePreferringLarger(subsequence1, subsequence2);
            if (IsGreaterOrEqual(candidate, 0, best, 0))
            {
                best = candidate;
            }
        }

        return best;
    }

    private static int[] NaiveMaxSubsequence(int[] nums, int length)
    {
        var result = new int[length];
        var start = 0;
        var n = nums.Length;

        for (var remaining = length; remaining >= 1; remaining--)
        {
            var end = n - remaining + 1;
            var maxIndex = start;
            for (var i = start + 1; i < end; i++)
            {
                if (nums[i] > nums[maxIndex])
                {
                    maxIndex = i;
                }
            }

            result[length - remaining] = nums[maxIndex];
            start = maxIndex + 1;
        }

        return result;
    }

    private static int[] StackMaxSubsequence(int[] nums, int length)
    {
        var stack = BuildMonotonicStack(nums, nums.Length - length);
        return ExtractSubsequence(stack, length);
    }

    private static MaxDigitsStack BuildMonotonicStack(int[] nums, int drop)
    {
        var stack = new MaxDigitsStack();

        foreach (var num in nums)
        {
            while (drop > 0 && stack.TryPeek(out var top) && top < num)
            {
                stack.TryPop(out _);
                drop--;
            }

            stack.Push(num);
        }

        return stack;
    }

    private static int[] ExtractSubsequence(MaxDigitsStack stack, int length)
    {
        while (stack.Count > length)
        {
            stack.TryPop(out _);
        }

        var result = new int[length];
        for (var i = length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return result;
    }

    private static int[] MergePreferringLarger(int[] a, int[] b)
    {
        var merged = new int[a.Length + b.Length];
        var ai = 0;
        var bi = 0;

        for (var m = 0; m < merged.Length; m++)
        {
            merged[m] = IsGreaterOrEqual(a, ai, b, bi) ? a[ai++] : b[bi++];
        }

        return merged;
    }

    // Compares a[ai..] against b[bi..] the way "is a's remaining suffix at least as
    // large" needs to work for both callers: during a merge, when one side's
    // remaining digits are a prefix of the other's, the longer remaining suffix
    // wins (there's more of it left to place); the same rule doubles as "candidate
    // >= best" when bi/ai both start at 0.
    private static bool IsGreaterOrEqual(int[] a, int ai, int[] b, int bi)
    {
        while (ai < a.Length && bi < b.Length && a[ai] == b[bi])
        {
            ai++;
            bi++;
        }

        return bi == b.Length || (ai < a.Length && a[ai] > b[bi]);
    }
}
