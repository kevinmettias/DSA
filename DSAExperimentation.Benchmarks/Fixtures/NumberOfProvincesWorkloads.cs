namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 547 - a random symmetric adjacency matrix with a
// low connection probability, so most runs form several provinces rather than
// collapsing into one via a single dense component.
internal static class NumberOfProvincesWorkloads
{
    private const int ConnectionOddsDenominator = 10;

    public static int[][] BuildAdjacencyMatrix(int cityCount, int seed)
    {
        var random = new Random(seed);
        var isConnected = new int[cityCount][];

        for (var i = 0; i < cityCount; i++)
        {
            isConnected[i] = new int[cityCount];
            isConnected[i][i] = 1;
        }

        for (var i = 0; i < cityCount; i++)
        {
            for (var j = i + 1; j < cityCount; j++)
            {
                var connected = random.Next(0, ConnectionOddsDenominator) == 0 ? 1 : 0;
                isConnected[i][j] = connected;
                isConnected[j][i] = connected;
            }
        }

        return isConnected;
    }
}
