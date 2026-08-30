using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Integer Replacement (LC 397): plain unmemoized recursion over
// replace(n) = 0 when n == 1, 1 + replace(n/2) when n is even, and
// 1 + min(replace(n-1), replace(n+1)) when n is odd, vs. this repo's own
// Memoizer-driven version of the same recurrence - same DP composition
// CountingBitsBenchmarks/HouseRobberBenchmarks already use. N is deliberately the
// repeating-bit pattern 0b0101...01 at two bit-lengths (not a "typical" random
// value): every bit position forces an odd branch, so the unmemoized tree explodes
// into millions of redundant calls (~10.9M for the 31-bit case) while the memoized
// version only ever computes a few dozen distinct values along the way (~90 for
// the same case) - this recurrence's actual reconvergence-driven asymptotic gap,
// not an artifact of a convenient input. N is long, not int: n can be int.MaxValue,
// and the n+1 branch would otherwise silently overflow Int32.
[MemoryDiagnoser]
public class IntegerReplacementBenchmarks
{
    [Params(21_845, 1_431_655_765)]
    public long N;

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Replace(N);

    [Benchmark]
    public int MemoizedRecurrence()
        => Memoizer.Memoize<long, int>(N, (value, replace) => value switch
        {
            1 => 0,
            _ when value % 2 == 0 => 1 + replace(value / 2),
            _ => 1 + Math.Min(replace(value - 1), replace(value + 1)),
        });

    private static int Replace(long n)
        => n switch
        {
            1 => 0,
            _ when n % 2 == 0 => 1 + Replace(n / 2),
            _ => 1 + Math.Min(Replace(n - 1), Replace(n + 1)),
        };
}
