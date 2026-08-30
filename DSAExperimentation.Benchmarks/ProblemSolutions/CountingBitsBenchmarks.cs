using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Counting Bits (LC 338): counting each number's set bits independently (the
// textbook per-number loop) vs. this repo's own Memoizer-driven DP recurrence
// ans[i] = ans[i>>1] + (i&1) - the same Memoizer composition HouseRobberBenchmarks/
// HouseRobberIIBenchmarks already use for a different recurrence.
[MemoryDiagnoser]
public class CountingBitsBenchmarks
{
    [Params(2_000, 40_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int PerNumberLoop()
    {
        var total = 0;

        for (var i = 0; i <= N; i++)
        {
            var value = i;
            while (value != 0)
            {
                value &= value - 1;
                total++;
            }
        }

        return total;
    }

    [Benchmark]
    public int MemoizedRecurrence()
    {
        var total = 0;

        for (var i = 0; i <= N; i++)
        {
            total += Memoizer.Memoize<int, int>(
                i, (value, countBits) => value == 0 ? 0 : countBits(value >> 1) + (value & 1));
        }

        return total;
    }
}
