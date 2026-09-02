using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.XORAfterRangeMultiplicationQueriesII;

// LeetCode 3655. XOR After Range Multiplication Queries II: same [l, r, k, v] range-
// multiplication queries as Part I (nums[idx] = nums[idx] * v mod 1e9+7 for
// idx = l, l+k, l+2k, ... up to r), but n, q <= 1e5 here instead of Part I's <= 1000
// - a query with a small k can touch O(n) indices, and there can be O(n) such
// queries, so Part I's unconditional strided walk degrades to O(n * q) and is no
// longer sufficient on its own.
//
// The composed strategy buckets every "small-k" query by (k, l mod k): within one
// bucket, a residue class's touched indices are contiguous in the bucket's own
// compressed coordinates (position t <-> original index residue + t*k), so a query
// becomes one O(1) multiplicative difference-array mark there instead of an O(n/k)
// walk. Cancelling a mark needs the multiplier's modular inverse - Domain.Modular's
// job, and always defined here since v is in [1, 1e5], never 0 mod the 1e9+7 prime.
// "Large-k" queries already touch at most n/k <= threshold indices, so they are
// walked directly, the same way Part I walks every query.
internal static class XORAfterRangeMultiplicationQueriesIISolution
{
    // Textbook baseline: Part I's strided walk, unconditionally, regardless of k -
    // the arm the bucketed strategy below has to justify itself against at this
    // problem's larger n and q.
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

    // Sqrt-decomposition: queries with a stride above the threshold are walked
    // directly (few indices each); queries with a stride at or below it are grouped
    // into per-(k, residue) multiplicative difference arrays and applied with one
    // sweep per bucket. Multiplication is commutative, so it never matters that
    // this processes queries out of their original order or bucket-by-bucket
    // instead of query-by-query - only the final per-index product does.
    public static int XorAfterQueriesBySqrtDecomposition(int[] nums, int[][] queries)
    {
        var n = nums.Length;
        var values = Array.ConvertAll(nums, value => (long)value);
        var threshold = (int)Math.Sqrt(n) + 1;
        var buckets = new Dictionary<(int Stride, int Residue), long[]>();

        foreach (var query in queries)
        {
            var (l, r, k, v) = (query[0], query[1], query[2], query[3]);

            if (k > threshold)
            {
                for (var idx = l; idx <= r; idx += k)
                {
                    values[idx] = values[idx] * v % ModularArithmetic.Modulo;
                }

                continue;
            }

            MarkBucket(buckets, n, l, r, k, v);
        }

        foreach (var ((stride, residue), diff) in buckets)
        {
            SweepBucket(values, diff, stride, residue);
        }

        return XorAll(values);
    }

    // A bucket's diff array has one slot per compressed position (0..bucketLength -
    // 1) plus one trailing sentinel slot, so a cancel mark at posR + 1 always lands
    // in bounds even when posR is the bucket's last position.
    private static void MarkBucket(
        Dictionary<(int, int), long[]> buckets, int n, int l, int r, int k, int v)
    {
        var residue = l % k;
        var key = (k, residue);

        if (!buckets.TryGetValue(key, out var diff))
        {
            var bucketLength = (n - residue + k - 1) / k;
            diff = new long[bucketLength + 1];
            Array.Fill(diff, 1L);
            buckets[key] = diff;
        }

        var posL = (l - residue) / k;
        var posR = (r - residue) / k;

        diff[posL] = diff[posL] * v % ModularArithmetic.Modulo;
        diff[posR + 1] = diff[posR + 1] * ModularArithmetic.Inverse(v) % ModularArithmetic.Modulo;
    }

    private static void SweepBucket(long[] values, long[] diff, int stride, int residue)
    {
        var running = 1L;

        for (var t = 0; t < diff.Length - 1; t++)
        {
            running = running * diff[t] % ModularArithmetic.Modulo;

            var idx = residue + t * stride;
            values[idx] = values[idx] * running % ModularArithmetic.Modulo;
        }
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

    private static int XorAll(long[] values)
    {
        var result = 0L;

        foreach (var value in values)
        {
            result ^= value;
        }

        return (int)result;
    }
}
