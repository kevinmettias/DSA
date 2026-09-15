namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3666 - a random binary string with k pinned near
// n / 2, so a single operation's reachable zero-count range is wide and both
// strategies actually explore a nontrivial slice of the 0..n state graph instead of
// a handful of states.
internal static class MinimumOperationsToEqualizeBinaryStringWorkloads
{
    public static (string S, int K) Build(int length, int seed)
    {
        var random = new Random(seed);
        var chars = BuildRandomChars(random, length);

        var k = Math.Max(1, length / 2);

        return (new string(chars), k);
    }

    // One seeded draw per position, in order, so the string a given (length, seed)
    // pair produces is a function of the seed alone.
    private static char[] BuildRandomChars(Random random, int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            var isZero = random.Next(2) == 0;
            chars[i] = isZero ? '0' : '1';
        }

        return chars;
    }
}
