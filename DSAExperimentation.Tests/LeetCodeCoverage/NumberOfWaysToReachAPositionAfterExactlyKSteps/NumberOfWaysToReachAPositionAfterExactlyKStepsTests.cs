using DSAExperimentation.LeetCode.NumberOfWaysToReachAPositionAfterExactlyKSteps;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToReachAPositionAfterExactlyKSteps;

// Harness only. Both strategies are
// NumberOfWaysToReachAPositionAfterExactlyKStepsSolution's - this file pins them to
// LeetCode's published examples plus the three edges the examples leave out: zero
// steps already on target, a return-to-start walk, and a distance whose parity
// cannot match the step count.
public sealed partial class NumberOfWaysToReachAPositionAfterExactlyKStepsTests
{
    public static TheoryData<int, int, int, int> Examples =>
        new()
        {
            { 1, 2, 3, 3 },
            { 2, 5, 10, 0 },
            { 4, 4, 0, 1 },
            { 0, 0, 2, 2 },
            { 0, 3, 3, 1 },
            { 5, 1, 5, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByUnmemoizedRecursion_LeetCodeExamples_ReturnsStepSequenceCount(
        int startPos, int endPos, int stepCount, int expected)
    {
        var actual = NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByUnmemoizedRecursion(
            startPos, endPos, stepCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByMemoizedRecursion_LeetCodeExamples_ReturnsStepSequenceCount(
        int startPos, int endPos, int stepCount, int expected)
    {
        var actual = NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByMemoizedRecursion(
            startPos, endPos, stepCount);

        Assert.Equal(expected, actual);
    }
}
