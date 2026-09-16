using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PropertiesGraphWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3493's
// rows being drawn from the problem's own value range at a column count well under it, so pairs land a
// mix of above- and below-threshold intersections instead of the graph trivially collapsing.
public sealed partial class PropertiesGraphWorkloadsTests
{
    private const int RowCount = 20;
    private const int ColumnCount = 20;
    private const int Seed = 3493; // LC problem number
    private const int ValueRange = 100;
    private const int IntersectionThreshold = 6; // the threshold PropertiesGraphBenchmarks' K names
    private const int FewestPairsOnEitherSide = 1;

    [Fact]
    public void BuildProperties_RowCount_ReturnsOneRowPerPositionWithTheDocumentedColumnCount()
    {
        var properties = PropertiesGraphWorkloads.BuildProperties(RowCount, ColumnCount, Seed);

        Assert.Equal(RowCount, properties.Length);
        Assert.All(properties, row => Assert.Equal(ColumnCount, row.Length));
    }

    [Fact]
    public void BuildProperties_EveryValue_StaysInsideLeetCodeValueRange() =>
        Assert.All(
            PropertiesGraphWorkloads.BuildProperties(RowCount, ColumnCount, Seed).SelectMany(row => row),
            value => Assert.InRange(value, 1, ValueRange));

    // The comment's claim is about the threshold K, and it is the threshold that decides whether a pair
    // becomes an edge: pairs have to land on BOTH sides of it, or one of the two strategies' edge sets
    // is empty and the measurement says nothing about the interesting case.
    [Fact]
    public void BuildProperties_RowPairs_LandOnBothSidesOfTheIntersectionThreshold()
    {
        var properties = PropertiesGraphWorkloads.BuildProperties(RowCount, ColumnCount, Seed);
        var sharedCounts = RowPairs(properties)
            .Select(pair => pair.First.Intersect(pair.Second).Count())
            .ToList();

        Assert.InRange(
            sharedCounts.Count(shared => shared >= IntersectionThreshold),
            FewestPairsOnEitherSide,
            sharedCounts.Count - FewestPairsOnEitherSide);
        Assert.InRange(
            sharedCounts.Count(shared => shared < IntersectionThreshold),
            FewestPairsOnEitherSide,
            sharedCounts.Count - FewestPairsOnEitherSide);
    }

    [Fact]
    public void BuildProperties_SameSeed_ReturnsTheSameProperties() =>
        Assert.Equal(
            AnswerText.Of(PropertiesGraphWorkloads.BuildProperties(RowCount, ColumnCount, Seed)),
            AnswerText.Of(PropertiesGraphWorkloads.BuildProperties(RowCount, ColumnCount, Seed)));

    private static IEnumerable<(int[] First, int[] Second)> RowPairs(int[][] properties) =>
        from first in Enumerable.Range(0, properties.Length)
        from second in Enumerable.Range(first + 1, properties.Length - first - 1)
        select (properties[first], properties[second]);
}
