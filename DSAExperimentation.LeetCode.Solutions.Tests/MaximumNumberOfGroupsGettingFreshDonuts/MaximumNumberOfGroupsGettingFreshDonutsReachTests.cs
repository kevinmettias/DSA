using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfGroupsGettingFreshDonuts;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfGroupsGettingFreshDonuts;

public sealed partial class MaximumNumberOfGroupsGettingFreshDonutsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MaximumNumberOfGroupsGettingFreshDonutsReach.IsWithin(1, new int[9]));

    [Fact]
    public void IsWithin_GroupsLengthPastTheReach_IsFalse() => Assert.False(MaximumNumberOfGroupsGettingFreshDonutsReach.IsWithin(1, new int[10]));
}
