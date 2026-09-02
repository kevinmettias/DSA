using DSAExperimentation.LeetCode.GuessNumberHigherOrLower;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GuessNumberHigherOrLower;

// Harness only: both strategies live in GuessNumberHigherOrLowerSolution and are
// asserted against the same examples.
public sealed class GuessNumberHigherOrLowerTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 10, 6, 6 },
            { 1, 1, 1 },
            { 2, 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GuessNumberByLinearScan_LeetCodeExamples_ReturnsPickedNumber(int n, int pick, int expected) =>
        Assert.Equal(expected, GuessNumberHigherOrLowerSolution.GuessNumberByLinearScan(n, pick));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GuessNumberByBinarySearch_LeetCodeExamples_ReturnsPickedNumber(int n, int pick, int expected) =>
        Assert.Equal(expected, GuessNumberHigherOrLowerSolution.GuessNumberByBinarySearch(n, pick));
}
