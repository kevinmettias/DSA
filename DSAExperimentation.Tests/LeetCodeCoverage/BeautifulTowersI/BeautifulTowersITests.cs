using MonotonicStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulTowersI;

// LeetCode 2865. Beautiful Towers I: choose heights[i] in [1, maxHeights[i]] so the
// sequence is a mountain (non-decreasing then non-increasing) maximizing the sum.
// For a fixed peak, the optimal one-sided run clamps every value to the running
// minimum walking away from the peak - the O(n) solution (same LargestRectangleIn
// HistogramTests precedent) computes that clamped sum for EVERY index as a peak in
// one monotonic-stack pass per direction, over this repo's own Stack<int>, instead
// of re-walking O(n) per candidate peak.
public sealed class BeautifulTowersITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [5, 3, 4, 1, 1], 13 },
            { [6, 5, 3, 9, 2, 7], 22 },
            { [3, 2, 5, 5, 2, 3], 18 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumOfHeightsByBruteForce_LeetCodeExamples_ReturnsMaxSum(int[] maxHeights, long expected)
        => Assert.Equal(expected, MaximumSumOfHeightsByBruteForce(maxHeights));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumOfHeightsByMonotonicStack_LeetCodeExamples_ReturnsMaxSum(int[] maxHeights, long expected)
        => Assert.Equal(expected, MaximumSumOfHeightsByMonotonicStack(maxHeights));

    [Fact]
    public void MaximumSumOfHeightsByMonotonicStack_SingleTower_ReturnsItsOwnHeight()
        => Assert.Equal(7, MaximumSumOfHeightsByMonotonicStack([7]));

    private static long MaximumSumOfHeightsByBruteForce(int[] maxHeights)
    {
        var n = maxHeights.Length;
        var best = 0L;

        for (var peak = 0; peak < n; peak++)
        {
            var sum = (long)maxHeights[peak];

            var cap = maxHeights[peak];
            for (var j = peak - 1; j >= 0; j--)
            {
                cap = Math.Min(cap, maxHeights[j]);
                sum += cap;
            }

            cap = maxHeights[peak];
            for (var j = peak + 1; j < n; j++)
            {
                cap = Math.Min(cap, maxHeights[j]);
                sum += cap;
            }

            best = Math.Max(best, sum);
        }

        return best;
    }

    private static long MaximumSumOfHeightsByMonotonicStack(int[] maxHeights)
    {
        var left = ComputeLeftSums(maxHeights);
        var right = ComputeRightSums(maxHeights);
        var best = 0L;

        for (var i = 0; i < maxHeights.Length; i++)
        {
            best = Math.Max(best, left[i] + right[i] - maxHeights[i]);
        }

        return best;
    }

    // sums[i] = best achievable sum over [0, i] with a non-decreasing run reaching
    // maxHeights[i] at index i. The stack holds indices with strictly increasing
    // maxHeights; popping past every taller-or-equal one lands on the nearest
    // shorter index to reuse its already-clamped sum for the gap in between.
    private static long[] ComputeLeftSums(int[] maxHeights)
    {
        var n = maxHeights.Length;
        var sums = new long[n];
        var stack = new MonotonicStack();

        for (var i = 0; i < n; i++)
        {
            while (stack.TryPeek(out var top) && maxHeights[top] > maxHeights[i])
            {
                stack.TryPop(out _);
            }

            sums[i] = stack.TryPeek(out var prev)
                ? sums[prev] + (long)maxHeights[i] * (i - prev)
                : (long)maxHeights[i] * (i + 1);

            stack.Push(i);
        }

        return sums;
    }

    // Mirror of ComputeLeftSums, swept right-to-left for the non-increasing run
    // reaching maxHeights[i] from the right.
    private static long[] ComputeRightSums(int[] maxHeights)
    {
        var n = maxHeights.Length;
        var sums = new long[n];
        var stack = new MonotonicStack();

        for (var i = n - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && maxHeights[top] > maxHeights[i])
            {
                stack.TryPop(out _);
            }

            sums[i] = stack.TryPeek(out var next)
                ? sums[next] + (long)maxHeights[i] * (next - i)
                : (long)maxHeights[i] * (n - i);

            stack.Push(i);
        }

        return sums;
    }
}
