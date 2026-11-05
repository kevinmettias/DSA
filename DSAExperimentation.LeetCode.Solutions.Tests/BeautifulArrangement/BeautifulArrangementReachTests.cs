using DSAExperimentation.LeetCode.Solutions.BeautifulArrangement;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BeautifulArrangement;

public sealed partial class BeautifulArrangementReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(BeautifulArrangementReach.IsWithin(9));

    [Fact]
    public void IsWithin_SizePastTheReach_IsFalse() => Assert.False(BeautifulArrangementReach.IsWithin(10));
}
