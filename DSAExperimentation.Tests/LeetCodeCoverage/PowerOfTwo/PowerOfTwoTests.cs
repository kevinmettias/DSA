using DSAExperimentation.LeetCode.PowerOfTwo;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfTwo;

// Harness only: both strategies live in PowerOfTwoSolution and are asserted
// against the same examples.
public sealed class PowerOfTwoTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 1, true },
            { 16, true },
            { 3, false },
            { 0, false },
            { -1, false },
            { 1_073_741_824, true }, // 2^30, the largest power of two an int can hold
            { int.MaxValue, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfTwoByRepeatedDivision_LeetCodeExamples_ReturnsExpected(int n, bool expected) =>
        Assert.Equal(expected, PowerOfTwoSolution.IsPowerOfTwoByRepeatedDivision(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfTwoByBitTrick_LeetCodeExamples_ReturnsExpected(int n, bool expected) =>
        Assert.Equal(expected, PowerOfTwoSolution.IsPowerOfTwoByBitTrick(n));
}
