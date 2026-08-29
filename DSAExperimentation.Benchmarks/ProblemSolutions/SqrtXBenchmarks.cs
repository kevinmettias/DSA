using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SqrtXBenchmarks
{
    [Params(10_000, int.MaxValue)] public int Value;
    [Benchmark(Baseline = true)] public int MathSqrt() => (int)Math.Sqrt(Value);
    [Benchmark] public int BinarySearchRoot() { var sequence = new SquareExceedsSequence(Value, Math.Min(Value, 46341) + 1); return BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1; }
    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int> { public int Length => length; public int Get(int value) => (long)value * value > x ? 1 : 0; }
}
