using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfWorkSessionsToFinishTheTasks;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfWorkSessionsToFinishTheTasks;

// Harness only. The feasible-subset table is
// LeetCode.MinimumNumberOfWorkSessionsToFinishTheTasks' FeasibleSessionMasks and
// both strategies are MinimumNumberOfWorkSessionsToFinishTheTasksSolution's - this
// file just pins them to the degenerate shapes
// (a single task, a budget that forces one task per session, a split that has to
// beat the greedy "fill each session as full as possible" answer). The unmemoized
// arm is asserted here for the first time: before the migration it existed only as
// the benchmark's baseline, which nothing checked.
public sealed partial class MinimumNumberOfWorkSessionsToFinishTheTasksSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [5], 5, 1 },
            { [1, 1, 1, 1], 1, 4 },
            { [2, 2, 3], 4, 2 },
            { [9, 8, 8, 4, 6], 14, 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSessionsByUnmemoizedRecursion_Cases_ReturnsFewestSessions(
        int[] tasks, int sessionTime, int expected)
    {
        var actual = MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByUnmemoizedRecursion(tasks, sessionTime);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSessionsByMemoizedBitmaskDp_Cases_ReturnsFewestSessions(
        int[] tasks, int sessionTime, int expected)
    {
        var actual = MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByMemoizedBitmaskDp(tasks, sessionTime);
        Assert.Equal(expected, actual);
    }
}
