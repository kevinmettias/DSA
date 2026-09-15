using DSAExperimentation.DataStructures.DynamicArray;
using RepoTimeStack = DSAExperimentation.DataStructures.Stack.Stack<double>;

namespace DSAExperimentation.LeetCode.CarFleet;

// LeetCode 853. Car Fleet: cars on a one-lane road never overtake, so a faster car
// that would reach the target before a slower car ahead of it instead bunches up
// behind it and the pair arrives together as one fleet. Count the fleets that reach
// the target.
//
// Both strategies start from the same derived input: each car's unobstructed arrival
// time (target - position) / speed, listed in order of starting position DESCENDING
// (closest to the target first). Sorting into that order is input preparation shared
// by both arms - the O(n log n) sort is not what the two strategies differ in - so
// the LeetCode-shaped overloads build it and hand it to a prepared-input overload
// (ARCHITECTURE.md §17.4), which is also what lets a benchmark charge the sort to
// [GlobalSetup].
//
// Given that order, a car forms its own new fleet exactly when its arrival time is
// strictly greater than every arrival time ahead of it: anything less-or-equal is
// caught by - and merges into - the slowest fleet already in front. So the whole
// problem is "count the running strict maxima", and the two strategies differ only
// in how they maintain that running maximum.
internal static class CarFleetSolution
{
    // The textbook answer: re-derive the "slowest fleet ahead" from scratch for every
    // car with a nested scan over everything ahead of it, O(n^2). Deliberately written
    // without this repo's primitives - it is the arm the composed solution below has
    // to justify itself against.
    public static int CountFleetsByRecomputeMaxEachCar(int target, int[] position, int[] speed)
    {
        var arrivalTimes = ArrivalTimesByPositionDescending(target, position, speed);
        return CountFleetsByRecomputeMaxEachCar(arrivalTimes);
    }

    public static int CountFleetsByRecomputeMaxEachCar(DynamicArray<double> arrivalTimesByPositionDescending)
    {
        var times = arrivalTimesByPositionDescending;
        var fleets = 0;

        for (var i = 0; i < times.Count; i++)
        {
            var slowestAhead = double.NegativeInfinity;

            for (var j = 0; j < i; j++)
            {
                if (times.Get(j) > slowestAhead)
                {
                    slowestAhead = times.Get(j);
                }
            }

            if (times.Get(i) > slowestAhead)
            {
                fleets++;
            }
        }

        return fleets;
    }

    // This repo's own Stack<double> carries the fleet arrival times
    // (DailyTemperatures/AsteroidCollision precedent for the repo's own Stack over the
    // CLR's). Each car is compared against the current top - the slowest arrival time
    // among every fleet already ahead of it - so the stack is monotonically increasing
    // and only ever grows: a car whose own time is <= the top merges into that fleet
    // and is never pushed, a strictly slower car starts a new one. The final stack
    // height is the fleet count, in one O(n) pass with no pop ever needed.
    public static int CountFleetsByMonotonicStackSweep(int target, int[] position, int[] speed)
    {
        var arrivalTimes = ArrivalTimesByPositionDescending(target, position, speed);
        return CountFleetsByMonotonicStackSweep(arrivalTimes);
    }

    public static int CountFleetsByMonotonicStackSweep(DynamicArray<double> arrivalTimesByPositionDescending)
    {
        var times = arrivalTimesByPositionDescending;
        var fleetArrivalTimes = new RepoTimeStack();

        for (var i = 0; i < times.Count; i++)
        {
            var time = times.Get(i);

            if (!fleetArrivalTimes.TryPeek(out var slowestAhead) || time > slowestAhead)
            {
                fleetArrivalTimes.Push(time);
            }
        }

        return fleetArrivalTimes.Count;
    }

    // Input preparation shared by both strategies: LeetCode's (target, position, speed)
    // triple reduced to the only thing either strategy reads, in the only order either
    // strategy can read it in.
    public static DynamicArray<double> ArrivalTimesByPositionDescending(
        int target, int[] position, int[] speed)
    {
        var times = new DynamicArray<double>();

        foreach (var i in Enumerable.Range(0, position.Length).OrderByDescending(i => position[i]))
        {
            times.Add((double)(target - position[i]) / speed[i]);
        }

        return times;
    }
}
