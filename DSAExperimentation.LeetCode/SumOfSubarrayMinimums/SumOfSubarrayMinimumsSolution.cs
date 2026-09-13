using DSAExperimentation.Domain.Modular;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

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
// right[i]. Both distance arrays come from one sweep each over this repo's own
// Stack<int> of pending indices (the DailyTemperatures/NextGreaterElementI
// precedent), popping every index the new element dominates. The >= / > asymmetry
// between the two passes is what stops a run of equal minimums being counted
// twice: a tie is only ever owned by its leftmost occurrence. Every index enters
// and leaves each stack once, so the whole thing is O(n).
internal static class SumOfSubarrayMinimumsSolution
{
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
        var left = DistancesToPreviousSmaller(arr);
        var right = DistancesToNextSmallerOrEqual(arr);

        return SumWeightedContributions(arr, left, right);
    }

    // left[i] = how many subarray start positions end at i without meeting a
    // strictly smaller element first (i + 1 when no smaller element exists to the
    // left at all).
    private static int[] DistancesToPreviousSmaller(int[] arr)
    {
        var left = new int[arr.Length];
        var pendingIndices = new RepoIntStack();

        for (var i = 0; i < arr.Length; i++)
        {
            while (pendingIndices.TryPeek(out var top) && arr[top] >= arr[i])
            {
                pendingIndices.TryPop(out _);
            }

            left[i] = pendingIndices.TryPeek(out var previous) ? i - previous : i + 1;
            pendingIndices.Push(i);
        }

        return left;
    }

    // right[i] = the mirror image, stopping at the next smaller-or-equal element
    // rather than the next strictly smaller one, so equal values never both claim
    // the same subarray.
    private static int[] DistancesToNextSmallerOrEqual(int[] arr)
    {
        var right = new int[arr.Length];
        var pendingIndices = new RepoIntStack();

        for (var i = arr.Length - 1; i >= 0; i--)
        {
            while (pendingIndices.TryPeek(out var top) && arr[top] > arr[i])
            {
                pendingIndices.TryPop(out _);
            }

            right[i] = pendingIndices.TryPeek(out var next) ? next - i : arr.Length - i;
            pendingIndices.Push(i);
        }

        return right;
    }

    private static int SumWeightedContributions(int[] arr, int[] left, int[] right)
    {
        long sum = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            sum = (sum + ((long)arr[i] * left[i] * right[i])) % ModularArithmetic.Modulo;
        }

        return (int)sum;
    }
}
