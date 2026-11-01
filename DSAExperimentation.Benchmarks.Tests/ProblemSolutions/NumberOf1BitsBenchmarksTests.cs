using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOf1BitsBenchmarks (ARCHITECTURE 17.9): the baseline's count against one
// derived from the operand rather than from either arm, which the generic check cannot supply - it
// holds the shift-and-mask arm to the baseline. It carries no [Params] and no [GlobalSetup] - the
// fixed packed constant is the whole workload - so the only thing to construct is the harness itself.
//
// The operand is 2147483647 (2^31 - 1, LC 191's largest n), i.e. all 32 bits set except the top one,
// which is the class comment's own "31 of 32 bits set" worst case for Kernighan's loop. That count is
// the decisive literal, so it is stated here rather than read back out of the arm.
public sealed partial class NumberOf1BitsBenchmarksTests
{
    private const int ExpectedSetBitCount = 31;

    [Fact]
    public void BitClear_DenseThirtyTwoBitOperand_CountsTheThirtyOneSetBits() =>
        Assert.Equal(ExpectedSetBitCount, BuildHarness().BitClear());

    private static NumberOf1BitsBenchmarks BuildHarness() => new();
}
