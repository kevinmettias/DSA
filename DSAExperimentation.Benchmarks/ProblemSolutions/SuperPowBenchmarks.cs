using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Super Pow (LC 372): naive O(exponent) repeated modular multiplication vs.
// O(digits * log 10) digit-wise modular exponentiation by squaring (Horner's rule over
// the digit array, PowXnBenchmarks' squaring loop folded under Modulus at every step).
// No repo container or algorithm primitive applies here - there is nothing to compose
// over a handful of running scalars, the same "lighter repo-primitive fit" case
// PowXnBenchmarks already documents. Exponent is kept to a size the naive loop can
// still finish (LeetCode's real inputs go up to a 2000-digit exponent - far beyond any
// naive loop, which is exactly why the digit-wise algorithm exists at all).
[MemoryDiagnoser]
public class SuperPowBenchmarks
{
    private const int Modulus = 1337;
    private const int Base = 7;
    private const int HornerDigitBase = 10;

    [Params(10_000, 1_000_000)]
    public int Exponent;

    private int[] _digits = null!;

    [GlobalSetup]
    public void Setup() => _digits = Exponent.ToString().Select(c => c - '0').ToArray();

    [Benchmark(Baseline = true)]
    public int RepeatedModularMultiplication()
    {
        var result = 1L;
        var baseTerm = Base % Modulus;

        for (var i = 0; i < Exponent; i++)
        {
            result = result * baseTerm % Modulus;
        }

        return (int)result;
    }

    [Benchmark]
    public int DigitwiseModPow() => SuperPow(Base, _digits);

    private static int SuperPow(int a, int[] b)
    {
        var result = 1L;
        var baseTerm = a % Modulus;

        foreach (var digit in b)
        {
            result = ModPow(result, HornerDigitBase) * ModPow(baseTerm, digit) % Modulus;
        }

        return (int)result;
    }

    private static long ModPow(long value, int exponent)
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
