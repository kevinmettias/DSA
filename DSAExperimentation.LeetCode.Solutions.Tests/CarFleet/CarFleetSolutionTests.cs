using DSAExperimentation.LeetCode.Solutions.CarFleet;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CarFleet;

// Harness only. Both strategies are CarFleetSolution's - this file states its cases
// once and asserts each strategy against them, so a failure names the strategy that
// broke; the case here is cars at equal speeds, which never merge. The O(n^2)
// recompute-max-each-car arm used to live only in the benchmark, unasserted; it is a
// first-class tested strategy now. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class CarFleetSolutionTests
{
    public static TheoryData<int, int[], int[], int> Cases =>
        new()
        {
            // Equal speeds never close a gap, so no car ever merges.
            { 10, [1, 2, 3], [1, 1, 1], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountFleetsByRecomputeMaxEachCar_Cases_ReturnsFleetCount(
        int target, int[] position, int[] speed, int expected)
    {
        var fleets = CarFleetSolution.CountFleetsByRecomputeMaxEachCar(target, position, speed);

        Assert.Equal(expected, fleets);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountFleetsByMonotonicStackSweep_Cases_ReturnsFleetCount(
        int target, int[] position, int[] speed, int expected)
    {
        var fleets = CarFleetSolution.CountFleetsByMonotonicStackSweep(target, position, speed);

        Assert.Equal(expected, fleets);
    }

    // LeetCode example 1's cars, given out of position order: sorted nearest the target
    // first they are 10, 8, 5, 3 and 0, arriving at 2/2, 4/4, 7/1, 9/3 and 12/1.
    [Fact]
    public void ArrivalTimesNearestTargetFirst_UnsortedPositions_ListsEachCarsUnobstructedArrival()
    {
        var times = CarFleetSolution.ArrivalTimesNearestTargetFirst(12, [10, 8, 0, 5, 3], [2, 4, 1, 1, 3]);

        Assert.Equal([1.0, 1.0, 7.0, 3.0, 12.0], Enumerable.Range(0, times.Count).Select(times.Get));
    }
}
