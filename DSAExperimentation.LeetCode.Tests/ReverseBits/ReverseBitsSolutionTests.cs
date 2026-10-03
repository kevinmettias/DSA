using DSAExperimentation.LeetCode.ReverseBits;

namespace DSAExperimentation.LeetCode.Tests.ReverseBits;

// Harness only: both strategies live in ReverseBitsSolution and are asserted
// against LeetCode's published examples.
public sealed partial class ReverseBitsSolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseByByteLookup_LeetCodeExamples_ReturnsReversedBitPattern(uint value, uint expected) =>
        Assert.Equal(expected, ReverseBitsSolution.ReverseByByteLookup(value));
}
