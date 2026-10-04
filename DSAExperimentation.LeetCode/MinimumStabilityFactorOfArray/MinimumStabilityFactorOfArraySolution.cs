using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MinimumStabilityFactorOfArray;

// LeetCode 3605. Minimum Stability Factor of Array: a subarray is stable iff its
// gcd is >= 2, and the stability factor is the length of the longest stable
// subarray. Up to maxC elements may be changed to ANY integer, and the cheapest
// possible change is always to overwrite an element with 1 - gcd(x, 1) = 1 for any
// x, so a 1 at position i makes every subarray containing i unstable, both as an
// interior element and as its own length-1 subarray. Placing 1s therefore
// dominates every other choice, and the problem reduces to choosing up to maxC
// positions to "cut".
//
// gcd of a sub-range is always a multiple of - so never smaller than - the gcd of
// any range it's nested inside (the outer range's gcd already divides every
// element, hence divides the inner range's elements too), so if any subarray of
// length > L is stable, every length-(L+1) window inside it is also stable. That
// collapses "does a stable subarray of length > L exist" down to a purely local
// question - "does any length-(L+1) window have gcd >= 2" - which is exactly
// where the classic minimum-points-to-stab-every-interval greedy applies: sweep
// windows left to right by increasing start (equivalently, increasing right
// edge), and whenever a dangerous window isn't already covered by the last cut
// placed, cut its rightmost position (covering as many later windows as
// possible). The number of cuts that greedy needs is the true minimum for that L
// (interval-stabbing's standard optimality argument), so "minimum stability
// factor" is the smallest L for which that minimum is <= maxC - a monotonic
// predicate in L (any placement that already caps the longest stable run at L
// also caps it at any L' >= L), so binary search applies directly.
internal static class MinimumStabilityFactorOfArraySolution
{
    // The gcd of no values - GcdOperation's identity, since gcd(0, x) = x - where a running gcd
    // starts folding from.
    private static int EmptyGcd => GcdOperation<int>.Identity;

    // Baseline: recomputes each window's gcd from scratch by scanning its L+1
    // elements (with an early exit once the running gcd hits 1) - "what you'd
    // write without this repo" (ARCHITECTURE.md 17.5), no range-query structure at
    // all.
    public static int MinStabilityByBruteForceGcdScan(int[] nums, int maxC)
    {
        var (low, high) = (0, nums.Length);

        while (low < high)
        {
            var mid = low + ((high - low) / 2);

            if (CutsNeededByScan(nums, mid) <= maxC)
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    private static int CutsNeededByScan(int[] nums, int windowLength)
    {
        var cuts = 0;
        var lastCut = -1;

        for (var start = 0; start + windowLength < nums.Length; start++)
        {
            if (start <= lastCut)
            {
                continue;
            }

            if (WindowGcd(nums, start, windowLength) >= 2)
            {
                lastCut = start + windowLength;
                cuts++;
            }
        }

        return cuts;
    }

    private static int WindowGcd(int[] nums, int start, int windowLength)
    {
        var gcd = EmptyGcd;

        for (var offset = 0; offset <= windowLength; offset++)
        {
            gcd = Gcd(gcd, nums[start + offset]);

            if (gcd == 1)
            {
                return 1;
            }
        }

        return gcd;
    }

    // Composed: builds a SegmentTree<int, GcdOperation<int>> once so every window's gcd
    // is an O(log n) range query instead of an O(windowLength) rescan - the
    // Query-with-a-custom-ICombineOperation extensibility point
    // DataStructures.SegmentTree.SegmentTree<Element,TOperation> exists for - and hands
    // the cut-budget rule, with every factor from 0 to n, to this repo's own
    // MonotonePredicateSearch.FirstTrue. A factor of n always holds - a window of n + 1
    // elements does not fit in the array, so nothing needs a cut - so the search always
    // lands inside the range.
    public static int MinStabilityBySegmentTreeGcd(int[] nums, int maxC) =>
        MinStabilityBySegmentTreeGcd(new SegmentTree<int, GcdOperation<int>>(nums), maxC);

    public static int MinStabilityBySegmentTreeGcd(SegmentTree<int, GcdOperation<int>> gcdTree, int maxC) =>
        MonotonePredicateSearch.FirstTrue(0, gcdTree.Count, new FactorReachableWithinChanges(gcdTree, maxC));

    private static int CutsNeededByRangeQuery(SegmentTree<int, GcdOperation<int>> gcdTree, int windowLength)
    {
        var cuts = 0;
        var lastCut = -1;

        for (var start = 0; start + windowLength < gcdTree.Count; start++)
        {
            if (start <= lastCut)
            {
                continue;
            }

            if (gcdTree.Query(start, start + windowLength) >= 2)
            {
                lastCut = start + windowLength;
                cuts++;
            }
        }

        return cuts;
    }

    // IsSatisfiedBy(factor) is "at most maxC cuts cap every stable run at this length" - false
    // below the answer and true from there on, since a placement that caps runs at L
    // also caps them at any longer L. A rule for this problem alone: the stabbing greedy
    // is LC 3605's own content.
    private readonly struct FactorReachableWithinChanges(SegmentTree<int, GcdOperation<int>> gcdTree, int maxC)
        : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int factor) => CutsNeededByRangeQuery(gcdTree, factor) <= maxC;
    }

    // Euclid's algorithm, written out here rather than taken from the core GcdOperation: the
    // brute-force baseline reaches it through WindowGcd, and a baseline arm composes nothing of this
    // repository's own (ARCHITECTURE section 17.5).
    private static int Gcd(int left, int right)
    {
        while (right != 0)
        {
            (left, right) = (right, left % right);
        }

        return left;
    }
}
