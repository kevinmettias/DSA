using MonotonicStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulTowersII;

// LeetCode 2866. Beautiful Towers II: identical problem to Beautiful Towers I
// (id 2865, see BeautifulTowersITests) - only maxHeights[i]/n's upper bound grows,
// which is exactly why the O(n) monotonic-stack solution (not I's also-tractable
// O(n^2) per-peak walk) is the one that actually matters here. Same composition:
// this repo's Stack<int>, one increasing-stack pass per direction.
public sealed class BeautifulTowersIITests
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
    public void MaximumSumOfHeightsByMonotonicStack_StrictlyIncreasingMaxHeights_PeaksAtTheEnd()
    {
        // Non-decreasing already, so the whole array is its own best left run and
        // the peak sits at the last index - the maximum-sum long-overflow-prone
        // shape II's larger n is meant to exercise.
        int[] maxHeights = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        Assert.Equal(55, MaximumSumOfHeightsByMonotonicStack(maxHeights));
    }

    [Fact]
    public void MaximumSumOfHeightsByMonotonicStack_AllEqualMaxHeights_SumsEveryTowerAtCap()
    {
        int[] maxHeights = Enumerable.Repeat(1_000_000_000, 1_000).ToArray();

        Assert.Equal(1_000_000_000L * 1_000, MaximumSumOfHeightsByMonotonicStack(maxHeights));
    }

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
