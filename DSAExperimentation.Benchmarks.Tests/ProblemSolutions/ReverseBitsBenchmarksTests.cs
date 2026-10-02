using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseBitsBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the bit-shift walk and the byte-lookup table - so a harness
// whose arms disagree is reversing two different bit patterns. It carries no [Params] and no
// [GlobalSetup] - the fixed packed operand is the whole workload - so the only thing to construct
// is the harness itself.
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

    [Fact]
    public void ByteLookup_SingleBitClearOperand_ReversesToTheComplementaryPattern() =>
        Assert.Equal(ExpectedReversedBits, BuildHarness().ByteLookup());

    [Fact]
    public void ByteLookup_AgreesWithBitShift() =>
        Assert.Equal(BuildHarness().BitShift(), BuildHarness().ByteLookup());

    private static ReverseBitsBenchmarks BuildHarness() => new();
}
