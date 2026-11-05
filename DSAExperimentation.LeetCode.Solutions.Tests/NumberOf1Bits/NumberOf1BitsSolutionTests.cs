using DSAExperimentation.LeetCode.Solutions.NumberOf1Bits;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOf1Bits;

// Harness only: both strategies live in NumberOf1BitsSolution and are asserted
// against an input from LeetCode's earlier unsigned statement, which today's signed
// bound no longer admits. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class NumberOf1BitsSolutionTests
{
    public static TheoryData<uint, int> Cases =>
        new()
        {
            // Example 3 of LeetCode's earlier unsigned statement, past today's bound
            // of 2^31 - 1: 4294967293 is 0xFFFFFFFD, all 32 bits but bit 1, so 31.
            { 4294967293u, 31 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByBitClear_Cases_ReturnsSetBitCount(uint value, int expected) =>
        Assert.Equal(expected, NumberOf1BitsSolution.CountByBitClear(value));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByShiftAndMask_Cases_ReturnsSetBitCount(uint value, int expected) =>
        Assert.Equal(expected, NumberOf1BitsSolution.CountByShiftAndMask(value));
}
