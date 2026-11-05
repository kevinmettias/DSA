namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 547 - a random symmetric adjacency matrix in which
// a city expects the same number of neighbours at every size: each pair of cities is
// joined with probability ExpectedNeighbourCount / (cityCount - 1). Two neighbours is
// under ln n, the mean degree at which G(n,p) becomes connected, at both of the
// benchmark's sizes (3.9 at 50 cities, 5.3 at 200), so a draw forms several provinces,
// one large and the rest small, rather than collapsing into one dense component.
internal static class NumberOfProvincesWorkloads
{
    private const int ExpectedNeighbourCount = 2;

    public static int[][] BuildAdjacencyMatrix(int cityCount, int seed)
    {
        var random = new Random(seed);
        var isConnected = new int[cityCount][];
        var otherCityCount = cityCount - 1;

        for (var i = 0; i < cityCount; i++)
        {
            isConnected[i] = new int[cityCount];
            isConnected[i][i] = 1;
        }

        for (var i = 0; i < cityCount; i++)
        {
            for (var j = i + 1; j < cityCount; j++)
            {
                var edgeDrawn = random.Next(otherCityCount) < ExpectedNeighbourCount;
                var connected = edgeDrawn ? 1 : 0;
                isConnected[i][j] = connected;
                isConnected[j][i] = connected;
            }
        }

        return isConnected;
    }
}
