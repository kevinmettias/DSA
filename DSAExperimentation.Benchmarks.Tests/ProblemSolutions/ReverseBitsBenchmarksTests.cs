using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseBitsBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the bit-shift walk and the byte-lookup table - so a harness
// whose arms disagree is reversing two different bit patterns. It carries no [Params] and no
// [GlobalSetup] - the fixed packed operand is the whole workload - so the only thing to construct
// is the harness itself.
//
// The operand is 2147483644, bits 2 through 30 set, which is LeetCode 190's own second example.
// Reversed, that run lands on bits 1 through 29: 1073741822, the output LeetCode publishes for it,
// so the expected answer is that stated output rather than read back out of the arm.
public sealed partial class ReverseBitsBenchmarksTests
{
    private const uint ExpectedReversedBits = 1073741822u;

    [Fact]
    public void BitShift_LeetCodeSecondExample_ReversesToItsPublishedOutput() =>
        Assert.Equal(ExpectedReversedBits, BuildHarness().BitShift());

    [Fact]
    public void ByteLookup_LeetCodeSecondExample_ReversesToItsPublishedOutput() =>
        Assert.Equal(ExpectedReversedBits, BuildHarness().ByteLookup());

    [Fact]
    public void ByteLookup_AgreesWithBitShift() =>
        Assert.Equal(BuildHarness().BitShift(), BuildHarness().ByteLookup());

    private static ReverseBitsBenchmarks BuildHarness() => new();
}
