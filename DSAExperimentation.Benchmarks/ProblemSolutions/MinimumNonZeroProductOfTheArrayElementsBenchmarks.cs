using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Non-Zero Product of the Array Elements (LC 1969): naive O(exponent)
// repeated modular multiplication vs. O(log exponent) exponentiation by squaring
// (PowXnBenchmarks'/SuperPowBenchmarks' squaring loop, folded under Modulus at
// every step). No repo container or algorithm primitive applies here - there is
// nothing to compose over a handful of running scalars, the same "lighter
// repo-primitive fit" those two benchmarks already document. Exponent stands in
// for 2^(p-1)-1 (the real pair count for LeetCode's own p <= 60 - up to ~2^59,
// far beyond any naive loop, which is exactly why the squaring algorithm exists
// at all) kept to a size the naive loop can still finish inside a benchmark run.
[MemoryDiagnoser]
public class MinimumNonZeroProductOfTheArrayElementsBenchmarks
{
    private const long Modulus = 1_000_000_007;
    private const long PairBase = 999_999_999; // stand-in for 2^p - 2

    [Params(10_000, 1_000_000)]
    public int Exponent;

    [Benchmark(Baseline = true)]
    public long RepeatedModularMultiplication()
    {
        var result = 1L;
        var value = PairBase % Modulus;

        for (var i = 0; i < Exponent; i++)
        {
            result = result * value % Modulus;
        }

        return result;
    }

    [Benchmark]
    public long ModPowBySquaring() => ModPow(PairBase, Exponent);

    private static long ModPow(long value, long exponent)
    {
        var result = 1L;
        value %= Modulus;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % Modulus;
            }

            value = value * value % Modulus;
            exponent >>= 1;
        }

        return result;
    }
}
