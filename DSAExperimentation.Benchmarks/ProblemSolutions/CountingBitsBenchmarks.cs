using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountingBits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountingBitsSolution's, the same methods
// CountingBitsTests proves correct. The maximum value is a scalar [Params] value
// with nothing to hoist into [GlobalSetup] - there is no input container to prepare
// ahead of the measured call.
[MemoryDiagnoser]
public class CountingBitsBenchmarks
{
    [Params(2_000, 40_000)]
    public int MaximumValue { get; set; }

    [Benchmark(Baseline = true)]
    public int[] PerNumberLoop() => CountingBitsSolution.CountBitsByPerNumberLoop(MaximumValue);

    [Benchmark]
    public int[] MemoizedRecurrence() => CountingBitsSolution.CountBitsByMemoizedRecurrence(MaximumValue);
}
