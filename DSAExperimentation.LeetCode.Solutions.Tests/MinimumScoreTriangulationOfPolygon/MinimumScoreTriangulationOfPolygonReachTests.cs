using DSAExperimentation.LeetCode.Solutions.MinimumScoreTriangulationOfPolygon;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumScoreTriangulationOfPolygon;

public sealed partial class MinimumScoreTriangulationOfPolygonReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumScoreTriangulationOfPolygonReach.IsWithin(new int[16]));

    [Fact]
    public void IsWithin_ValuesLengthPastTheReach_IsFalse() => Assert.False(MinimumScoreTriangulationOfPolygonReach.IsWithin(new int[17]));
}
