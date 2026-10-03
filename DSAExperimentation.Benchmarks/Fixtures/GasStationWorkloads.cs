namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 134 - seeded station amounts arranged so the circuit
// can be completed from the LAST station and from no other. That makes the answer exist
// and be unique, as LC 134 guarantees, and sit as late as it can, so the brute-force
// baseline simulates a failing lap from every earlier start before it finds it.
//
// Every station first draws its gas and then its cost from 1..9. With d = gas - cost and
// P(k) the running sum of d through station k, a start s completes the circuit exactly
// when the total is non-negative and P(s - 1) - zero for s = 0 - is the lowest running
// sum. So the second-to-last station's cost is raised until P(n - 2) sits strictly below
// zero and below every earlier running sum, and the last station's amounts are then
// evened up so the total is exactly zero: the lowest running sum is P(n - 2) alone, and
// only station n - 1 can start. Every amount stays inside LC 134's 0..10^4.
internal static class GasStationWorkloads
{
    private const int MaxStationAmountExclusive = 10;

    // The two stations the construction adjusts, counted back from the end.
    private const int StationsAfterTheDraws = 2;

    public static (int[] Gas, int[] Cost) Build(int length, int seed)
    {
        var random = new Random(seed);
        var gas = new int[length];
        var cost = new int[length];

        // One draw for gas and one for cost per station, in that order, off one generator.
        for (var i = 0; i < length; i++)
        {
            gas[i] = random.Next(1, MaxStationAmountExclusive);
            cost[i] = random.Next(1, MaxStationAmountExclusive);
        }

        SinkTheSecondToLastRunningSum(gas, cost);
        EvenUpTheTotal(gas, cost);

        return (gas, cost);
    }

    // Raises cost[n - 2] until P(n - 2) is one below the lowest of zero and every earlier
    // running sum.
    private static void SinkTheSecondToLastRunningSum(int[] gas, int[] cost)
    {
        var secondToLast = gas.Length - StationsAfterTheDraws;
        var running = 0;
        var lowest = 0;

        for (var i = 0; i < secondToLast; i++)
        {
            running += gas[i] - cost[i];
            lowest = Math.Min(lowest, running);
        }

        var target = lowest - 1;
        var reached = running + gas[secondToLast] - cost[secondToLast];
        cost[secondToLast] += Math.Max(reached - target, 0);
    }

    // Moves the last station's gas or cost until the whole circuit nets to exactly zero.
    private static void EvenUpTheTotal(int[] gas, int[] cost)
    {
        var total = gas.Sum() - cost.Sum();
        var isShort = total < 0;

        if (isShort)
        {
            gas[^1] -= total;
        }
        else
        {
            cost[^1] += total;
        }
    }
}
