using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MergeTripletsToFormTargetTriplet;

// LeetCode 1899. Merge Triplets to Form Target Triplet: a merge replaces two
// triplets with their component-wise max, so the set of reachable triplets is
// exactly "component-wise max over some non-empty subset of the input".
//
// The two strategies answer that from opposite ends. The baseline enumerates the
// subsets literally; the linear scan uses the observation that an incompatible
// triplet (one that exceeds target in any coordinate) can never appear in a subset
// whose max equals target, while a compatible one can always be merged in for
// free - so the whole question collapses to "does some compatible triplet hit
// target[i] exactly, for every coordinate i", tracked with this repo's own
// Set<int> over the matched coordinate indices, the same membership-tracking role
// ContainsDuplicate gives it.
internal static class MergeTripletsToFormTargetTripletSolution
{
    private const int TripletDimension = 3;

    // The textbook answer without this repo: try all 2^n - 1 non-empty subsets,
    // take each one's component-wise max, and compare it to target. Pure BCL, and
    // exponential - it is the arm the linear scan below has to justify itself
    // against.
    public static bool CanFormTargetByBruteForceSubsets(int[][] triplets, int[] target)
    {
        for (var mask = 1; mask < 1 << triplets.Length; mask++)
        {
            if (CanSubsetMergeToTarget(triplets, target, mask))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CanSubsetMergeToTarget(int[][] triplets, int[] target, int mask)
    {
        Span<int> merged = stackalloc int[TripletDimension];

        for (var i = 0; i < triplets.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                MergeInto(merged, triplets[i]);
            }
        }

        return IsMatch(merged, target);
    }

    private static void MergeInto(Span<int> merged, int[] triplet)
    {
        for (var i = 0; i < TripletDimension; i++)
        {
            merged[i] = Math.Max(merged[i], triplet[i]);
        }
    }

    private static bool IsMatch(ReadOnlySpan<int> merged, int[] target)
    {
        for (var i = 0; i < TripletDimension; i++)
        {
            if (merged[i] != target[i])
            {
                return false;
            }
        }

        return true;
    }

    // One O(n) pass: skip every triplet that overshoots target somewhere, and
    // record which coordinates the surviving ones match exactly. All three matched
    // means merging exactly those triplets yields target.
    public static bool CanFormTargetBySetTrackedLinearScan(int[][] triplets, int[] target)
    {
        var matched = new Set<int>();

        foreach (var triplet in triplets)
        {
            if (IsIncompatible(triplet, target))
            {
                continue;
            }

            for (var i = 0; i < TripletDimension; i++)
            {
                if (triplet[i] == target[i])
                {
                    matched.TryAdd(i);
                }
            }
        }

        return matched.Count == TripletDimension;
    }

    private static bool IsIncompatible(int[] triplet, int[] target)
    {
        for (var i = 0; i < TripletDimension; i++)
        {
            if (triplet[i] > target[i])
            {
                return true;
            }
        }

        return false;
    }
}
