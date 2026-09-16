using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindKthBitInNthBinaryString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindKthBitInNthBinaryStringSolution's. The bit position
// is fixed at the last bit of S(n) (2^n - 1) so both approaches do a full-depth walk: a
// complete string build for the baseline, the deepest possible recursion chain for the
// bisection.
[MemoryDiagnoser]
public class FindKthBitInNthBinaryStringBenchmarks
{
    private int _bitPosition;

    [Params(10, 20)]
    public int StringOrder { get; set; }

    [GlobalSetup]
    public void Setup() => _bitPosition = (1 << StringOrder) - 1;

    [Benchmark(Baseline = true)]
    public char BruteForceConstruction() =>
        FindKthBitInNthBinaryStringSolution.FindKthBitByStringConstruction(StringOrder, _bitPosition);

    [Benchmark]
    public char RecursiveBisection() =>
        FindKthBitInNthBinaryStringSolution.FindKthBitByRecursiveBisection(StringOrder, _bitPosition);
}
