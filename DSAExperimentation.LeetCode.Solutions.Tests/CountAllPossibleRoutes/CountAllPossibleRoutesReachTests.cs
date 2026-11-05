using DSAExperimentation.LeetCode.Solutions.CountAllPossibleRoutes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountAllPossibleRoutes;

public sealed partial class CountAllPossibleRoutesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountAllPossibleRoutesReach.IsWithin(new int[8], 1, 1, 1));

    [Fact]
    public void IsWithin_LocationsLengthPastTheReach_IsFalse() => Assert.False(CountAllPossibleRoutesReach.IsWithin(new int[9], 1, 1, 1));
}
