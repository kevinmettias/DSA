using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Collisions of Monkeys on a Polygon (LC 2550): the answer is always
// (2^n - 2) mod 1e9+7, so the only real performance question is how 2^n mod
// is computed - naive repeated multiplication (O(n)) vs. exponentiation by
// squaring (O(log n)), the same "two ways to compute the same fold" contrast
// FindGreatestCommonDivisorOfArrayBenchmarks already runs for its own
// Gcd(min, max).
[MemoryDiagnoser]
public class CountCollisionsOfMonkeysOnAPolygonBenchmarks
{
    private const long Mod = 1_000_000_007;

    [Params(1_000, 1_000_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public long RepeatedMultiplication()
    {
        var power = 1L;

        for (var i = 0; i < N; i++)
        {
            power = power * 2 % Mod;
        }

        return (power - 2 + Mod) % Mod;
    }

    [Benchmark]
    public long ExponentiationBySquaring()
    {
        var power = ModPow(2, N, Mod);
        return (power - 2 + Mod) % Mod;
    }

    private static long ModPow(long value, long exponent, long modulus)
    {
        var result = 1L;
        value %= modulus;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % modulus;
            }

            value = value * value % modulus;
            exponent >>= 1;
        }

        return result;
    }
}
