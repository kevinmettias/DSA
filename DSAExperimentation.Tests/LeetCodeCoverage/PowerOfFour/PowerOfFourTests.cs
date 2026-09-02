using DSAExperimentation.LeetCode.PowerOfFour;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfFour;

// Harness only: both strategies live in PowerOfFourSolution and are asserted
// against the same examples.
public sealed class PowerOfFourTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 1, true },
            { 16, true },
            { 5, false },
            { 0, false },
            { -4, false },
            { 1073741824, true }, // 4^15, the largest power of four an int holds
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfFourByDivisionLoop_LeetCodeExamples_ReturnsExpected(int n, bool expected) =>
        Assert.Equal(expected, PowerOfFourSolution.IsPowerOfFourByDivisionLoop(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfFourByBinarySearch_LeetCodeExamples_ReturnsExpected(int n, bool expected) =>
        Assert.Equal(expected, PowerOfFourSolution.IsPowerOfFourByBinarySearch(n));
}
