using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindKthBitInNthBinaryString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindKthBitInNthBinaryStringSolution's. K is fixed at
// the last bit of S(n) (2^n - 1) so both approaches do a full-depth walk: a complete
// string build for the baseline, the deepest possible recursion chain for the
// bisection.
[MemoryDiagnoser]
public class FindKthBitInNthBinaryStringBenchmarks
{
    [Params(10, 20)]
    public int N;

    private int _k;

    [GlobalSetup]
    public void Setup() => _k = (1 << N) - 1;

    [Benchmark(Baseline = true)]
    public char BruteForceConstruction() =>
        FindKthBitInNthBinaryStringSolution.FindKthBitByStringConstruction(N, _k);

    [Benchmark]
    public char RecursiveBisection() =>
        FindKthBitInNthBinaryStringSolution.FindKthBitByRecursiveBisection(N, _k);
}
