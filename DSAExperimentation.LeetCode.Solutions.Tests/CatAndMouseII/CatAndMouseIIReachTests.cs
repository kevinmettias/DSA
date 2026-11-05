using DSAExperimentation.LeetCode.Solutions.CatAndMouseII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CatAndMouseII;

public sealed partial class CatAndMouseIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CatAndMouseIIReach.IsWithin(new string[1], 1, 1));

    [Fact]
    public void IsWithin_GridLengthPastTheReach_IsFalse() => Assert.False(CatAndMouseIIReach.IsWithin(new string[2], 1, 1));
}
