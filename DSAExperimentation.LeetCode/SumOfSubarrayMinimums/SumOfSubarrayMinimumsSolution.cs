using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.SumOfSubarrayMinimums;

// LeetCode 907. Sum of Subarray Minimums: sum min(b) over every contiguous
// subarray b, modulo 1e9+7.
//
// SumSubarrayMinsByBruteForce is the textbook O(n^2): fix a start index and extend
// a running minimum rightwards, adding it at every step.
//
// SumSubarrayMinsByMonotonicStack flips the question from "what is each subarray's
// minimum" to "how many subarrays is each element the minimum of". An element at i
// owns every subarray whose start lies in the run back to the previous strictly
// smaller element and whose end lies in the run forward to the next
// smaller-or-equal element, so its total contribution is arr[i] * left[i] *
// right[i]. Both distances are index gaps to a boundary from one NearestBoundary
// sweep each - SmallerToTheLeft back, SmallerOrEqualToTheRight forward. That strict /
// or-equal asymmetry between the two sweeps is what stops a run of equal minimums
// being counted twice: a tie is only ever owned by its leftmost occurrence. Every
// index enters and leaves each sweep's stack once, so the whole thing is O(n).
internal static class SumOfSubarrayMinimumsSolution
{
    private const int NoSmallerElementToTheLeft = -1;

    // The textbook answer: every subarray's minimum, computed directly.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int SumSubarrayMinsByBruteForce(int[] arr)
    {
        long sum = 0;

        for (var start = 0; start < arr.Length; start++)
        {
            var min = arr[start];

            for (var end = start; end < arr.Length; end++)
            {
                min = Math.Min(min, arr[end]);
                sum = (sum + min) % ModularArithmetic.Modulo;
            }
        }

        return (int)sum;
    }

    public static int SumSubarrayMinsByMonotonicStack(int[] arr)
    {
        // Both sentinels are one step outside the array, so the distances below need no
        // special case when nothing blocks: i + 1 start positions back to the array's
        // start, length - i end positions forward to its end.
        var previousSmaller = NearestBoundary.SmallerToTheLeft(arr, NoSmallerElementToTheLeft);
        var nextSmallerOrEqual = NearestBoundary.SmallerOrEqualToTheRight(arr, arr.Length);

        return SumWeightedContributions(arr, previousSmaller, nextSmallerOrEqual);
    }

    // left = how many subarray start positions reach i without meeting a strictly
    // smaller element first; right = the mirror image, stopping at the next
    // smaller-or-equal element rather than the next strictly smaller one, so equal
    // values never both claim the same subarray.
    private static int SumWeightedContributions(int[] arr, int[] previousSmaller, int[] nextSmallerOrEqual)
    {
        long sum = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            var left = i - previousSmaller[i];
            var right = nextSmallerOrEqual[i] - i;
            sum = (sum + ((long)arr[i] * left * right)) % ModularArithmetic.Modulo;
        }

        return (int)sum;
    }
}
