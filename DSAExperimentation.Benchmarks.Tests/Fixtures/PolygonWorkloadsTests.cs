using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PolygonWorkloads (ARCHITECTURE 17.7). The reading depends on the two "pick sides,
// maximize perimeter" problems (LC 2971 and LC 976) getting the same shape of input - a bag of positive
// side lengths - at whatever size each one's own baseline can afford, so the sorted arm's O(n log n)
// path is exercised on identical data by both.
public sealed partial class PolygonWorkloadsTests
{
    private const int SmallestSideCount = 16; // the subset-enumerating baseline's affordable size
    private const int LargestSideCount = 80;
    private const int SmallestSideSeed = 2971; // LC problem number
    private const int LargestSideSeed = 976; // LC problem number
    private const int MaxSideExclusive = 1_000;

    [Fact]
    public void BuildSides_SideCount_ReturnsOneSidePerPosition()
    {
        Assert.Equal(
            SmallestSideCount,
            PolygonWorkloads.BuildSides(SmallestSideCount, SmallestSideSeed).Length);
        Assert.Equal(
            LargestSideCount,
            PolygonWorkloads.BuildSides(LargestSideCount, LargestSideSeed).Length);
    }

    [Fact]
    public void BuildSides_EverySide_IsAPositiveLengthBelowTheDocumentedUpperBound()
    {
        Assert.All(
            PolygonWorkloads.BuildSides(SmallestSideCount, SmallestSideSeed),
            side => Assert.InRange(side, 1, MaxSideExclusive - 1));
        Assert.All(
            PolygonWorkloads.BuildSides(LargestSideCount, LargestSideSeed),
            side => Assert.InRange(side, 1, MaxSideExclusive - 1));
    }

    // Equal sides would make the sorted arm's ordering invisible - the same input under a permuted
    // draw - so this asserts the draw actually varies across the bag it hands both problems.
    [Fact]
    public void BuildSides_Sides_AreNotAllTheSameLength() =>
        Assert.NotEqual(
            PolygonWorkloads.BuildSides(SmallestSideCount, SmallestSideSeed).Min(),
            PolygonWorkloads.BuildSides(SmallestSideCount, SmallestSideSeed).Max());

    [Fact]
    public void BuildSides_SameSeed_ReturnsTheSameSides() =>
        Assert.Equal(
            AnswerText.Of(PolygonWorkloads.BuildSides(SmallestSideCount, SmallestSideSeed)),
            AnswerText.Of(PolygonWorkloads.BuildSides(SmallestSideCount, SmallestSideSeed)));
}
