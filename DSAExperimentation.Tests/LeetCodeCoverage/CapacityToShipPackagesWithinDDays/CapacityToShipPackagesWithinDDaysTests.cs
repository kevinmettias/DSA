using DSAExperimentation.LeetCode.CapacityToShipPackagesWithinDDays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CapacityToShipPackagesWithinDDays;

// Harness only. Both strategies are CapacityToShipPackagesWithinDDaysSolution's -
// the hand-rolled lo/hi bisection that used to live only in the benchmark's baseline
// arm, and the BinarySearch.LowerBound walk over the feasibility sequence the test
// used to inline.
public sealed class CapacityToShipPackagesWithinDDaysTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            // LC examples 1-3.
            { [1, 2, 3, 4, 5, 6, 7, 8, 9, 10], 5, 15 },
            { [3, 2, 2, 4, 1, 4], 3, 6 },
            { [1, 2, 3, 1, 1], 4, 3 },

            // One package, one day: the package itself.
            { [7], 1, 7 },

            // A single day must carry everything, so the answer is the total weight.
            { [1, 2, 3, 4, 5, 6, 7, 8, 9, 10], 1, 55 },

            // As many days as packages, so the answer is the heaviest package.
            { [1, 2, 3, 4, 5, 6, 7, 8, 9, 10], 10, 10 },

            // More days than packages: still bounded below by the heaviest package.
            { [3, 2, 2, 4, 1, 4], 100, 4 },

            // Equal weights split evenly across the days.
            { [5, 5, 5, 5], 2, 10 },

            // Packing order matters: 500 must ship alone, so 3 days need capacity 500
            // even though the total is only 700.
            { [500, 100, 100], 3, 500 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShipWithinDaysByManualBisection_LeetCodeExamples_ReturnsSmallestFeasibleCapacity(
        int[] weights, int days, int expected)
    {
        var capacity =
            CapacityToShipPackagesWithinDDaysSolution.ShipWithinDaysByManualBisection(weights, days);

        Assert.Equal(expected, capacity);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShipWithinDaysBySequenceLowerBound_LeetCodeExamples_ReturnsSmallestFeasibleCapacity(
        int[] weights, int days, int expected)
    {
        var capacity =
            CapacityToShipPackagesWithinDDaysSolution.ShipWithinDaysBySequenceLowerBound(weights, days);

        Assert.Equal(expected, capacity);
    }
}
