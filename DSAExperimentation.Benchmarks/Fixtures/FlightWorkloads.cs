namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 787 - how large a flight network to measure and
// how deep a stop bound to allow, which are measurement decisions. The flight
// network itself is CheapestFlightsWithinKStops' own FlightNetwork/FlightStateGraph.
internal static class FlightWorkloads
{
    // Lower bound on the K-stop count so small city counts still get a
    // non-trivial search depth.
    private const int MinStops = 2;

    // The city count is divided by this to derive K, scaling the stop bound with
    // graph size so the naive side's exponential blowup is actually exercised at
    // the larger size rather than trivialized.
    private const int StopScaleFactor = 6;

    private const int MaxFlightPrice = 100;

    // Extra randomly-targeted flights added per city, on top of its guaranteed back edge.
    private const int ExtraFlightsPerCity = 2;

    public static int StopBound(int cityCount) => Math.Max(MinStops, cityCount / StopScaleFactor);

    // LeetCode's own flights shape: one [from, to, price] row per flight. A back
    // edge per city guarantees reachability from city 0, then a couple of extra
    // random flights per city give the search real branching density. LC 787 allows
    // at most one flight between two cities - its n * (n - 1) / 2 cap on the flight
    // count counts unordered pairs - so an extra flight joining an already-linked
    // pair, in either direction, is dropped.
    public static int[][] BuildFlights(int cityCount, int seed)
    {
        var random = new Random(seed);
        var flights = new List<int[]>();

        AddBackFlights(cityCount, random, flights);
        AddBranchingFlights(cityCount, random, flights);

        return [.. flights];
    }

    private static void AddBackFlights(int cityCount, Random random, List<int[]> flights)
    {
        for (var city = 1; city < cityCount; city++)
        {
            flights.Add([random.Next(city), city, random.Next(1, MaxFlightPrice)]);
        }
    }

    // A self-loop is skipped before its price is drawn and a repeated pair after, so
    // dropping a flight never moves a later flight's draws.
    private static void AddBranchingFlights(int cityCount, Random random, List<int[]> flights)
    {
        var linkedPairs = flights.Select(flight => CityPair(flight[0], flight[1])).ToHashSet();

        for (var city = 0; city < cityCount; city++)
        {
            for (var extra = 0; extra < ExtraFlightsPerCity; extra++)
            {
                var target = random.Next(cityCount);
                if (target == city)
                {
                    continue;
                }

                int[] flight = [city, target, random.Next(1, MaxFlightPrice)];
                var pair = CityPair(city, target);
                if (!linkedPairs.Add(pair))
                {
                    continue;
                }

                flights.Add(flight);
            }
        }
    }

    // The two cities a flight joins, whichever way it flies.
    private static (int Lower, int Higher) CityPair(int from, int to) => (Math.Min(from, to), Math.Max(from, to));
}
