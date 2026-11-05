using DSAExperimentation.LeetCode.Solutions.FindAllGoodStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindAllGoodStrings;

public sealed partial class FindAllGoodStringsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindAllGoodStringsReach.IsWithin(3, default, default, default));

    [Fact]
    public void IsWithin_StringLengthPastTheReach_IsFalse() => Assert.False(FindAllGoodStringsReach.IsWithin(4, default, default, default));
}
