using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.CountBowlSubarrays;

// LeetCode 3676. Count Bowl Subarrays: nums has distinct elements, and
// nums[l..r] (r - l + 1 >= 3) is a "bowl" when min(nums[l], nums[r]) is strictly
// greater than max(nums[l+1..r-1]).
//
// Every bowl is pinned by exactly one of its two ends - the one holding the
// SMALLER value, since the bowl condition is really "min(ends) > interior max".
// Whichever end is smaller, the interior can never reach as far as that end's own
// next-greater neighbour (in whichever direction), or the interior max would meet
// or beat it. So [l, r] is a bowl iff either r is l's next strictly-greater index
// (nums[l] < nums[r]) or l is r's previous strictly-greater index (nums[r] <
// nums[l]) - two boundary arrays NearestBoundary's one-pass sweep produces, the same
// sweep ApplyOperationsToMaximizeScore's stack arm reads its boundaries from.
internal static class CountBowlSubarraysSolution
{
    private const int NoGreaterElement = -1;

    // Textbook reading of the definition: for every (l, r) pair of length >= 3,
    // track the interior max with a running value as r grows - still checks every
    // pair directly against the definition, with no insight about which ones can
    // possibly qualify.
    public static int CountBowlsByPairScan(int[] nums)
    {
        var count = 0;

        for (var l = 0; l < nums.Length; l++)
        {
            var interiorMax = int.MinValue;

            for (var r = l + 1; r < nums.Length; r++)
            {
                if (r - l >= 2 && Math.Min(nums[l], nums[r]) > interiorMax)
                {
                    count++;
                }

                interiorMax = Math.Max(interiorMax, nums[r]);
            }
        }

        return count;
    }

    // Composed: only the 2n (index, next/previous strictly-greater index) pairs a
    // monotonic sweep produces can ever be a bowl's pinning end, so counting those
    // directly replaces the O(n^2) pair scan with two O(n) sweeps. The two relations are
    // the pair every "nearest greater boundary" count in this repo pairs up: the rightward
    // one strict so an equal neighbour cannot resolve an index, the leftward one or-equal
    // so that index resolves against it instead - which on this problem's distinct values
    // is the strictly-greater index either way.
    public static int CountBowlsByMonotonicStack(int[] nums)
    {
        var nextGreater = NearestBoundary.GreaterToTheRight(nums, NoGreaterElement);
        var previousGreater = NearestBoundary.GreaterOrEqualToTheLeft(nums, NoGreaterElement);
        var count = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if (nextGreater[i] != NoGreaterElement && nextGreater[i] - i >= 2)
            {
                count++;
            }

            if (previousGreater[i] != NoGreaterElement && i - previousGreater[i] >= 2)
            {
                count++;
            }
        }

        return count;
    }
}
