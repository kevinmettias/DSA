using MaxDigitsStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CreateMaximumNumber;

// LeetCode 321. Create Maximum Number: for every valid split of k digits between
// the two arrays, pull the largest length-i (resp. length-(k-i)) subsequence out of
// each with the same monotonic-stack sweep LargestRectangleInHistogram/
// MultiplyStrings already use this repo's own Stack<int> for, greedily merge the
// two subsequences preserving each one's relative order, and keep the
// lexicographically largest merged result across every split.
public sealed partial class CreateMaximumNumberTests
{
    [Theory]
    [InlineData(new[] { 3, 4, 6, 5 }, new[] { 9, 1, 2, 5, 8, 3 }, 5, new[] { 9, 8, 6, 5, 3 })]
    [InlineData(new[] { 6, 7 }, new[] { 6, 0, 4 }, 5, new[] { 6, 7, 6, 0, 4 })]
    [InlineData(new[] { 3, 9 }, new[] { 8, 9 }, 3, new[] { 9, 8, 9 })]
    public void MaxNumber_LeetCodeExamples_ReturnsLargestMergedDigits(int[] nums1, int[] nums2, int k, int[] expected)
    {
        var actual = MaxNumber(nums1, nums2, k);
        Assert.Equal(expected, actual);
    }

    private static int[] MaxNumber(int[] nums1, int[] nums2, int k)
    {
        var best = Array.Empty<int>();

        var lowI = Math.Max(0, k - nums2.Length);
        var highI = Math.Min(k, nums1.Length);

        for (var i = lowI; i <= highI; i++)
        {
            var subsequence1 = MaxSubsequence(nums1, i);
            var subsequence2 = MaxSubsequence(nums2, k - i);
            var candidate = MergePreferringLarger(subsequence1, subsequence2);
            if (IsGreaterOrEqual(candidate, 0, best, 0))
            {
                best = candidate;
            }
        }

        return best;
    }

    private static int[] MaxSubsequence(int[] nums, int length)
    {
        var stack = new MaxDigitsStack();
        var drop = nums.Length - length;

        BuildMonotonicStack(stack, nums, drop);
        TrimToLength(stack, length);

        return ExtractDigits(stack, length);
    }

    private static void BuildMonotonicStack(MaxDigitsStack stack, int[] nums, int drop)
    {
        foreach (var num in nums)
        {
            while (drop > 0 && stack.TryPeek(out var top) && top < num)
            {
                stack.TryPop(out _);
                drop--;
            }

            stack.Push(num);
        }
    }

    private static void TrimToLength(MaxDigitsStack stack, int length)
    {
        while (stack.Count > length)
        {
            stack.TryPop(out _);
        }
    }

    private static int[] ExtractDigits(MaxDigitsStack stack, int length)
    {
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
