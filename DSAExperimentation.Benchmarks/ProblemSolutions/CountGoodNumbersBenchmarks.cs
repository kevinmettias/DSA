using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Good Numbers (LC 1922): n can be up to 1e15, so the naive "multiply n times"
// construction is the wrong shape entirely - exponentiation by squaring (PowXn's own
// loop, folded under a modulus the way SuperPow's is) answers the same question in
// O(log n). No repo container or algorithm primitive applies to either side - the
// same "lighter repo-primitive fit" this repo already accepted for both of those.
[MemoryDiagnoser]
public class CountGoodNumbersBenchmarks
{
    private const long Modulo = 1_000_000_007;

    // LC 1922: even-index digits must be one of {0,2,4,6,8}, odd-index digits must
    // be one of the prime digits {2,3,5,7}.
    private const long EvenIndexDigitChoices = 5;
    private const long OddIndexDigitChoices = 4;

    // Splits the N positions into how many sit at an even index vs. an odd index.
    private const int PositionParityDivisor = 2;

    [Params(1_000, 1_000_000)]
    public long N;

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        var evenPositions = (N + 1) / PositionParityDivisor;
        var oddPositions = N / PositionParityDivisor;

        return NaivePow(EvenIndexDigitChoices, evenPositions) * NaivePow(OddIndexDigitChoices, oddPositions) % Modulo;
    }

    [Benchmark]
    public long ExponentiationBySquaring()
    {
        var evenPositions = (N + 1) / PositionParityDivisor;
        var oddPositions = N / PositionParityDivisor;

        return ModPow(EvenIndexDigitChoices, evenPositions) * ModPow(OddIndexDigitChoices, oddPositions) % Modulo;
    }

    private static long NaivePow(long value, long exponent)
    {
        var result = 1L;
        for (var i = 0L; i < exponent; i++)
        {
            result = result * value % Modulo;
        }

        return result;
    }

    private static long ModPow(long value, long exponent)
    {
        var result = 1L;
        value %= Modulo;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % Modulo;
            }

            value = value * value % Modulo;
            exponent >>= 1;
        }

        return result;
    }
}
