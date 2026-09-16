using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOf1BitsBenchmarks (ARCHITECTURE 17.9): the class has a single arm, so
// there is no second strategy to reconcile it against and the assertion has to come from what the
// class comment makes decisive. It also carries no [Params] and no [GlobalSetup] - the fixed packed
// constant is the whole workload - so the only thing to construct is the harness itself, and the
// lone arm need not carry [Benchmark(Baseline = true)] (it does not).
//
// The operand is 4294967293, i.e. all 32 bits set except bit 1, which is the class comment's own
// "31 of 32 bits set" worst case for Kernighan's loop. That count is the decisive literal, so it is
// stated here rather than read back out of the arm.
public sealed partial class NumberOf1BitsBenchmarksTests
{
    private const int ExpectedSetBitCount = 31;

    [Fact]
    public void BitClear_DenseThirtyTwoBitOperand_CountsTheThirtyOneSetBits() =>
        Assert.Equal(ExpectedSetBitCount, BuildHarness().BitClear());

    private static NumberOf1BitsBenchmarks BuildHarness() => new();
}
