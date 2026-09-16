using DSAExperimentation.LeetCode.CarFleet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CarFleet;

// Harness only. Both strategies are CarFleetSolution's - this file just states
// LeetCode's published examples once and asserts each strategy against them, so a
// failure names the strategy that broke. The O(n^2) recompute-max-each-car arm used
// to live only in the benchmark, unasserted; it is a first-class tested strategy now.
public sealed class CarFleetTests
{
    public static TheoryData<int, int[], int[], int> Examples =>
        new()
        {
            // LeetCode example 1: the [0,3] and [5,8] pairs each merge, [10] arrives alone.
            { 12, [10, 8, 0, 5, 3], [2, 4, 1, 1, 3], 3 },

            // LeetCode example 2: a lone car is a fleet of one.
            { 10, [3], [3], 1 },

            // LeetCode example 3 / every car catches the one ahead of it.
            { 100, [0, 2, 4], [4, 2, 1], 1 },

            // Equal speeds never close a gap, so no car ever merges.
            { 10, [1, 2, 3], [1, 1, 1], 3 },

            // Cars given in an order unrelated to position, to prove the sort is real.
            { 10, [6, 8], [3, 2], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountFleetsByRecomputeMaxEachCar_LeetCodeExamples_ReturnsFleetCount(
        int target, int[] position, int[] speed, int expected)
    {
        var fleets = CarFleetSolution.CountFleetsByRecomputeMaxEachCar(target, position, speed);

        Assert.Equal(expected, fleets);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountFleetsByMonotonicStackSweep_LeetCodeExamples_ReturnsFleetCount(
        int target, int[] position, int[] speed, int expected)
    {
        var fleets = CarFleetSolution.CountFleetsByMonotonicStackSweep(target, position, speed);

        Assert.Equal(expected, fleets);
    }
}
