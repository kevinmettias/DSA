using DSAExperimentation.Domain.Mathematics.Geometry;

namespace DSAExperimentation.Tests.Domain.Mathematics.Geometry;

public sealed partial class EuclideanPolygonsTests
{
    // Each row's longest side against the others' total: shorter closes.
    public static TheoryData<int, long> Closing => new()
    {
        { 5, 7 },                           // 3, 4 and 5
        { 12, 13 },                         // 1, 2, 5, 5 and 12
        { int.MaxValue, 3L * int.MaxValue },
    };

    // Equal leaves a flat figure, and longer leaves the sides unable to meet.
    public static TheoryData<int, long> NotClosing => new()
    {
        { 7, 7 },   // 3, 4 and 7 lie flat
        { 8, 7 },   // 3, 4 and 8 cannot meet
        { 13, 13 },
    };

    [Fact]
    public void FewestSides_IsATrianglesThree() => Assert.Equal(3, EuclideanPolygons.FewestSides);

    [Theory]
    [MemberData(nameof(Closing))]
    public void CanClose_LongestSideShorterThanTheRest_IsTrue(int longestSide, long otherSidesTotal) =>
        Assert.True(EuclideanPolygons.CanClose(longestSide, otherSidesTotal));

    [Theory]
    [MemberData(nameof(NotClosing))]
    public void CanClose_LongestSideNoShorterThanTheRest_IsFalse(int longestSide, long otherSidesTotal) =>
        Assert.False(EuclideanPolygons.CanClose(longestSide, otherSidesTotal));
}
