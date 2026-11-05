using DSAExperimentation.LeetCode.Solutions.RectangleArea;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RectangleArea;

public sealed partial class RectangleAreaReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() =>
        Assert.True(RectangleAreaReach.IsWithin(new Rectangle(-1000, -1000, 1000, 1000), new Rectangle(-500, -500, 500, 500)));

    [Fact]
    public void IsWithin_BoxPastTheReach_IsFalse() =>
        Assert.False(RectangleAreaReach.IsWithin(new Rectangle(-1000, -1000, 1000, 1000), new Rectangle(0, 0, 1001, 1)));
}
