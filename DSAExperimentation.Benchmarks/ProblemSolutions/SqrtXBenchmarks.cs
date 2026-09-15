using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SqrtXSolution's.
[MemoryDiagnoser]
public class SqrtXBenchmarks
{
    [Params(10_000, int.MaxValue)]
    public int Value { get; set; }

    [Benchmark(Baseline = true)]
    public int MathSqrt() => SqrtXSolution.RootByMathSqrt(Value);

    [Benchmark]
    public int BinarySearchRoot() => SqrtXSolution.RootByBinarySearch(Value);
}
