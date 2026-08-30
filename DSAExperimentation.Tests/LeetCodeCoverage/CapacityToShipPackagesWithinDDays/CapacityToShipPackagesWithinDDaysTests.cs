using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CapacityToShipPackagesWithinDDays;

// LeetCode 1011. Capacity To Ship Packages Within D Days: "binary search on the
// answer" - the feasibility of a candidate capacity ("can every package ship
// within days, loading packages onto a day in order without exceeding capacity?")
// is monotone non-decreasing in capacity, so the minimum feasible capacity is the
// leftmost "true" in an implicit [false...false, true...true] sequence. Same
// FeasibleCapacitySequence + BinarySearch.LowerBound shape SplitArrayLargestSumTests
// already uses for LC 410 - that problem's "limit"/"k" are this one's
// "capacity"/"days" under a different name, same feasibility-monotonicity proof.
public sealed partial class CapacityToShipPackagesWithinDDaysTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5, 15)]
    [InlineData(new[] { 3, 2, 2, 4, 1, 4 }, 3, 6)]
    [InlineData(new[] { 1, 2, 3, 1, 1 }, 4, 3)]
    public void ShipWithinDays_LeetCodeExamples_ReturnsSmallestFeasibleCapacity(int[] weights, int days, int expected)
        => Assert.Equal(expected, ShipWithinDays(weights, days));

    private static int ShipWithinDays(int[] weights, int days)
    {
        var floor = weights.Max();
        var ceiling = weights.Sum();
        var sequence = new FeasibleCapacitySequence(weights, days, floor, ceiling);

        return floor + BinarySearch.LowerBound(sequence, true);
    }

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

    private readonly struct FeasibleCapacitySequence(int[] weights, int days, int floor, int ceiling)
        : IRandomAccessSequence<bool>
    {
        public int Length => ceiling - floor + 1;

        public bool Get(int index) => CanShipWithinDays(weights, days, floor + index);
    }
}
