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

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they reverse to the same pattern on every example - not merely
    // that each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void Reverse_AgreeOnEveryExample(uint value, uint expected) =>
        Assert.Equal(
            ReverseBitsSolution.ReverseByBitShift(value),
            ReverseBitsSolution.ReverseByByteLookup(value));
}
