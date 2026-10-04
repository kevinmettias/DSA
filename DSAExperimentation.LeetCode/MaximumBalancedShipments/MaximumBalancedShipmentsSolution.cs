using DSAExperimentation.Algorithms.Searching;

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
    // No earlier element is greater, so no balanced shipment ends at that index.
    private const int NoGreaterToTheLeft = -1;

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

    // NearestBoundary.GreaterToTheLeft finds every index's nearest earlier strictly-
    // greater element in one O(n) pass (the classic monotonic-stack reduction),
    // turning the O(n^2) backward walk above into an O(1) dp transition per index.
    public static int MaxBalancedShipmentsByPreviousGreaterStack(int[] weight)
    {
        var n = weight.Length;
        var previousGreater = NearestBoundary.GreaterToTheLeft(weight, NoGreaterToTheLeft);
        var dp = new int[n + 1];

        for (var i = 1; i <= n; i++)
        {
            dp[i] = dp[i - 1];

            var start = previousGreater[i - 1];

            if (start != NoGreaterToTheLeft)
            {
                dp[i] = Math.Max(dp[i], 1 + dp[start]);
            }
        }

        return dp[n];
    }
}
