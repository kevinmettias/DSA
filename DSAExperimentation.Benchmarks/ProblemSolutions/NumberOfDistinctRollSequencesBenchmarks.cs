using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Distinct Roll Sequences (LC 2318): the textbook unmemoized recursion
// over (day, secondLastRoll, lastRoll) - re-explores the same state on every
// distinct path that reaches it, exponential in n - vs. the same recursion routed
// through this repo's own Memoizer, keyed on that 3-tuple state
// (SuperEggDropBenchmarks/NumberOfMusicPlaylistsBenchmarks precedent for a
// multi-field tuple state), which visits each of the O(n) distinct states exactly
// once. N is kept small enough that the brute force still finishes in reasonable
// time (CanIWinBenchmarks' own precedent for bounding an exponential baseline's
// input size).
[MemoryDiagnoser]
public class NumberOfDistinctRollSequencesBenchmarks
{
    private const long Modulus = 1_000_000_007;

    [Params(6, 10)]
    public int N;

    [Benchmark(Baseline = true)]
    public long BruteForceRecursion() => CountSequences(1, 0, 0);

    private long CountSequences(int day, int prev2, int prev1)
    {
        if (day > N)
        {
            return 1;
        }

        var total = 0L;

        for (var value = 1; value <= 6; value++)
        {
            if (value == prev1 || value == prev2)
            {
                continue;
            }

            if (prev1 != 0 && Gcd(value, prev1) != 1)
            {
                continue;
            }

            total = (total + CountSequences(day + 1, prev1, value)) % Modulus;
        }

        return total;
    }

    [Benchmark]
    public long MemoizedRecursion()
        => Memoizer.Memoize<(int Day, int Prev2, int Prev1), long>((1, 0, 0), Ways);

    private long Ways((int Day, int Prev2, int Prev1) state, Func<(int, int, int), long> ways)
    {
        var (day, prev2, prev1) = state;

        if (day > N)
        {
            return 1;
        }

        var total = 0L;

        for (var value = 1; value <= 6; value++)
        {
            if (value == prev1 || value == prev2)
            {
                continue;
            }

            if (prev1 != 0 && Gcd(value, prev1) != 1)
            {
                continue;
            }

            total = (total + ways((day + 1, prev1, value))) % Modulus;
        }

        return total;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
