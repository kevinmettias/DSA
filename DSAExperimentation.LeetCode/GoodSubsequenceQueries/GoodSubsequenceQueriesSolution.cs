namespace DSAExperimentation.LeetCode.GoodSubsequenceQueries;

// LeetCode 3901. Good Subsequence Queries: after each update, does nums have a
// proper (length < n) non-empty subsequence whose gcd is exactly p?
//
// Only elements divisible by p can ever belong to such a subsequence, and a
// subset's gcd is always a multiple of the gcd of any superset drawn from the
// same elements - so removing elements can only ever raise (never lower) the
// divisibility of the gcd. That makes the *full* set of p-multiples the unique
// best candidate: dividing every one of those elements by p, if their combined
// gcd isn't exactly 1, no smaller subset can do better either, so the answer is
// no. If it is 1, that full set already witnesses "yes" - unless it happens to
// be the entire array (every element a multiple of p), in which case the
// witness itself has length n and is disqualified, and the question becomes
// whether dropping any single element still leaves a combined gcd of 1.
//
// Both strategies share that reduction and the same GcdOperation; they differ
// only in how a query's post-update gcd/multiple-count are obtained - an O(n)
// rescan from scratch, or two persistent SegmentTree instances point-updated in
// O(log n) and range-queried in O(log n), falling back to a scan only for the
// one case a range query alone cannot decide (see HasGoodSubsequenceByRangeQuery).
internal static class GoodSubsequenceQueriesSolution
{
    // The textbook answer: no persistent structure, every query rescans nums'
    // p-divided values from scratch in O(n) - "what you'd write without this
    // repo" (ARCHITECTURE.md 17.5).
    public static int CountGoodSubseqByBruteForce(int[] nums, int p, int[][] queries)
    {
        var divided = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            divided[i] = DivideByP(nums[i], p);
        }

        var answer = 0;

        foreach (var query in queries)
        {
            divided[query[0]] = DivideByP(query[1], p);

            if (HasGoodSubsequenceByScan(divided))
            {
                answer++;
            }
        }

        return answer;
    }

    private static bool HasGoodSubsequenceByScan(int[] divided)
    {
        var multiplesCount = 0;
        var gcd = GcdOperation.Identity;

        foreach (var value in divided)
        {
            if (value != 0)
            {
                multiplesCount++;
            }

            gcd = GcdOperation.Combine(gcd, value);
        }

        if (multiplesCount == 0 || gcd != 1)
        {
            return false;
        }

        return multiplesCount < divided.Length || ExistsRemovableIndex(divided);
    }

    // Composed: builds a GoodSubsequenceIndex once so every query's full-range
    // gcd and multiple-count are O(log n) SegmentTree range queries instead of an
    // O(n) rescan - the Query-with-a-custom-ICombineOperation extensibility point
    // DataStructures.SegmentTree.SegmentTree<Element,TOperation> exists for,
    // instantiated twice over (gcd, sum) the same way MinimumStabilityFactorOfArray
    // instantiates it once for LC 3605's window gcds.
    public static int CountGoodSubseqBySegmentTreeGcd(int[] nums, int p, int[][] queries) =>
        CountGoodSubseqBySegmentTreeGcd(GoodSubsequenceIndex.Build(nums, p), queries);

    public static int CountGoodSubseqBySegmentTreeGcd(GoodSubsequenceIndex index, int[][] queries)
    {
        var n = index.Divided.Length;
        var answer = 0;

        foreach (var query in queries)
        {
            index.Apply(query[0], query[1]);

            if (HasGoodSubsequenceByRangeQuery(index, n))
            {
                answer++;
            }
        }

        return answer;
    }

    // The O(log n) range queries alone settle every query except the one where
    // every element is a multiple of p (multiplesCount == n): only then does
    // "does removing some single element keep the gcd at 1" need asking, and
    // that still requires an O(n) scan over the maintained Divided array - the
    // same fallback the brute-force arm always pays, paid here only on that
    // narrower edge case instead of on every query.
    private static bool HasGoodSubsequenceByRangeQuery(GoodSubsequenceIndex index, int n)
    {
        var multiplesCount = index.MultipleCountTree.Query(0, n - 1);

        if (multiplesCount == 0 || index.GcdTree.Query(0, n - 1) != 1)
        {
            return false;
        }

        return multiplesCount < n || ExistsRemovableIndex(index.Divided);
    }

    // Whether some single index can be dropped from divided (every entry already
    // a multiple of p) while the rest still combine to a gcd of 1: a standard
    // prefix/suffix gcd sweep, shared by both strategies exactly the way LC
    // 3605's GcdOperation.Combine is - not itself a second strategy, just the one
    // subroutine both arms need for this one case.
    private static bool ExistsRemovableIndex(int[] divided)
    {
        var n = divided.Length;
        var prefix = new int[n];
        var suffix = new int[n];

        prefix[0] = divided[0];
        for (var i = 1; i < n; i++)
        {
            prefix[i] = GcdOperation.Combine(prefix[i - 1], divided[i]);
        }

        suffix[n - 1] = divided[n - 1];
        for (var i = n - 2; i >= 0; i--)
        {
            suffix[i] = GcdOperation.Combine(suffix[i + 1], divided[i]);
        }

        for (var i = 0; i < n; i++)
        {
            var left = i > 0 ? prefix[i - 1] : GcdOperation.Identity;
            var right = i < n - 1 ? suffix[i + 1] : GcdOperation.Identity;

            if (GcdOperation.Combine(left, right) == 1)
            {
                return true;
            }
        }

        return false;
    }

    private static int DivideByP(int value, int p) => value % p == 0 ? value / p : 0;
}
