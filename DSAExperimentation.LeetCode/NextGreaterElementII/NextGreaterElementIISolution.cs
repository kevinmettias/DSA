using NextGreaterStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.NextGreaterElementII;

// LeetCode 503. Next Greater Element II: for a circular array, find the first
// greater element walking forward from each index, wrapping around once.
//
// The composed strategy is the classic monotonic-stack sweep over this repo's
// own Stack<int> - the same OneThreeTwoPattern/LargestRectangleInHistogram
// precedent - holding indices whose next-greater element hasn't been found
// yet, walked twice around the array (i % n) so every index gets a chance to
// see a greater value that wraps around from the front. The baseline instead
// scans up to n-1 positions ahead of each index directly.
internal static class NextGreaterElementIISolution
{
    // The textbook answer: an O(n^2) scan ahead of each index (wrapping via
    // modulo), deliberately written without this repo's primitives - it is the
    // arm the composed monotonic-stack sweep below has to justify itself against.
    public static int[] NextGreaterElementsByBruteForce(int[] nums)
    {
        var n = nums.Length;
        var result = new int[n];

        for (var i = 0; i < n; i++)
        {
            result[i] = LeetCodeAnswer.None;

            for (var offset = 1; offset < n; offset++)
            {
                var candidate = nums[(i + offset) % n];

                if (candidate > nums[i])
                {
                    result[i] = candidate;
                    break;
                }
            }
        }

        return result;
    }

    // This repo's own O(n) monotonic-stack sweep, walking the array twice so
    // every index can find a wrap-around next-greater value.
    public static int[] NextGreaterElementsByMonotonicStack(int[] nums)
    {
        var n = nums.Length;
        var result = new int[n];
        Array.Fill(result, LeetCodeAnswer.None);
        var pendingIndices = new NextGreaterStack();

        for (var i = 0; i < 2 * n; i++)
        {
            var value = nums[i % n];

            while (pendingIndices.TryPeek(out var top) && nums[top] < value)
            {
                pendingIndices.TryPop(out _);
                result[top] = value;
            }

            if (i < n)
            {
                pendingIndices.Push(i);
            }
        }

        return result;
    }
}
