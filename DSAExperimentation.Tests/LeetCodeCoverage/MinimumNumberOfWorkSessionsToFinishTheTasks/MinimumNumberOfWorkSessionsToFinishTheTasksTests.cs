using DSAExperimentation.LeetCode.MinimumNumberOfWorkSessionsToFinishTheTasks;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfWorkSessionsToFinishTheTasks;

// Harness only. The feasible-subset table is
// LeetCode.MinimumNumberOfWorkSessionsToFinishTheTasks' FeasibleSessionMasks and
// both strategies are MinimumNumberOfWorkSessionsToFinishTheTasksSolution's - this
// file just pins them to LeetCode's published examples plus the degenerate shapes
// (a single task, a budget that forces one task per session, a split that has to
// beat the greedy "fill each session as full as possible" answer). The unmemoized
// arm is asserted here for the first time: before the migration it existed only as
// the benchmark's baseline, which nothing checked.
public sealed partial class MinimumNumberOfWorkSessionsToFinishTheTasksTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 3], 3, 2 },
            { [3, 1, 3, 1, 1], 8, 2 },
            { [1, 2, 3, 4, 5], 15, 1 },
            { [5], 5, 1 },
            { [1, 1, 1, 1], 1, 4 },
            { [2, 2, 3], 4, 2 },
            { [9, 8, 8, 4, 6], 14, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSessionsByUnmemoizedRecursion_LeetCodeExamples_ReturnsFewestSessions(
        int[] tasks, int sessionTime, int expected)
    {
        var actual = MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByUnmemoizedRecursion(tasks, sessionTime);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSessionsByMemoizedBitmaskDp_LeetCodeExamples_ReturnsFewestSessions(
        int[] tasks, int sessionTime, int expected)
    {
        var actual = MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByMemoizedBitmaskDp(tasks, sessionTime);
        Assert.Equal(expected, actual);
    }
}
