namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3638 - a deterministic array of random parcel
// weights, the only input either strategy needs.
internal static class MaximumBalancedShipmentsWorkloads
{
    private const int MaxWeight = 1_000_000_000;

    public static int[] BuildWeights(int count, int seed)
    {
        var random = new Random(seed);
        var weights = new int[count];

        for (var i = 0; i < count; i++)
        {
            weights[i] = random.Next(1, MaxWeight + 1);
        }

        return weights;
    }
}
