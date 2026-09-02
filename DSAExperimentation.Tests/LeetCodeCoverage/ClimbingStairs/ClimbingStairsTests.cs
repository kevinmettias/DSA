using DSAExperimentation.LeetCode.ClimbingStairs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClimbingStairs;

// Harness only. ClimbingStairsSolution owns the memoized recurrence; this file
// pins it to LeetCode's published examples plus the original five-step case.
public sealed class ClimbingStairsTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 2, 2 },
            { 3, 3 },
            { 5, 8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByMemoizedRecurrence_LeetCodeExamples_ReturnsDistinctClimbSequenceCount(
        int stepCount, int expected) =>
        Assert.Equal(expected, ClimbingStairsSolution.CountWaysByMemoizedRecurrence(stepCount));
}
