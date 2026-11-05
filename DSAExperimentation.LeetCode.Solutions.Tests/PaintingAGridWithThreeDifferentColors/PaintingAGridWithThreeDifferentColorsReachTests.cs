using DSAExperimentation.LeetCode.Solutions.PaintingAGridWithThreeDifferentColors;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PaintingAGridWithThreeDifferentColors;

public sealed partial class PaintingAGridWithThreeDifferentColorsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(PaintingAGridWithThreeDifferentColorsReach.IsWithin(1, 7));

    [Fact]
    public void IsWithin_ColumnsPastTheReach_IsFalse() => Assert.False(PaintingAGridWithThreeDifferentColorsReach.IsWithin(1, 8));
}
