using DSAExperimentation.LeetCode.PowerOfThree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfThree;

// Harness only: both strategies live in PowerOfThreeSolution and are asserted
// against the same examples.
public sealed class PowerOfThreeTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 1, true },
            { 3, true },
            { 9, true },
            { 27, true },
            { 45, false },
            { 0, false },
            { -3, false },
            { 1162261467, true }, // 3^19, the largest power of three that fits in an int
            { 1162261466, false },
            { int.MaxValue, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfThreeByDivisionLoop_LeetCodeExamples_ReturnsExpected(int n, bool expected) =>
        Assert.Equal(expected, PowerOfThreeSolution.IsPowerOfThreeByDivisionLoop(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfThreeByBinarySearch_LeetCodeExamples_ReturnsExpected(int n, bool expected) =>
        Assert.Equal(expected, PowerOfThreeSolution.IsPowerOfThreeByBinarySearch(n));
}
