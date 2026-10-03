namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1039 - a random scattering of vertex weights
// around one convex polygon, at whatever count an arm asks for. The benchmark keeps
// its genuinely exponential un-memoized baseline to 14 vertices and runs the memoized
// arm on to LC 1039's bound of 50, the same split BurstBalloonsWorkloads records for
// LC 312's identical interval recurrence.
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
