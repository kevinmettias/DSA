using DSAExperimentation.LeetCode.NumberOf1Bits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOf1Bits;

// Harness only: both strategies live in NumberOf1BitsSolution and are asserted
// against LeetCode's published examples.
public sealed partial class NumberOf1BitsTests
{
    public static TheoryData<uint, int> Examples =>
        new()
        {
            { 11u, 3 },
            { 128u, 1 },
            { 4294967293u, 31 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBitClear_LeetCodeExamples_ReturnsSetBitCount(uint value, int expected) =>
        Assert.Equal(expected, NumberOf1BitsSolution.CountByBitClear(value));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByShiftAndMask_LeetCodeExamples_ReturnsSetBitCount(uint value, int expected) =>
        Assert.Equal(expected, NumberOf1BitsSolution.CountByShiftAndMask(value));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they count the same bits on every example - not merely that
    // each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void Count_AgreeOnEveryExample(uint value, int expected) =>
        Assert.Equal(
            NumberOf1BitsSolution.CountByBitClear(value),
            NumberOf1BitsSolution.CountByShiftAndMask(value));
}
