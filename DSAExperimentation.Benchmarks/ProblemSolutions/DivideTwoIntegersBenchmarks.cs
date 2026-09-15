using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DivideTwoIntegers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DivideTwoIntegersSolution's, the same methods
// DivideTwoIntegersTests proves correct.
[MemoryDiagnoser]
public class DivideTwoIntegersBenchmarks
{
    private const int Divisor = 7;

    [Params(1_000_000, int.MaxValue)]
    public int Dividend { get; set; }

    [Benchmark(Baseline = true)]
    public int BuiltInDivide() => DivideTwoIntegersSolution.DivideByBuiltInDivision(Dividend, Divisor);

    [Benchmark]
    public int BinarySearchProduct() => DivideTwoIntegersSolution.DivideByBinarySearchProduct(Dividend, Divisor);
}
