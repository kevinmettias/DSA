using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumXorSumOfTwoArrays;

// LeetCode 1879. Minimum XOR Sum of Two Arrays: rearrange nums2 so that the sum of
// nums1[i] XOR nums2[i] is as small as possible. Both arrays are the same length, so
// this is a full permutation - every nums1 element takes a distinct nums2 element.
//
// Both strategies are the same recurrence, walking nums1 left to right and carrying a
// bitmask of which nums2 elements an earlier position already claimed:
// f(index, mask) = min over every unclaimed j of (nums1[index] ^ nums2[j]) +
// f(index + 1, mask | 1 << j). Constraints cap the length at 14, which is what makes
// the mask fit in an int.
//
// The arms differ only in whether that recurrence remembers anything: many different
// assignment orderings reach the identical "these nums2 elements are already used"
// state, and only the index and the mask matter from there on - the same collapse
// MinimumCostToConnectTwoGroupsOfPointsSolution relies on for LC 1595's near-twin
// (index, connected-mask) assignment recurrence, here XOR-ing instead of adding an
// edge cost and with no unmatched-element fallback to make up, since the matching is
// total.
internal static class MinimumXorSumOfTwoArraysSolution
{
    // The textbook answer: plain exponential recursion, no cache, nothing from this
    // repo in its internals. It is the arm the memoized strategy below has to justify
    // itself against, and putting it here is what finally gets it asserted.
    public static int MinimumXorSumByBruteForceRecursion(int[] nums1, int[] nums2) =>
        CheapestFromScratch(nums1, nums2, 0, 0);

    private static int CheapestFromScratch(int[] nums1, int[] nums2, int index, int mask)
    {
        if (index == nums1.Length)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var j = 0; j < nums2.Length; j++)
        {
            if ((mask & (1 << j)) != 0)
            {
                continue;
            }

            var candidate = (nums1[index] ^ nums2[j]) +
                CheapestFromScratch(nums1, nums2, index + 1, mask | (1 << j));
            best = Math.Min(best, candidate);
        }

        return best;
    }

    // The same recurrence routed through this repo's own Memoizer, keyed on the tuple
    // state (index, claimedMask), so each reachable state is evaluated once rather
    // than once per ordering that reaches it.
    public static int MinimumXorSumByMemoizedBitmask(int[] nums1, int[] nums2) =>
        Memoizer.Memoize<(int Index, int Mask), int>(
            (0, 0), (state, cheapestFor) => CheapestFor(nums1, nums2, state, cheapestFor));

    private static int CheapestFor(
        int[] nums1,
        int[] nums2,
        (int Index, int Mask) state,
        Func<(int Index, int Mask), int> cheapestFor)
    {
        var (index, mask) = state;

        if (index == nums1.Length)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var j = 0; j < nums2.Length; j++)
        {
            if ((mask & (1 << j)) != 0)
            {
                continue;
            }

            var candidate = (nums1[index] ^ nums2[j]) + cheapestFor((index + 1, mask | (1 << j)));
            best = Math.Min(best, candidate);
        }

        return best;
    }
}
