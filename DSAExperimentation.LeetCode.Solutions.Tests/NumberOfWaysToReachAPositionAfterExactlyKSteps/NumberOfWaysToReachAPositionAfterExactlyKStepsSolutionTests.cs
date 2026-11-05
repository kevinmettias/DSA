using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToReachAPositionAfterExactlyKSteps;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToReachAPositionAfterExactlyKSteps;

// Harness only. Both strategies are
// NumberOfWaysToReachAPositionAfterExactlyKStepsSolution's - this file pins them to
// edges LeetCode's examples leave out: zero steps already on target, a
// return-to-start walk, a distance equal to the step count so that only one walk
// reaches it, and a distance whose parity cannot match the step count. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class NumberOfWaysToReachAPositionAfterExactlyKStepsSolutionTests
{
    public static TheoryData<int, int, int, int> Cases =>
        new()
        {
            { 4, 4, 0, 1 },
            { 0, 0, 2, 2 },
            { 0, 3, 3, 1 },
            { 5, 1, 5, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWaysByUnmemoizedRecursion_Cases_ReturnsStepSequenceCount(
        int startPos, int endPos, int stepCount, int expected)
    {
        var actual = NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByUnmemoizedRecursion(
            startPos, endPos, stepCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWaysByMemoizedRecursion_Cases_ReturnsStepSequenceCount(
        int startPos, int endPos, int stepCount, int expected)
    {
        var actual = NumberOfWaysToReachAPositionAfterExactlyKStepsSolution.NumberOfWaysByMemoizedRecursion(
            startPos, endPos, stepCount);

        Assert.Equal(expected, actual);
    }
}
