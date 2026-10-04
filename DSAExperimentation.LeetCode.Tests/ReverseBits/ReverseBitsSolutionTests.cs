using DSAExperimentation.LeetCode.ReverseBits;

namespace DSAExperimentation.LeetCode.Tests.ReverseBits;

// Harness only: both strategies live in ReverseBitsSolution and are asserted
// against LeetCode's published examples and one from its earlier statement.
public sealed partial class ReverseBitsSolutionTests
{
    public static TheoryData<uint, uint> Examples =>
        new()
        {
            // LeetCode examples 1 and 2.
            { 43261596u, 964176192u },
            { 2147483644u, 1073741822u },

            // The example 2 of LeetCode's earlier unsigned statement, past today's bound
            // of 2^31 - 2: 0xFFFFFFFD, all ones but bit 1, reverses to all ones but bit
            // 30, 0xBFFFFFFF = 3221225471.
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
