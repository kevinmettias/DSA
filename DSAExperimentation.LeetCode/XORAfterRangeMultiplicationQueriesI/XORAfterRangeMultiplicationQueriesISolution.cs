using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.XORAfterRangeMultiplicationQueriesI;

// LeetCode 3653. XOR After Range Multiplication Queries I: for each query
// [l, r, k, v], multiply nums[l], nums[l + k], nums[l + 2k], ... (every index up to
// r) by v, modulo 1e9+7, in order. Return the XOR of the final array.
//
// n, q <= 1000 here (the "I" version), so a direct simulation of every query is the
// intended solution - there is no batching trick across queries with different k's
// worth reaching for. The two strategies differ only in how a query walks its
// affected indices: checking every position in [l, r] and skipping the ones that
// don't line up with the stride, versus stepping directly by k the way the problem
// statement itself describes.
internal static class XORAfterRangeMultiplicationQueriesISolution
{
    // Textbook baseline: visit every index in [l, r] and filter by (idx - l) % k,
    // instead of stepping straight to the indices that qualify. O(sum of (r - l + 1))
    // - the arm the composed strategy below has to justify itself against.
    public static int XorAfterQueriesByRangeScan(int[] nums, int[][] queries)
    {
        var values = (int[])nums.Clone();

        foreach (var query in queries)
        {
            var (l, r, k, v) = (query[0], query[1], query[2], query[3]);

            for (var idx = l; idx <= r; idx++)
            {
                if ((idx - l) % k == 0)
                {
                    values[idx] = (int)((long)values[idx] * v % ModularArithmetic.Modulo);
                }
            }
        }

        return XorAll(values);
    }

    // The query's own shape: start at l and step by k, so only the indices that
    // actually get multiplied are ever touched. O(sum of (r - l) / k + 1) - the same
    // "step directly to what matters" saving MinimumCostPathWithEdgeReversalsSolution
    // gets from its own graph strategy, just without a search behind it here.
    public static int XorAfterQueriesByStridedWalk(int[] nums, int[][] queries)
    {
        var values = (int[])nums.Clone();

        foreach (var query in queries)
        {
            var (l, r, k, v) = (query[0], query[1], query[2], query[3]);

            for (var idx = l; idx <= r; idx += k)
            {
                values[idx] = (int)((long)values[idx] * v % ModularArithmetic.Modulo);
            }
        }

        return XorAll(values);
    }

    private static int XorAll(int[] values)
    {
        var result = 0;

        foreach (var value in values)
        {
            result ^= value;
        }

        return result;
    }
}
