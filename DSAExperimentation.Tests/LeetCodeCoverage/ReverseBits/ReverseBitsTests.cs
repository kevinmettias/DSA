using DSAExperimentation.LeetCode.ReverseBits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseBits;

// Harness only: the single strategy lives in ReverseBitsSolution and is asserted
// against LeetCode's published examples.
public sealed class ReverseBitsTests
{
    public static TheoryData<uint, uint> Examples =>
        new()
        {
            { 43261596u, 964176192u },
            { 4294967293u, 3221225471u },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseByBitShift_LeetCodeExamples_ReturnsReversedBitPattern(uint value, uint expected) =>
        Assert.Equal(expected, ReverseBitsSolution.ReverseByBitShift(value));
}
