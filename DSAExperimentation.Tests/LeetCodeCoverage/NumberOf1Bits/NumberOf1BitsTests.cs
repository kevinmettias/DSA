using DSAExperimentation.LeetCode.NumberOf1Bits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOf1Bits;

// Harness only: the one strategy lives in NumberOf1BitsSolution and is asserted
// against LeetCode's published examples.
public sealed class NumberOf1BitsTests
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
    public void CountByBitClear_LeetCodeExamples_ReturnsSetBitCount(uint n, int expected) =>
        Assert.Equal(expected, NumberOf1BitsSolution.CountByBitClear(n));
}
