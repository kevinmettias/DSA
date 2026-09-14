namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing only (ARCHITECTURE.md 17.7) for LC 1928: how large a road map to
// measure, and how tight a minute budget to allow. The map itself is
// MinimumCostToReachDestinationInTime's own RoadNetwork/TimeCityGraph.
//
// The shape is a chain in which every city reaches the next one in a minute and
// the one after that in two, the same "two roads out of every city" branching
// NumberOfRestrictedPathsFromFirstToLastNode's own workload uses to force
// genuinely Fibonacci-many distinct routes. The budget is set to the city count,
// which is exactly what a straight run to the end costs: roads are two-way, so a
// route that ever doubles back spends more minutes than it has, and the naive walk
// has to discover that for itself rather than being handed a one-way map.
internal static class TimedRoadWorkloads
{
    // Fees cycle through 1..7 so no single route is uniformly cheapest and the
    // search cannot be won by following one dominant road.
    private const int FeeAlphabet = 7;

    private const int LongStepSize = 2;

    // LeetCode's own edges shape: one [from, to, minutes] row per road.
    public static int[][] BuildStepChain(int lastCity)
    {
        var roads = new List<int[]>();

        for (var city = 0; city < lastCity; city++)
        {
            roads.Add([city, city + 1, 1]);

            if (city + LongStepSize <= lastCity)
            {
                roads.Add([city, city + LongStepSize, LongStepSize]);
            }
        }

        return [.. roads];
    }

    public static int[] BuildCyclingFees(int lastCity)
    {
        var fees = new int[lastCity + 1];

        for (var city = 0; city <= lastCity; city++)
        {
            fees[city] = (city % FeeAlphabet) + 1;
        }

        return fees;
    }

    // Exactly what a straight run to the last city costs, so every minute of the
    // budget is spent making progress.
    public static int BudgetFor(int lastCity) => lastCity;
}
