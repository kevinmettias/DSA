using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count All Valid Pickup and Delivery Options (LC 1359): plain bottom-up
// tabulation of f(i) = f(i-1) * i * (2i-1) vs. this repo's Memoizer-based top-down
// recursion over the same recurrence (UniqueBinarySearchTreesBenchmarks precedent
// for tabulation-vs-Memoizer on a single-int-state DP), both reducing mod 1e9+7 as
// LeetCode requires.
[MemoryDiagnoser]
public class CountAllValidPickupAndDeliveryOptionsBenchmarks
{
    private const long Mod = 1_000_000_007;

    [Params(100, 10_000)]
    public int Orders;

    [Benchmark(Baseline = true)]
    public long Tabulation()
    {
        var ways = 1L;
        for (var i = 1; i <= Orders; i++)
        {
            ways = ways * i % Mod * (2 * i - 1) % Mod;
        }

        return ways;
    }

    [Benchmark]
    public long Memoized() => Memoizer.Memoize<int, long>(Orders, Ways);

    private static long Ways(int orders, Func<int, long> ways)
        => orders == 0 ? 1L : ways(orders - 1) * orders % Mod * (2 * orders - 1) % Mod;
}
