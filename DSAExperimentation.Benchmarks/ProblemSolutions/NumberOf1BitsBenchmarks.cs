using DSAExperimentation.LeetCode.NumberOf1Bits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOf1BitsSolution's. The previous class was
// a compile-smoke placeholder (`=> 1` on both arms) that measured nothing; this
// measures two counting loops against a dense worst case (31 of 32 bits set):
// 2^31 - 1, the largest n LC 191 allows, sets every bit below the top one.
// Kernighan's loop then runs its full 31 iterations, while the shift-and-mask
// arm always runs all 32 positions. No [Params] axis: the input is one 32-bit word,
// so there is no size to scale.
public class NumberOf1BitsBenchmarks
{
    private const uint Value = 2_147_483_647u;

    [Benchmark(Baseline = true)]
    public int BitClear() => NumberOf1BitsSolution.CountByBitClear(Value);

    [Benchmark]
    public int ShiftAndMask() => NumberOf1BitsSolution.CountByShiftAndMask(Value);
}
