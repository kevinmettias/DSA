namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3605 - alternates runs of small-prime
// multiples (so real stable subarrays exist to cut) with occasional guaranteed
// breaks (a bare 1, coprime to everything) so the array isn't just one giant
// stable block, keeping both the greedy sweep and the binary search doing
// genuine work rather than settling on a trivial answer.
internal static class StabilityFactorWorkloads
{
    private static readonly int[] Primes = [2, 3, 5, 7];
    private const int RunLengthCeilingExclusive = 12;
    private const int MultiplierCeilingExclusive = 100;
    private const int BreakChanceOutOf = 5;
    private const int BreakValue = 1;

    public static int[] Build(int length, int seed)
    {
        var random = new Random(seed);
        var nums = new int[length];
        var index = 0;

        while (index < length)
        {
            var prime = Primes[random.Next(Primes.Length)];
            var runLength = Math.Min(length - index, random.Next(1, RunLengthCeilingExclusive));

            for (var i = 0; i < runLength; i++)
            {
                nums[index++] = prime * random.Next(1, MultiplierCeilingExclusive);
            }

            if (index < length && random.Next(BreakChanceOutOf) == 0)
            {
                nums[index++] = BreakValue;
            }
        }

        return nums;
    }
}
