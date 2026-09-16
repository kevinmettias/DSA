using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseBitsBenchmarks (ARCHITECTURE 17.9): the class has a single arm, so
// there is no second strategy to reconcile it against and the assertion has to come from what the
// class comment makes decisive. It also carries no [Params] and no [GlobalSetup] - the fixed packed
// operand is the whole workload - so the only thing to construct is the harness itself, and the lone
// arm need not carry [Benchmark(Baseline = true)] (it does not).
//
// The operand is 4294967293, i.e. all 32 bits set except bit 1, which is LeetCode 190's own second
// example. Reversing a bit pattern with exactly that one bit clear is 0xBFFFFFFF, so the expected
// answer is the complementary pattern stated here rather than read back out of the arm.
public sealed partial class ReverseBitsBenchmarksTests
{
    private const uint ExpectedReversedBits = 3221225471u;

    [Fact]
    public void BitShift_SingleBitClearOperand_ReversesToTheComplementaryPattern() =>
        Assert.Equal(ExpectedReversedBits, BuildHarness().BitShift());

    private static ReverseBitsBenchmarks BuildHarness() => new();
}
