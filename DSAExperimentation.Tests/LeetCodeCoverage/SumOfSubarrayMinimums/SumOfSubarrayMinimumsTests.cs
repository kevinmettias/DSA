using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfSubarrayMinimums;

// LeetCode 907. Sum of Subarray Minimums: rather than the O(n^2) "min of every
// subarray" scan, each element's contribution across every subarray it is the
// minimum of collapses to two monotonic-stack sweeps over this repo's own
// Stack<int> of pending indices (the same DailyTemperatures/NextGreaterElementI
// precedent) - one finds each index's distance to the previous strictly smaller
// element, the other to the next smaller-or-equal element. The </<= asymmetry
// between the two passes is what avoids double-counting a run of equal minimums:
// a tie is only ever "owned" by its leftmost occurrence. Every index enters and
// leaves each stack at most once, so both passes are O(n).
public sealed partial class SumOfSubarrayMinimumsTests
{
    private const int Modulus = 1_000_000_007;

    [Fact]
    public void SumSubarrayMins_LeetCodeExampleOne_ReturnsSeventeen()
        => Assert.Equal(17, SumSubarrayMins([3, 1, 2, 4]));

    [Fact]
    public void SumSubarrayMins_LeetCodeExampleTwo_ReturnsFourHundredFortyFour()
        => Assert.Equal(444, SumSubarrayMins([11, 81, 94, 43, 3]));

    [Fact]
    public void SumSubarrayMins_SingleElement_ReturnsThatElement()
        => Assert.Equal(7, SumSubarrayMins([7]));

    [Fact]
    public void SumSubarrayMins_DuplicateMinimums_DoesNotDoubleCount()
        => Assert.Equal(12, SumSubarrayMins([2, 2, 2]));

    private static int SumSubarrayMins(int[] arr)
    {
        var left = ComputeDistanceToPreviousSmaller(arr);
        var right = ComputeDistanceToNextSmallerOrEqual(arr);

        return SumWeightedContributions(arr, left, right);
    }

    private static int[] ComputeDistanceToPreviousSmaller(int[] arr)
    {
        var n = arr.Length;
        var left = new int[n];
        var stack = new RepoIntStack();

        for (var i = 0; i < n; i++)
        {
            while (stack.TryPeek(out var top) && arr[top] >= arr[i])
            {
                stack.TryPop(out _);
            }

            left[i] = stack.TryPeek(out var previous) ? i - previous : i + 1;
            stack.Push(i);
        }

        return left;
    }

    private static int[] ComputeDistanceToNextSmallerOrEqual(int[] arr)
    {
        var n = arr.Length;
        var right = new int[n];
        var stack = new RepoIntStack();

        for (var i = n - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && arr[top] > arr[i])
            {
                stack.TryPop(out _);
            }

            right[i] = stack.TryPeek(out var next) ? next - i : n - i;
            stack.Push(i);
        }

        return right;
    }

    private static int SumWeightedContributions(int[] arr, int[] left, int[] right)
    {
        long sum = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            sum = (sum + ((long)arr[i] * left[i] * right[i])) % Modulus;
        }

        return (int)sum;
    }
}
