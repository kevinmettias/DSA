namespace DSAExperimentation.LeetCode.Solutions.GasStation;

// LC 134's promise about its input: when a station can start a full lap, it is the only one. LeetCode's
// judge never poses an input that breaks it. Of several stations that could start, the simulation
// answers the first and the debt reset the last. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class GasStationPromise
{
    public static bool IsKeptBy(int[] gas, int[] cost) => CountStartsCompletingALap(gas, cost) <= 1;

    // A lap from a station runs dry nowhere when the tank, counted from an empty start at station 0, is
    // never lower later in the lap than where it began; past the last station the lap wraps, carrying
    // the whole lap's surplus.
    private static int CountStartsCompletingALap(int[] gas, int[] cost)
    {
        var tank = TankOnEachArrival(gas, cost);
        var lowestFrom = LowestFromEachArrival(tank);
        var stationCount = gas.Length;
        var starts = 0;
        var lowestBefore = long.MaxValue;

        for (var start = 0; start < stationCount; start++)
        {
            var reachesTheLast = lowestFrom[start + 1] >= tank[start];
            var wrapsBack = start == 0 || tank[stationCount] + lowestBefore >= tank[start];
            starts += reachesTheLast && wrapsBack ? 1 : 0;
            lowestBefore = Math.Min(lowestBefore, tank[start + 1]);
        }

        return starts;
    }

    // tank[i]: what is left on arriving at station i from an empty start at station 0, with
    // tank[n] the whole lap's surplus.
    private static long[] TankOnEachArrival(int[] gas, int[] cost)
    {
        var tank = new long[gas.Length + 1];

        for (var station = 0; station < gas.Length; station++)
        {
            tank[station + 1] = tank[station] + gas[station] - cost[station];
        }

        return tank;
    }

    // lowestFrom[i]: the lowest of tank[i] to tank[n], for each i from 1.
    private static long[] LowestFromEachArrival(long[] tank)
    {
        var last = tank.Length - 1;
        var lowestFrom = new long[tank.Length];
        lowestFrom[last] = tank[last];

        for (var arrival = last - 1; arrival > 0; arrival--)
        {
            lowestFrom[arrival] = Math.Min(tank[arrival], lowestFrom[arrival + 1]);
        }

        return lowestFrom;
    }
}
