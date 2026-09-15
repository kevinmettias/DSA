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

        ApplyQueries(values, queries, buckets, threshold);
        SweepBuckets(values, buckets);

        return XorAll(values);
    }

    // Sends every query to the arm its stride selects: one above the threshold walks
    // directly, since it touches at most n/k <= threshold indices; a smaller one is
    // marked into the difference array of its (stride, residue) bucket instead.
    private static void ApplyQueries(
        long[] values, int[][] queries, Dictionary<(int Stride, int Residue), long[]> buckets, int threshold)
    {
        var n = values.Length;

        foreach (var query in queries)
        {
            var (l, r, k, v) = (query[0], query[1], query[2], query[3]);

            if (k > threshold)
            {
                ApplyStridedWalk(values, (L: l, R: r, K: k, V: v));
                continue;
            }

            MarkBucket(buckets, n, (L: l, R: r, K: k, V: v));
        }
    }

    // Multiplies every k-th index in [l, r] by v - Part I's strided walk, used here only
    // for the strides large enough to make it cheap.
    private static void ApplyStridedWalk(long[] values, (int L, int R, int K, int V) query)
    {
        for (var idx = query.L; idx <= query.R; idx += query.K)
        {
            values[idx] = values[idx] * query.V % ModularArithmetic.Modulo;
        }
    }

    // Applies every bucket's multiplicative difference array to the values it covers.
    private static void SweepBuckets(long[] values, Dictionary<(int Stride, int Residue), long[]> buckets)
    {
        foreach (var ((stride, residue), diff) in buckets)
        {
            SweepBucket(values, diff, stride, residue);
        }
    }

    // l, r, k and v are the query itself - the same four values LeetCode hands over
    // together and the same four the walk arm above unpacks - so they arrive as that
    // one query rather than as four independent ints. n is not part of the query: it
    // is the array length the bucket's compressed coordinates are sized against.
    private static void MarkBucket(
        Dictionary<(int, int), long[]> buckets, int n, (int L, int R, int K, int V) query)
    {
        var residue = query.L % query.K;
        var diff = ResolveBucket(buckets, n, query.K, residue);

        MarkQueryRange(diff, query, residue);
    }

    // A bucket's diff array has one slot per compressed position (0..bucketLength -
    // 1) plus one trailing sentinel slot, so a cancel mark at posR + 1 always lands
    // in bounds even when posR is the bucket's last position. A bucket is created the
    // first time a query lands in it, filled at the multiplicative identity.
    private static long[] ResolveBucket(
        Dictionary<(int, int), long[]> buckets, int n, int stride, int residue)
    {
        if (!buckets.TryGetValue((stride, residue), out var diff))
        {
            var bucketLength = (n - residue + stride - 1) / stride;
            diff = new long[bucketLength + 1];
            Array.Fill(diff, 1L);
            buckets[(stride, residue)] = diff;
        }

        return diff;
    }

    // The query's own two multiplicative marks in its bucket's diff array: one at L's
    // compressed position carrying v, and a cancelling one just past R's carrying v's
    // modular inverse, so the mark's effect stops at R.
    private static void MarkQueryRange(long[] diff, (int L, int R, int K, int V) query, int residue)
    {
        var posL = (query.L - residue) / query.K;
        var posR = (query.R - residue) / query.K;

        diff[posL] = diff[posL] * query.V % ModularArithmetic.Modulo;
        diff[posR + 1] = diff[posR + 1] * ModularArithmetic.Inverse(query.V) % ModularArithmetic.Modulo;
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
