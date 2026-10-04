using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.CapacityToShipPackagesWithinDDays;

// LeetCode 1011. Capacity To Ship Packages Within D Days: the least ship capacity
// that still clears every package within days, loading packages onto a day in the
// order given.
//
// "Can a ship of this capacity finish within days?" is monotone - once a capacity
// works, every larger one works too - so the answer is the leftmost true in an
// implicit [false...false, true...true] sequence over capacities in
// [max(weights), sum(weights)]. The baseline bisects that range with a hand-rolled
// lo/hi loop; the composed strategy states the same predicate as an
// IMonotonePredicate<int> and hands it, with the capacity range, to this repo's own
// MonotonePredicateSearch.FirstTrue - the same "binary search on the answer" shape
// KokoEatingBananas and SplitArrayLargestSum use, where this problem's
// "capacity"/"days" are that problem's "limit"/"k" under a different name.
internal static class CapacityToShipPackagesWithinDDaysSolution
{

    // The textbook answer: a hand-written bisection over the capacity range, BCL-only.
    public static int ShipWithinDaysByManualBisection(int[] weights, int days)
    {
        var low = weights.Max();
        var high = weights.Sum();

        while (low < high)
        {
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

            if (CanShipWithinDays(weights, days, mid))
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

    // The feasibility predicate both strategies ask about a candidate capacity: fill
    // the current day until the next package would overflow it, then start a new day.
    // Monotone in capacity, which is what makes bisecting it valid.
    private static bool CanShipWithinDays(int[] weights, int days, int capacity)
    {
        var daysNeeded = 1;
        var currentLoad = 0;

        foreach (var weight in weights)
        {
            if (currentLoad + weight > capacity)
            {
                daysNeeded++;
                currentLoad = 0;
            }

            currentLoad += weight;
        }

        return daysNeeded <= days;
    }

    // This repo's own MonotonePredicateSearch over the capacity range: the capacities
    // are never materialized, each probe just replays the loading walk, and the first
    // capacity that clears every package is the answer.
    public static int ShipWithinDaysByPredicateSearch(int[] weights, int days) =>
        MonotonePredicateSearch.FirstTrue(weights.Max(), weights.Sum(), new ShipsWithinDays(weights, days));

    // IsSatisfiedBy(capacity) is "a ship of this capacity clears every package within days" -
    // false up to the answer and true from there on, the monotonicity
    // MonotonePredicateSearch assumes but never checks. A rule for this problem alone:
    // the loading rule is LC 1011's own content.
    private readonly struct ShipsWithinDays(int[] weights, int days) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int capacity) => CanShipWithinDays(weights, days, capacity);
    }
}
