using DSAExperimentation.LeetCode.Solutions.BeautifulArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BeautifulArray;

public sealed partial class BeautifulArrayReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(BeautifulArrayReach.IsWithin(9));

    [Fact]
    public void IsWithin_LengthPastTheReach_IsFalse() => Assert.False(BeautifulArrayReach.IsWithin(10));
}
