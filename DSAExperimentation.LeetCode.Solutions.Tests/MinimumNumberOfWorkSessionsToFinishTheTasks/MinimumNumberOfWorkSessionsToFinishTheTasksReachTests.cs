using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfWorkSessionsToFinishTheTasks;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfWorkSessionsToFinishTheTasks;

public sealed partial class MinimumNumberOfWorkSessionsToFinishTheTasksReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumNumberOfWorkSessionsToFinishTheTasksReach.IsWithin(new int[8], 1));

    [Fact]
    public void IsWithin_TasksLengthPastTheReach_IsFalse() => Assert.False(MinimumNumberOfWorkSessionsToFinishTheTasksReach.IsWithin(new int[9], 1));
}
