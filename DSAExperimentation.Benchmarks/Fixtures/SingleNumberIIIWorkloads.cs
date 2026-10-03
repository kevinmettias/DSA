namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 260: two singleton values plus (length / 2 - 1)
// duplicated pairs of distinct values, so no two pairs collide into a value that
// appears four times, then shuffled so BruteForce can't exploit input order.
internal static class SingleNumberIIIWorkloads
{
    // Each pair consists of two identical values contributed to the array.
    private const int ElementsPerPair = 2;

    // Exclusive upper bound for paired values.
    private const int ValueUpperBound = 1_000_000;

    // LC 260 guarantees exactly two numbers appear exactly once.
    private const int SingletonCount = 2;

    // The second of the two singleton (appears-once) values seeded into the array.
    private const int SecondSingletonValue = -2;

    public static int[] BuildValues(int length, int seed)
    {
        var random = new Random(seed);
        var pairCount = length / ElementsPerPair - 1;
        var pairs = SeededDraws.DistinctValues(pairCount, 1, ValueUpperBound, random);

        var values = new List<int>(pairs.Length * ElementsPerPair + SingletonCount);
        values.AddRange(pairs);
        values.AddRange(pairs);
        AppendSingletonValues(values);

        return values.OrderBy(_ => random.Next()).ToArray();
    }

    // The two values that appear exactly once: one fixed, one at the far negative end.
    private static void AppendSingletonValues(List<int> values)
    {
        values.Add(-1);
        values.Add(SecondSingletonValue);
    }
}
