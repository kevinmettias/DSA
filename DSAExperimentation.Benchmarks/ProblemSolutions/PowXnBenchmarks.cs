using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Pow(x, n) (LC 50): naive O(|n|) repeated multiplication vs. O(log |n|)
// exponentiation by squaring. No repo container or algorithm primitive applies here -
// there is nothing to compose over a single running scalar and exponent counter, the
// same "lighter repo-primitive fit" case as Power of Two's bit trick. Base is chosen
// close to 1 so the largest exponent doesn't overflow to infinity under either
// strategy.
[MemoryDiagnoser]
public class PowXnBenchmarks
{
    private const double Base = 1.0000001;

    [Params(10_000, 1_000_000)]
    public int Exponent;

    [Benchmark(Baseline = true)]
    public double RepeatedMultiplication()
    {
        var result = 1.0;

        for (var i = 0; i < Exponent; i++)
        {
            result *= Base;
        }

        return result;
    }

    [Benchmark]
    public double ExponentiationBySquaring() => MyPow(Base, Exponent);

    private static double MyPow(double x, int n)
    {
        long exponent = n;

        if (exponent < 0)
        {
            x = 1 / x;
            exponent = -exponent;
        }

        var result = 1.0;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result *= x;
            }

            x *= x;
            exponent >>= 1;
        }

        return result;
    }
}
