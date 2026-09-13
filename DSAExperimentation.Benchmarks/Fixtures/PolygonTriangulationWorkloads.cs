namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1039 - a random scattering of vertex weights
// around one convex polygon. Kept modest (<=14 in the benchmark's own [Params])
// because MinimumScoreTriangulationOfPolygonSolution's un-memoized baseline is
// genuinely exponential, the same reasoning BurstBalloonsWorkloads already records
// for LC 312's identical interval recurrence.
internal static class PolygonTriangulationWorkloads
{
    private const int MaxVertexWeightExclusive = 100;

    public static int[] BuildVertexWeights(int count, int seed)
    {
        var random = new Random(seed);
        var values = new int[count];

        for (var i = 0; i < count; i++)
        {
            values[i] = random.Next(1, MaxVertexWeightExclusive);
        }

        return values;
    }
}
