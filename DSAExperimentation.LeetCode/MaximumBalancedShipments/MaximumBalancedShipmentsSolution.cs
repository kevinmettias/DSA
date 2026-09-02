using IndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaximumBalancedShipments;

// LeetCode 3638. Maximum Balanced Shipments: choose the most non-overlapping
// contiguous shipments from weight where a shipment is "balanced" whenever its
// last parcel is strictly less than the maximum weight anywhere in it.
//
// A balanced shipment ending at i can start anywhere from the nearest earlier
// strictly-greater element through i itself (any wider window still has that
// element inside it as the max; any window that doesn't include it has weight[i]
// itself as the max, which fails "strictly less"). Both strategies are the same
// dp[i] = max(dp[i-1], 1 + dp[start-1]) recurrence over that fact - they differ
// only in how they find each index's nearest earlier strictly-greater element.
internal static class MaximumBalancedShipmentsSolution
{
    // The textbook O(n^2) DP: for each end index, walk backward maintaining the
    // running max directly, no auxiliary structure - the monotonic-stack strategy
    // below has to beat it.
    public static int MaxBalancedShipmentsByBruteForce(int[] weight)
    {
        var n = weight.Length;
        var dp = new int[n + 1];

        for (var i = 1; i <= n; i++)
        {
            dp[i] = dp[i - 1];
            var runningMax = weight[i - 1];

            for (var start = i - 1; start >= 1; start--)
            {
                runningMax = Math.Max(runningMax, weight[start - 1]);

                if (weight[i - 1] < runningMax)
                {
                    dp[i] = Math.Max(dp[i], 1 + dp[start - 1]);
                }
            }
        }

        return dp[n];
    }

    // This repo's own Stack<int> finds every index's nearest earlier strictly-
    // greater element in one O(n) pass (the classic monotonic-stack reduction),
    // turning the O(n^2) backward walk above into an O(1) dp transition per index.
    public static int MaxBalancedShipmentsByPreviousGreaterStack(int[] weight)
    {
        var n = weight.Length;
        var previousGreater = PreviousGreaterIndices(weight);
        var dp = new int[n + 1];

        for (var i = 1; i <= n; i++)
        {
            dp[i] = dp[i - 1];

            if (previousGreater[i - 1] is int start)
            {
                dp[i] = Math.Max(dp[i], 1 + dp[start]);
            }
        }

        return dp[n];
    }

    // result[i] = the largest index j < i with weight[j] > weight[i], or null when
    // no earlier element is greater. A decreasing stack of candidate indices: any
    // index whose weight doesn't exceed the current one can never be *anyone's*
    // previous-greater from here on, so it is popped for good before i is pushed.
    private static int?[] PreviousGreaterIndices(int[] weight)
    {
        var result = new int?[weight.Length];
        var candidates = new IndexStack();

        for (var i = 0; i < weight.Length; i++)
        {
            while (candidates.TryPeek(out var top) && weight[top] <= weight[i])
            {
                candidates.TryPop(out _);
            }

            result[i] = candidates.TryPeek(out var previous) ? previous : null;
            candidates.Push(i);
        }

        return result;
    }
}
