namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 749 - a side x side grid whose cells are each infected
// with a fixed probability, every draw from one seed. LC 749 also promises that each round
// exactly one region threatens the most uninfected cells, and a random grid does not keep
// that promise by itself: which seeds keep it is a property of the seed, so
// ContainVirusWorkloadsTests replays the process on the seed ContainVirusBenchmarks uses, at
// every side it runs, and checks every round.
internal static class ContainVirusWorkloads
{
    private const double InfectionProbability = 0.15;

    public static int[][] BuildGrid(int side, int seed)
    {
        var random = new Random(seed);
        var grid = new int[side][];
        for (var row = 0; row < side; row++)
        {
            grid[row] = new int[side];
            for (var col = 0; col < side; col++)
            {
                var isInfected = random.NextDouble() < InfectionProbability;
                grid[row][col] = isInfected ? 1 : 0;
            }
        }

        return grid;
    }
}
