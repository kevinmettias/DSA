using DSAExperimentation.LeetCode.Solutions.CircleAndRectangleOverlapping;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CircleAndRectangleOverlapping;

public sealed partial class CircleAndRectangleOverlappingReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() =>
        Assert.True(CircleAndRectangleOverlappingReach.IsWithin(new Circle(1, 0, 0), new Rectangle(-3000, -3000, 3000, 3000)));

    [Fact]
    public void IsWithin_RectanglePastTheReach_IsFalse() =>
        Assert.False(CircleAndRectangleOverlappingReach.IsWithin(new Circle(1, 0, 0), new Rectangle(-3000, -3000, 3001, 3000)));
}
