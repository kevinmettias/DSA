using MaxDigitsStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.CreateMaximumNumber;

// LeetCode 321. Create Maximum Number: for every valid split of the digit count
// between the two arrays, pull the largest length-i (resp. length-(digitCount - i))
// subsequence out of each, greedily merge the two subsequences preserving each
// one's relative order, and keep the lexicographically largest merged result
// across every split.
//
// The two strategies share that split/merge orchestration - it is identical either
// way - and differ only in how "largest subsequence of length L" is pulled out of a
// single array: a naive O(n * L) repeated scan of the still-eligible window, or this
// repo's own O(n) Stack<int> monotonic sweep (the LargestRectangleInHistogram/
// MultiplyStrings precedent).
internal static class CreateMaximumNumberSolution
{
    // Both strategies are stateless, so one instance each serves every call and the
    // benchmark arms that measure these two methods allocate nothing to pick one.
    private static readonly ILargestSubsequence NaiveScan = new LargestSubsequenceByNaiveScan();
    private static readonly ILargestSubsequence MonotonicStack = new LargestSubsequenceByMonotonicStack();

    public static int[] MaxNumberByNaiveScan(int[] nums1, int[] nums2, int digitCount) =>
        MaxNumber(nums1, nums2, digitCount, NaiveScan);

    public static int[] MaxNumberByMonotonicStack(int[] nums1, int[] nums2, int digitCount) =>
        MaxNumber(nums1, nums2, digitCount, MonotonicStack);

    // The one question the two strategies answer differently: the largest
    // subsequence of exactly `length` digits that can be pulled out of `digits`
    // while every digit it keeps stays in its original relative order, ties broken
    // by taking the leftmost largest digit. Both inputs are named here, and that
    // contract - the thing a bare `Func<int[], int, int[]>` had nowhere to state -
    // has somewhere to be written down.
    private interface ILargestSubsequence
    {
        int[] Take(int[] digits, int length);
    }

    // The textbook O(n * length) baseline: repeatedly scan the still-eligible
    // window for its largest digit (leftmost on ties), then continue past it.
    // Deliberately written without this repo's primitives - it is the arm the
    // monotonic-stack strategy has to justify itself against.
    private sealed class LargestSubsequenceByNaiveScan : ILargestSubsequence
    {
        public int[] Take(int[] nums, int length)
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
    }

    private sealed class LargestSubsequenceByMonotonicStack : ILargestSubsequence
    {
        public int[] Take(int[] nums, int length)
        {
            var stack = BuildMonotonicStack(nums, nums.Length - length);
            return ExtractSubsequence(stack, length);
        }
    }

    private static MaxDigitsStack BuildMonotonicStack(int[] nums, int drop)
    {
        var stack = new MaxDigitsStack();

        foreach (var num in nums)
        {
            while (ShouldDropTop(drop, stack, num))
            {
                stack.TryPop(out _);
                drop--;
            }

            stack.Push(num);
        }

        return stack;
    }

    // A drop is still available and the digit sitting on top of the stack is
    // smaller than the one arriving, so popping it makes the number larger.
    private static bool ShouldDropTop(int drop, MaxDigitsStack stack, int incoming)
        => drop > 0 && stack.TryPeek(out var top) && top < incoming;

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

    private static int[] MaxNumber(int[] nums1, int[] nums2, int digitCount, ILargestSubsequence maxSubsequence)
    {
        var best = Array.Empty<int>();

        var lowI = Math.Max(0, digitCount - nums2.Length);
        var highI = Math.Min(digitCount, nums1.Length);

        for (var i = lowI; i <= highI; i++)
        {
            var subsequence1 = maxSubsequence.Take(nums1, i);
            var subsequence2 = maxSubsequence.Take(nums2, digitCount - i);
            var candidate = MergePreferringLarger(subsequence1, subsequence2);
            if (IsGreaterOrEqual(candidate, 0, best, 0))
            {
                best = candidate;
            }
        }

        return best;
    }

    private static int[] MergePreferringLarger(int[] leftDigits, int[] rightDigits)
    {
        var merged = new int[leftDigits.Length + rightDigits.Length];
        var leftIndex = 0;
        var rightIndex = 0;

        for (var m = 0; m < merged.Length; m++)
        {
            merged[m] = IsGreaterOrEqual(leftDigits, leftIndex, rightDigits, rightIndex)
                ? TakeNext(leftDigits, ref leftIndex)
                : TakeNext(rightDigits, ref rightIndex);
        }

        return merged;
    }

    // Reads the digit at `index` and steps past it: the one-step read a merge makes.
    private static int TakeNext(int[] source, ref int index) => source[index++];

    // Compares leftDigits[leftIndex..] against rightDigits[rightIndex..] the way "is
    // the left one's remaining suffix at least as large" needs to work for both
    // callers: during a merge, when one side's remaining digits are a prefix of the
    // other's, the longer remaining suffix wins (there's more of it left to place);
    // the same rule doubles as "candidate >= best" when both cursors start at 0.
    private static bool IsGreaterOrEqual(int[] leftDigits, int leftIndex, int[] rightDigits, int rightIndex)
    {
        while (IsNextDigitPairEqual(leftDigits, leftIndex, rightDigits, rightIndex))
        {
            leftIndex++;
            rightIndex++;
        }

        return rightIndex == rightDigits.Length
            || (leftIndex < leftDigits.Length && leftDigits[leftIndex] > rightDigits[rightIndex]);
    }

    // Both remaining suffixes have a digit left to compare at their cursor, and
    // those digits are the same - so neither side can be called larger yet.
    private static bool IsNextDigitPairEqual(int[] leftDigits, int leftIndex, int[] rightDigits, int rightIndex)
        => leftIndex < leftDigits.Length && rightIndex < rightDigits.Length
            && leftDigits[leftIndex] == rightDigits[rightIndex];
}
