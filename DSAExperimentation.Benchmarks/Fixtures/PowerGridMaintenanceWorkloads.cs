namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3607 - stations 2..stationCount each connect to a
// random earlier station (guaranteeing one connected grid spanning all of them) plus a
// couple of extra random edges for density, and queries alternate a station going
// offline with a maintenance check on a random station, so most checks eventually
// land on an offline one and have to search their grid for a replacement.
internal static class PowerGridMaintenanceWorkloads
{
    private const int ExtraConnectionsPerStation = 2;

    public static int[][] BuildConnections(int stationCount, int seed)
    {
        var random = new Random(seed);
        var connections = new List<int[]>();

        for (var station = 2; station <= stationCount; station++)
        {
            connections.Add([random.Next(1, station), station]);

            for (var extra = 0; extra < ExtraConnectionsPerStation; extra++)
            {
                var other = random.Next(1, stationCount + 1);

                if (other != station)
                {
                    connections.Add([station, other]);
                }
            }
        }

        return [.. connections];
    }

    public static int[][] BuildQueries(int stationCount, int queryCount, int seed)
    {
        var random = new Random(seed);
        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            var isOfflineQuery = i % 3 == 0;
            var queryKind = isOfflineQuery ? 2 : 1;
            queries[i] = [queryKind, random.Next(1, stationCount + 1)];
        }

        return queries;
    }
}
