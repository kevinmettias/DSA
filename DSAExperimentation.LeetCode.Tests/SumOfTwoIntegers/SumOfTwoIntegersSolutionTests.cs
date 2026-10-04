using DSAExperimentation.LeetCode.SumOfTwoIntegers;

namespace DSAExperimentation.LeetCode.Tests.SumOfTwoIntegers;

// Harness only: both strategies live in SumOfTwoIntegersSolution and are asserted
// against the same examples. The first two rows are LeetCode's published examples;
// the rest are sums read by hand, inside LC 371's [-1000, 1000] for each addend:
// - -2 + 3 = 1 and -1 + -1 = -2 carry through every bit of the two's-complement word.
// - 0 + 0 = 0 has no bit set at all.
// - -1000 + -1000 = -2000, 1000 + -1000 = 0 and 1000 + 1000 = 2000 are the corners
//   of the addend range.
public sealed partial class SumOfTwoIntegersSolutionTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 1, 2, 3 },
            { 2, 3, 5 },
            { -2, 3, 1 },
            { -1, -1, -2 },
            { 0, 0, 0 },
            { -1000, -1000, -2000 },
            { 1000, -1000, 0 },
            { 1000, 1000, 2000 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetSumByRippleCarryAdder_LeetCodeExamples_ReturnsArithmeticSum(
        int firstAddend, int secondAddend, int expected)
    {
        var actual = SumOfTwoIntegersSolution.GetSumByRippleCarryAdder(firstAddend, secondAddend);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetSumByBitwiseCarryLoop_LeetCodeExamples_ReturnsArithmeticSum(
        int firstAddend, int secondAddend, int expected)
    {
        var actual = SumOfTwoIntegersSolution.GetSumByBitwiseCarryLoop(firstAddend, secondAddend);

        Assert.Equal(expected, actual);
    }
}
