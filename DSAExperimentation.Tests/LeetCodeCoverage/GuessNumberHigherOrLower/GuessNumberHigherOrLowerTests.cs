using DSAExperimentation.LeetCode.GuessNumberHigherOrLower;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GuessNumberHigherOrLower;

// Harness only: both strategies live in GuessNumberHigherOrLowerSolution and are
// asserted against the same examples.
public sealed partial class GuessNumberHigherOrLowerTests
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
    public void GuessNumberByLinearScan_LeetCodeExamples_ReturnsPickedNumber(
        int upperBound, int pick, int expected)
    {
        var actual = GuessNumberHigherOrLowerSolution.GuessNumberByLinearScan(upperBound, pick);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GuessNumberByBinarySearch_LeetCodeExamples_ReturnsPickedNumber(
        int upperBound, int pick, int expected)
    {
        var actual = GuessNumberHigherOrLowerSolution.GuessNumberByBinarySearch(upperBound, pick);

        Assert.Equal(expected, actual);
    }
}
