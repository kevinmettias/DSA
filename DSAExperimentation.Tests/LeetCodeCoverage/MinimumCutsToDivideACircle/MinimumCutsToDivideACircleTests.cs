using DSAExperimentation.LeetCode.MinimumCutsToDivideACircle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCutsToDivideACircle;

// Harness only. Both strategies are MinimumCutsToDivideACircleSolution's - this file
// states LeetCode's examples once and asserts each strategy against them, including
// the one-cut-at-a-time simulation that was previously a benchmark-only arm and so
// was never checked against an expected answer at all.
public sealed class MinimumCutsToDivideACircleTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            // n = 4 -> 2 diameter cuts.
            { 4, 2 },

            // n = 3 -> 3 radius cuts; no pair of opposite slices exists.
            { 3, 3 },

            // n = 1 -> the circle is already one slice.
            { 1, 0 },

            // The smallest n that a single diameter cut settles.
            { 2, 1 },

            // Both parities at the top of the problem's 1 <= n <= 100 range, where
            // the simulation actually loops: the largest even n, and the largest odd.
            { 100, 50 },
            { 99, 99 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfCutsBySimulation_LeetCodeExamples_ReturnsExpectedMinimumCuts(
        int sliceCount, int expected) =>
        Assert.Equal(expected, MinimumCutsToDivideACircleSolution.NumberOfCutsBySimulation(sliceCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfCutsByClosedFormParity_LeetCodeExamples_ReturnsExpectedMinimumCuts(
        int sliceCount, int expected) =>
        Assert.Equal(expected, MinimumCutsToDivideACircleSolution.NumberOfCutsByClosedFormParity(sliceCount));
}
