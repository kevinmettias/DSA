using DSAExperimentation.LeetCode.Solutions.FindTheKthCharacterInStringGameII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheKthCharacterInStringGameII;

public sealed partial class FindTheKthCharacterInStringGameIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindTheKthCharacterInStringGameIIReach.IsWithin(1, new int[20]));

    [Fact]
    public void IsWithin_OperationsLengthPastTheReach_IsFalse() => Assert.False(FindTheKthCharacterInStringGameIIReach.IsWithin(1, new int[21]));
}
