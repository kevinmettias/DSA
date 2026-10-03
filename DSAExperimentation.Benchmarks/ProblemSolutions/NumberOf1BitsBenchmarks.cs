using DSAExperimentation.LeetCode.NumberOf1Bits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOf1BitsSolution's. The previous class was
// a compile-smoke placeholder (`=> 1` on both arms) that measured nothing; this
// measures two counting loops against a dense worst case (31 of 32 bits set).
// Kernighan's loop then runs its full 31 iterations, while the shift-and-mask
// arm always runs all 32 positions.
public class NumberOf1BitsBenchmarks
{
    private const uint Value = 4294967293u;

    [Benchmark(Baseline = true)]
    public int BitClear() => NumberOf1BitsSolution.CountByBitClear(Value);

    [Benchmark]
    public int ShiftAndMask() => NumberOf1BitsSolution.CountByShiftAndMask(Value);
}
