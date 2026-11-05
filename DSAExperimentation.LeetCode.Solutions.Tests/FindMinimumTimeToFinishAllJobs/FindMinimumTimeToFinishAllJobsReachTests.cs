using DSAExperimentation.LeetCode.Solutions.FindMinimumTimeToFinishAllJobs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindMinimumTimeToFinishAllJobs;

public sealed partial class FindMinimumTimeToFinishAllJobsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindMinimumTimeToFinishAllJobsReach.IsWithin(new int[6], 1));

    [Fact]
    public void IsWithin_JobsLengthPastTheReach_IsFalse() => Assert.False(FindMinimumTimeToFinishAllJobsReach.IsWithin(new int[7], 1));
}
