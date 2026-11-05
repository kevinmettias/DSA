using DSAExperimentation.LeetCode.Solutions.SumOfTwoIntegers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumOfTwoIntegers;

// Harness only: both strategies live in SumOfTwoIntegersSolution and are asserted
// against the same cases. Every row is a sum read by hand, inside LC 371's
// [-1000, 1000] for each addend:
// - -2 + 3 = 1 and -1 + -1 = -2 carry through every bit of the two's-complement word.
// - -1000 + -1000 = -2000 and 1000 + 1000 = 2000 are corners of the addend range.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SumOfTwoIntegersSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { -2, 3, 1 },
            { -1, -1, -2 },
            { -1000, -1000, -2000 },
            { 1000, 1000, 2000 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetSumByRippleCarryAdder_Cases_ReturnsArithmeticSum(
        int firstAddend, int secondAddend, int expected)
    {
        var actual = SumOfTwoIntegersSolution.GetSumByRippleCarryAdder(firstAddend, secondAddend);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetSumByBitwiseCarryLoop_Cases_ReturnsArithmeticSum(
        int firstAddend, int secondAddend, int expected)
    {
        var actual = SumOfTwoIntegersSolution.GetSumByBitwiseCarryLoop(firstAddend, secondAddend);

        Assert.Equal(expected, actual);
    }
}
