using DSAExperimentation.LeetCode.Solutions.ReverseBits;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReverseBits;

// Harness only: both strategies live in ReverseBitsSolution and are asserted
// against an example from LeetCode's earlier unsigned statement, whose input today's
// bound no longer allows. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ReverseBitsSolutionTests
{
    public static TheoryData<uint, uint> Cases =>
        new()
        {
            // The example 2 of LeetCode's earlier unsigned statement, past today's bound
            // of 2^31 - 2: 0xFFFFFFFD, all ones but bit 1, reverses to all ones but bit
            // 30, 0xBFFFFFFF = 3221225471.
            { 4294967293u, 3221225471u },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReverseByBitShift_Cases_ReturnsReversedBitPattern(uint value, uint expected) =>
        Assert.Equal(expected, ReverseBitsSolution.ReverseByBitShift(value));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReverseByByteLookup_Cases_ReturnsReversedBitPattern(uint value, uint expected) =>
        Assert.Equal(expected, ReverseBitsSolution.ReverseByByteLookup(value));
}
