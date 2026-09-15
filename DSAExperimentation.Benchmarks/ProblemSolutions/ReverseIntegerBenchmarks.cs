using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReverseInteger;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseIntegerSolution's, the same methods
// ReverseIntegerTests proves correct.
[MemoryDiagnoser]
public class ReverseIntegerBenchmarks
{
    [Params(123456789, 1534236469)]
    public int Value { get; set; }

    [Benchmark(Baseline = true)]
    public int Arithmetic() => ReverseIntegerSolution.ReverseByArithmetic(Value);

    [Benchmark]
    public int StackDigits() => ReverseIntegerSolution.ReverseByDigitStack(Value);
}
