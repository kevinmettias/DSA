using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class DivideTwoIntegersBenchmarks
{
    [Params(1_000_000, int.MaxValue)] public int Dividend;
    private const int Divisor = 7;
    [Benchmark(Baseline = true)] public int BuiltInDivide() => Dividend / Divisor;
    [Benchmark] public int BinarySearchProduct() { var sequence = new ProductExceedsSequence(Divisor, Dividend, (Dividend / Divisor) + 2); var firstTooLarge = BinarySearch.LowerBound<int, ProductExceedsSequence>(sequence, 1); return firstTooLarge - 1; }
    private readonly struct ProductExceedsSequence(long divisor, long dividend, int length) : IRandomAccessSequence<int> { public int Length => length; public int Get(int quotient) => divisor * quotient > dividend ? 1 : 0; }
}
