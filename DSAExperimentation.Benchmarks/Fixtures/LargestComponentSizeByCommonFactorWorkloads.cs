namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 952: distinct values whose every prime factor comes from one
// small shared pool, so real overlaps - and therefore real merge work - occur, the same
// "force genuine matches, not coincidental ones" intent AccountsMergeBenchmarks' own
// generator uses. The values are a seeded sample of the pool-smooth numbers in
// [2, 10^5]: inside LC 952's value range, and unique as it requires. Which primes make
// the pool is the harness's choice, so it is handed in.
internal static class LargestComponentSizeByCommonFactorWorkloads
{
    private const int MaxValue = 100_000;

    public static int[] Build(int length, int[] primePool, int seed)
    {
        var smoothValues = SmoothValues(primePool);
        var order = SeededSequences.ShuffledZeroTo(smoothValues.Length, seed);

        return [.. order.Take(length).Select(index => smoothValues[index])];
    }

    // Every value in [2, MaxValue] with no prime factor outside primePool, ascending. Each
    // prime in turn multiplies every value found so far by each of its powers that still fits.
    private static int[] SmoothValues(int[] primePool)
    {
        var values = new SortedSet<int> { 1 };

        foreach (var prime in primePool)
        {
            foreach (var value in values.ToArray())
            {
                AddPowerMultiples(values, value, prime);
            }
        }

        values.Remove(1);

        return [.. values];
    }

    private static void AddPowerMultiples(SortedSet<int> values, int value, int prime)
    {
        for (var multiple = (long)value * prime; multiple <= MaxValue; multiple *= prime)
        {
            values.Add((int)multiple);
        }
    }
}
