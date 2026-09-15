using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AddDigits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddDigitsSolution's, the same methods AddDigitsTests
// proves correct.
[MemoryDiagnoser]
public class AddDigitsBenchmarks
{
    [Params(999_999, int.MaxValue)]
    public int Value { get; set; }

    [Benchmark(Baseline = true)]
    public int Arithmetic() => AddDigitsSolution.AddDigitsByArithmetic(Value);

    [Benchmark]
    public int StackDigits() => AddDigitsSolution.AddDigitsByStack(Value);
}
