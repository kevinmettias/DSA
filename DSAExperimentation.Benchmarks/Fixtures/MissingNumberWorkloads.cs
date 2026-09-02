namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 268: length distinct values from [0, length]
// with one value removed, then shuffled so BruteForce can't exploit an
// already-sorted input.
internal static class MissingNumberWorkloads
{
    public static int[] BuildValues(int length, int seed)
    {
        var random = new Random(seed);
        var missing = random.Next(0, length + 1);

        return Enumerable.Range(0, length + 1)
            .Where(n => n != missing)
            .OrderBy(_ => random.Next())
            .ToArray();
    }
}
