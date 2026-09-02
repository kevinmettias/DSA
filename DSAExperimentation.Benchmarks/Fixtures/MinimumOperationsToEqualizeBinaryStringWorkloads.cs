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
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = random.Next(2) == 0 ? '0' : '1';
        }

        var k = Math.Max(1, length / 2);

        return (new string(chars), k);
    }
}
