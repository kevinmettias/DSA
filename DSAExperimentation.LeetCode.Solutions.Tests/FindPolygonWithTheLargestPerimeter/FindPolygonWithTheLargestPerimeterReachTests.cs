using DSAExperimentation.LeetCode.Solutions.FindPolygonWithTheLargestPerimeter;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindPolygonWithTheLargestPerimeter;

public sealed partial class FindPolygonWithTheLargestPerimeterReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindPolygonWithTheLargestPerimeterReach.IsWithin(new int[6]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(FindPolygonWithTheLargestPerimeterReach.IsWithin(new int[7]));
}
