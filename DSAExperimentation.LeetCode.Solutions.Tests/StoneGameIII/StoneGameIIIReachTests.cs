using DSAExperimentation.LeetCode.Solutions.StoneGameIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameIII;

public sealed partial class StoneGameIIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(StoneGameIIIReach.IsWithin(new int[25]));

    [Fact]
    public void IsWithin_StoneValueLengthPastTheReach_IsFalse() => Assert.False(StoneGameIIIReach.IsWithin(new int[26]));
}
