using DSAExperimentation.LeetCode.Solutions.StoneGameVII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameVII;

public sealed partial class StoneGameVIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(StoneGameVIIReach.IsWithin(new int[13]));

    [Fact]
    public void IsWithin_StonesLengthPastTheReach_IsFalse() => Assert.False(StoneGameVIIReach.IsWithin(new int[14]));
}
