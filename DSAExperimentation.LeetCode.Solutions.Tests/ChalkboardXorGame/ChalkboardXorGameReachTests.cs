using DSAExperimentation.LeetCode.Solutions.ChalkboardXorGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ChalkboardXorGame;

public sealed partial class ChalkboardXorGameReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ChalkboardXorGameReach.IsWithin(new int[13]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(ChalkboardXorGameReach.IsWithin(new int[14]));
}
