using RepoTimeStack = DSAExperimentation.DataStructures.Stack.Stack<double>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CarFleet;

// LeetCode 853. Car Fleet: sort cars by starting position descending (closest to
// target first), then a single left-to-right pass over this repo's own
// Stack<double> of fleet arrival times (DailyTemperatures/AsteroidCollision
// precedent for this repo's own Stack instead of the CLR's own
// System.Collections.Generic.Stack). Each car's own arrival time only ever gets
// compared against the current top - the slowest (largest) arrival time among
// every fleet already ahead of it - so the stack only ever grows: a car whose
// own time is <= the top merges into that fleet (never pushed), and a strictly
// slower car starts a new one, leaving the final stack height as the fleet
// count.
public sealed partial class CarFleetTests
{
    [Fact]
    public void CountFleets_ClassicExample_ReturnsThreeFleets()
    {
        int[] position = [10, 8, 0, 5, 3];
        int[] speed = [2, 4, 1, 1, 3];

        var fleets = CountFleets(target: 12, position, speed);

        Assert.Equal(3, fleets);
    }

    [Fact]
    public void CountFleets_EveryCarCatchesTheOneAhead_ReturnsOneFleet()
    {
        int[] position = [0, 2, 4];
        int[] speed = [4, 2, 1];

        var fleets = CountFleets(target: 100, position, speed);

        Assert.Equal(1, fleets);
    }

    [Fact]
    public void CountFleets_SameSpeedNeverCatchesUp_ReturnsOneFleetPerCar()
    {
        int[] position = [1, 2, 3];
        int[] speed = [1, 1, 1];

        var fleets = CountFleets(target: 10, position, speed);

        Assert.Equal(3, fleets);
    }

    private static int CountFleets(int target, int[] position, int[] speed)
    {
        var order = Enumerable.Range(0, position.Length)
            .OrderByDescending(i => position[i])
            .ToArray();

        var fleetArrivalTimes = new RepoTimeStack();

        foreach (var i in order)
        {
            var time = (double)(target - position[i]) / speed[i];

            if (!fleetArrivalTimes.TryPeek(out var slowestAhead) || time > slowestAhead)
            {
                fleetArrivalTimes.Push(time);
            }
        }

        return fleetArrivalTimes.Count;
    }
}
