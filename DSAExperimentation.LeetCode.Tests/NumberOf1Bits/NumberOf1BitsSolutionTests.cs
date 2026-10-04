using DSAExperimentation.LeetCode.NumberOf1Bits;

namespace DSAExperimentation.LeetCode.Tests.NumberOf1Bits;

// Harness only: both strategies live in NumberOf1BitsSolution and are asserted
// against LeetCode's published examples and one from its earlier statement.
public sealed partial class NumberOf1BitsSolutionTests
{
    public static TheoryData<uint, int> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { 11u, 3 },
            { 128u, 1 },
            { 2147483645u, 30 },

            // The example 3 of LeetCode's earlier unsigned statement, past today's bound
            // of 2^31 - 1: 4294967293 is 0xFFFFFFFD, all 32 bits but bit 1, so 31.
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
}
