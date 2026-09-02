using IndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.CountBowlSubarrays;

// LeetCode 3676. Count Bowl Subarrays: nums has distinct elements, and
// nums[l..r] (r - l + 1 >= 3) is a "bowl" when min(nums[l], nums[r]) is strictly
// greater than max(nums[l+1..r-1]).
//
// Every bowl is pinned by exactly one of its two ends - the one holding the
// SMALLER value, since the bowl condition is really "min(ends) > interior max".
// Whichever end is smaller, the interior can never reach as far as that end's own
// next-greater neighbour (in whichever direction), or the interior max would meet
// or beat it. So [l, r] is a bowl iff either r is l's next strictly-greater index
// (nums[l] < nums[r]) or l is r's previous strictly-greater index (nums[r] <
// nums[l]) - two arrays a single monotonic-decreasing stack pass each produces,
// composed here from this repo's own Stack<int> rather than a hand-rolled array
// stack.
internal static class CountBowlSubarraysSolution
{
    private const int NoGreaterElement = -1;

    // Textbook reading of the definition: for every (l, r) pair of length >= 3,
    // track the interior max with a running value as r grows - still checks every
    // pair directly against the definition, with no insight about which ones can
    // possibly qualify.
    public static int CountBowlsByPairScan(int[] nums)
    {
        var count = 0;

        for (var l = 0; l < nums.Length; l++)
        {
            var interiorMax = int.MinValue;

            for (var r = l + 1; r < nums.Length; r++)
            {
                if (r - l >= 2 && Math.Min(nums[l], nums[r]) > interiorMax)
                {
                    count++;
                }

                interiorMax = Math.Max(interiorMax, nums[r]);
            }
        }

        return count;
    }

    // Composed: only the 2n (index, next/previous strictly-greater index) pairs a
    // monotonic-decreasing Stack<int> pass produces can ever be a bowl's pinning
    // end, so counting those directly replaces the O(n^2) pair scan with two O(n)
    // passes.
    public static int CountBowlsByMonotonicStack(int[] nums)
    {
        var nextGreater = NextGreaterIndices(nums);
        var previousGreater = PreviousGreaterIndices(nums);
        var count = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if (nextGreater[i] != NoGreaterElement && nextGreater[i] - i >= 2)
            {
                count++;
            }

            if (previousGreater[i] != NoGreaterElement && i - previousGreater[i] >= 2)
            {
                count++;
            }
        }

        return count;
    }

    private static int[] NextGreaterIndices(int[] nums)
    {
        var result = new int[nums.Length];
        Array.Fill(result, NoGreaterElement);

        var stack = new IndexStack();

        for (var i = 0; i < nums.Length; i++)
        {
            while (stack.TryPeek(out var top) && nums[top] < nums[i])
            {
                stack.TryPop(out _);
                result[top] = i;
            }

            stack.Push(i);
        }

        return result;
    }

    private static int[] PreviousGreaterIndices(int[] nums)
    {
        var result = new int[nums.Length];
        Array.Fill(result, NoGreaterElement);

        var stack = new IndexStack();

        for (var i = 0; i < nums.Length; i++)
        {
            while (stack.TryPeek(out var top) && nums[top] < nums[i])
            {
                stack.TryPop(out _);
            }

            result[i] = stack.TryPeek(out var previous) ? previous : NoGreaterElement;
            stack.Push(i);
        }

        return result;
    }
}
