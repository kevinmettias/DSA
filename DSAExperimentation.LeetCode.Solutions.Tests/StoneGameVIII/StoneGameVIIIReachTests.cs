using DSAExperimentation.LeetCode.Solutions.StoneGameVIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameVIII;

public sealed partial class StoneGameVIIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(StoneGameVIIIReach.IsWithin(new int[25]));

    [Fact]
    public void IsWithin_StonesLengthPastTheReach_IsFalse() => Assert.False(StoneGameVIIIReach.IsWithin(new int[26]));
}
