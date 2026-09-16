namespace DSAExperimentation.LeetCode.GoodSubsequenceQueries;

// LeetCode 3901. Good Subsequence Queries: after each update, does nums have a
// proper (length < n) non-empty subsequence whose gcd is exactly modulus?
//
// Only elements divisible by modulus can ever belong to such a subsequence, and a
// subset's gcd is always a multiple of the gcd of any superset drawn from the
// same elements - so removing elements can only ever raise (never lower) the
// divisibility of the gcd. That makes the *full* set of multiples of modulus the
// unique best candidate: dividing every one of those elements by modulus, if
// their combined gcd isn't exactly 1, no smaller subset can do better either, so
// the answer is no. If it is 1, that full set already witnesses "yes" - unless it
// happens to be the entire array (every element a multiple of modulus), in which
// case the witness itself has length n and is disqualified, and the question
// becomes whether dropping any single element still leaves a combined gcd of 1.
//
// Both strategies share that reduction and the same GcdOperation; they differ
// only in how a query's post-update gcd/multiple-count are obtained - an O(n)
// rescan from scratch, or two persistent SegmentTree instances point-updated in
// O(log n) and range-queried in O(log n), falling back to a scan only for the
// one case a range query alone cannot decide (see HasGoodSubsequenceByRangeQuery).
internal static class GoodSubsequenceQueriesSolution
{
    // The textbook answer: no persistent structure, every query rescans nums'
    // modulus-divided values from scratch in O(n) - "what you'd write without this
    // repo" (ARCHITECTURE.md 17.5).
    public static int CountGoodSubseqByBruteForce(int[] nums, int modulus, int[][] queries)
    {
        var divided = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            divided[i] = DivideByP(nums[i], modulus);
        }

        var answer = 0;

        foreach (var query in queries)
        {
            divided[query[0]] = DivideByP(query[1], modulus);

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

        return multiplesCount < divided.Length || HasRemovableIndex(divided);
    }

    // Composed: builds a GoodSubsequenceIndex once so every query's full-range
    // gcd and multiple-count are O(log n) SegmentTree range queries instead of an
    // O(n) rescan - the Query-with-a-custom-ICombineOperation extensibility point
    // DataStructures.SegmentTree.SegmentTree<Element,TOperation> exists for,
    // instantiated twice over (gcd, sum) the same way MinimumStabilityFactorOfArray
    // instantiates it once for LC 3605's window gcds.
    public static int CountGoodSubseqBySegmentTreeGcd(int[] nums, int modulus, int[][] queries)
    {
        var index = GoodSubsequenceIndex.Build(nums, modulus);

        return CountGoodSubseqBySegmentTreeGcd(index, queries);
    }

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
    // every element is a multiple of modulus (multiplesCount == length): only then
    // does "does removing some single element keep the gcd at 1" need asking, and
    // that still requires an O(n) scan over the maintained Divided array - the
    // same fallback the brute-force arm always pays, paid here only on that
    // narrower edge case instead of on every query.
    private static bool HasGoodSubsequenceByRangeQuery(GoodSubsequenceIndex index, int length)
    {
        var multiplesCount = index.MultipleCountTree.Query(0, length - 1);

        if (multiplesCount == 0 || index.GcdTree.Query(0, length - 1) != 1)
        {
            return false;
        }

        return multiplesCount < length || HasRemovableIndex(index.Divided);
    }

    // Whether some single index can be dropped from divided (every entry already
    // a multiple of modulus) while the rest still combine to a gcd of 1: a standard
    // prefix/suffix gcd sweep, shared by both strategies exactly the way LC
    // 3605's GcdOperation.Combine is - not itself a second strategy, just the one
    // subroutine both arms need for this one case.
    private static bool HasRemovableIndex(int[] divided)
    {
        var n = divided.Length;
        var prefix = new int[n + 1];
        var suffix = new int[n + 1];

        BuildPrefixCommonDivisors(prefix, divided);
        BuildSuffixCommonDivisors(suffix, divided);

        for (var i = 0; i < n; i++)
        {
            if (GcdOperation.Combine(prefix[i], suffix[i + 1]) == 1)
            {
                return true;
            }
        }

        return false;
    }

    // prefix[k] is the gcd of divided[0..k-1], with the empty prefix's Identity parked at
    // prefix[0] - so "everything before index i" is simply prefix[i], with no first-index
    // case for the sweep above to spell out.
    private static void BuildPrefixCommonDivisors(int[] prefix, int[] divided)
    {
        prefix[0] = GcdOperation.Identity;

        for (var i = 0; i < divided.Length; i++)
        {
            prefix[i + 1] = GcdOperation.Combine(prefix[i], divided[i]);
        }
    }

    // suffix[k] is the gcd of divided[k..n - 1], with the empty suffix's Identity parked at
    // suffix[n] - so "everything after index i" is simply suffix[i + 1], the mirror of the
    // prefix above.
    private static void BuildSuffixCommonDivisors(int[] suffix, int[] divided)
    {
        suffix[divided.Length] = GcdOperation.Identity;

        for (var i = divided.Length - 1; i >= 0; i--)
        {
            suffix[i] = GcdOperation.Combine(suffix[i + 1], divided[i]);
        }
    }

    // Dividing the modulus out of a value that is not a multiple of it is meaningless here:
    // 0 is already what "this element cannot join a modulus-divisible subsequence" looks
    // like to the rescan, the range queries and the removed-index sweep alike.
    private static int DivideByP(int value, int modulus)
    {
        if (value % modulus != 0)
        {
            return 0;
        }

        return value / modulus;
    }
}
