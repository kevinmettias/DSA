namespace DSAExperimentation.LeetCode.GasStation;

// LeetCode 134. Gas Station: is there a starting station from which a car with
// an empty tank can complete a full circular route, given gas[i] available at
// each station and cost[i] to drive from it to the next? LeetCode guarantees
// the answer is unique when one exists, so both strategies return either that
// single valid start index or LeetCodeAnswer.None.
//
// The two strategies differ in how much of the route they re-walk: the brute
// force simulates a full lap from every candidate start, while the greedy pass
// walks the route once, advancing its candidate start the moment the running
// tank goes negative.
internal static class GasStationSolution
{
    // The textbook O(n^2) check: simulate a full lap from each candidate start,
    // stopping the first time the tank would go negative. Deliberately written
    // without this repo's primitives - it is the arm the greedy strategy below
    // has to justify itself against.
    public static int CanCompleteCircuitByBruteForceSimulation(int[] gas, int[] cost)
    {
        for (var start = 0; start < gas.Length; start++)
        {
            if (CanCompleteLapFrom(start, gas, cost))
            {
                return start;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static bool CanCompleteLapFrom(int start, int[] gas, int[] cost)
    {
        var tank = 0;

        for (var step = 0; step < gas.Length; step++)
        {
            var i = (start + step) % gas.Length;
            tank += gas[i] - cost[i];

            if (tank < 0)
            {
                return false;
            }
        }

        return true;
    }

    // A single O(n) pass: track the running debt since the last candidate reset
    // and the total surplus across the whole route. Whenever the tank would go
    // negative, no station up to and including here can be the start either -
    // each has an even smaller reachable surplus - so the candidate start
    // advances past it. A feasible start exists iff the total surplus is
    // non-negative, in which case the last-reset candidate is it.
    public static int CanCompleteCircuitByGreedyDebtReset(int[] gas, int[] cost)
    {
        var total = 0;
        var tank = 0;
        var start = 0;

        for (var i = 0; i < gas.Length; i++)
        {
            var delta = gas[i] - cost[i];
            total += delta;
            tank += delta;

            if (tank < 0)
            {
                start = i + 1;
                tank = 0;
            }
        }

        return total < 0 ? LeetCodeAnswer.None : start;
    }
}
