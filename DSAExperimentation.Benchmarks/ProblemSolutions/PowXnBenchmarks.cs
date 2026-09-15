using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PowXn;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PowXnSolution's, the same methods PowXnTests proves
// correct. Base is chosen close to 1 so the largest exponent doesn't overflow to
// infinity under either strategy.
[MemoryDiagnoser]
public class PowXnBenchmarks
{
    private const double Base = 1.0000001;

    [Params(10_000, 1_000_000)]
    public int Exponent { get; set; }

    [Benchmark(Baseline = true)]
    public double RepeatedMultiplication() => PowXnSolution.PowByRepeatedMultiplication(Base, Exponent);

    [Benchmark]
    public double ExponentiationBySquaring() => PowXnSolution.PowByExponentiationBySquaring(Base, Exponent);
}
