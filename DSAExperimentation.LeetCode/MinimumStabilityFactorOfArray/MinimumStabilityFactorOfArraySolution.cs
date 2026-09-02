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
        var gcd = GcdOperation.Identity;

        for (var offset = 0; offset <= windowLength; offset++)
        {
            gcd = GcdOperation.Combine(gcd, nums[start + offset]);

            if (gcd == 1)
            {
                return 1;
            }
        }

        return gcd;
    }

    // Composed: builds a SegmentTree<int,GcdOperation> once so every window's gcd
    // is an O(log n) range query instead of an O(windowLength) rescan - the
    // Query-with-a-custom-ICombineOperation extensibility point
    // DataStructures.SegmentTree.SegmentTree<Element,TOperation> exists for.
    public static int MinStabilityBySegmentTreeGcd(int[] nums, int maxC) =>
        MinStabilityBySegmentTreeGcd(new SegmentTree<int, GcdOperation>(nums), maxC);

    public static int MinStabilityBySegmentTreeGcd(SegmentTree<int, GcdOperation> gcdTree, int maxC)
    {
        var (low, high) = (0, gcdTree.Count);

        while (low < high)
        {
            var mid = low + ((high - low) / 2);

            if (CutsNeededByRangeQuery(gcdTree, mid) <= maxC)
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

    private static int CutsNeededByRangeQuery(SegmentTree<int, GcdOperation> gcdTree, int windowLength)
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
}
