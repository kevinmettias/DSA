namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3899 - a single random side triple, rejection
// sampled until it satisfies the strict triangle inequality. LC 3899's two
// strategies differ only in whether the third angle costs a third Acos call, not
// in how the work scales with input size (there is no size axis - the input is
// always exactly three sides), so one fixed valid triangle is the whole workload.
internal static class TriangleSideWorkloads
{
    private const int MaxSide = 1000;

    public static int[] BuildValidTriangle(int seed)
    {
        var random = new Random(seed);

        while (true)
        {
            var sides = new[]
            {
                random.Next(1, MaxSide + 1),
                random.Next(1, MaxSide + 1),
                random.Next(1, MaxSide + 1),
            };

            var sorted = (int[])sides.Clone();
            Array.Sort(sorted);

            if (sorted[0] + sorted[1] > sorted[2])
            {
                return sides;
            }
        }
    }
}
