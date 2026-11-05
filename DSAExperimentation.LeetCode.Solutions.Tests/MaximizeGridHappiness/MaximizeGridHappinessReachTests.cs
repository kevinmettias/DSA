using DSAExperimentation.LeetCode.Solutions.MaximizeGridHappiness;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximizeGridHappiness;

public sealed partial class MaximizeGridHappinessReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MaximizeGridHappinessReach.IsWithin(1, 1, 4, 1));

    [Fact]
    public void IsWithin_IntrovertsCountPastTheReach_IsFalse() => Assert.False(MaximizeGridHappinessReach.IsWithin(1, 1, 5, 1));
}
