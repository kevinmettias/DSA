using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

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
// IRandomAccessSequence<bool> computed on demand and hands it to this repo's own
// BinarySearch.LowerBound - the same "binary search on the answer" shape
// KokoEatingBananas and SplitArrayLargestSum use, where this problem's
// "capacity"/"days" are that problem's "limit"/"k" under a different name.
internal static class CapacityToShipPackagesWithinDDaysSolution
{
    private const int MidpointDivisor = 2;

    // The textbook answer: a hand-written bisection over the capacity range, BCL-only.
    public static int ShipWithinDaysByManualBisection(int[] weights, int days)
    {
        var low = weights.Max();
        var high = weights.Sum();

        while (low < high)
        {
            var mid = low + ((high - low) / MidpointDivisor);

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

    // This repo's own BinarySearch.LowerBound over the feasibility sequence: the
    // capacities are never materialized, each probe just replays the loading walk,
    // and the leftmost feasible index maps back to its capacity.
    public static int ShipWithinDaysBySequenceLowerBound(int[] weights, int days)
    {
        var floor = weights.Max();
        var ceiling = weights.Sum();
        var sequence = new FeasibleCapacitySequence(weights, days, floor, ceiling);

        return floor + BinarySearch.LowerBound<bool, FeasibleCapacitySequence>(sequence, true);
    }

    // Get(index) is "a ship of capacity floor + index clears every package within
    // days" - false up to the answer and true from there on, the monotonicity
    // BinarySearch.LowerBound assumes but never checks. A witness for this problem
    // alone: the loading rule is LC 1011's own content, not a general
    // monotone-predicate shape.
    private readonly struct FeasibleCapacitySequence(int[] weights, int days, int floor, int ceiling)
        : IRandomAccessSequence<bool>
    {
        public int Length => ceiling - floor + 1;

        public bool Get(int index) => CanShipWithinDays(weights, days, floor + index);
    }
}
