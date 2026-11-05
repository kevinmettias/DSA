using DSAExperimentation.LeetCode.Solutions.CountAlmostEqualPairsII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountAlmostEqualPairsII;

public sealed partial class CountAlmostEqualPairsIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountAlmostEqualPairsIIReach.IsWithin(new int[21]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(CountAlmostEqualPairsIIReach.IsWithin(new int[22]));
}
