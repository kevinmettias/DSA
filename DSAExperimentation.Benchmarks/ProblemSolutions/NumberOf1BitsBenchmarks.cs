using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOf1Bits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is NumberOf1BitsSolution's. The previous class was
// a compile-smoke placeholder (`=> 1` on both arms) that measured nothing;
// this measures the actual bit-clearing loop against a dense worst case (31
// of 32 bits set, so Kernighan's loop runs its full 31 iterations).
[MemoryDiagnoser]
public class NumberOf1BitsBenchmarks
{
    private const uint Value = 4294967293u;

    [Benchmark]
    public int BitClear() => NumberOf1BitsSolution.CountByBitClear(Value);
}
