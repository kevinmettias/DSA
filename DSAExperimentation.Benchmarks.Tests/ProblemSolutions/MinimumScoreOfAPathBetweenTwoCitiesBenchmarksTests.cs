using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumScoreOfAPathBetweenTwoCitiesBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the smallest edge weight on the path joining the
// first and last city - so a harness whose arms disagree is timing two different problems. Both arms
// read the one road list [GlobalSetup] built from the same road list, so the comparison also pins that
// the flood-fill's adjacency walk and the disjoint-set merge order reach the same component. Setup
// draws the road weights from one fixed seed, so the same CityCount must rebuild the same road list.
public sealed partial class MinimumScoreOfAPathBetweenTwoCitiesBenchmarksTests
{
    private const int SmallestCityCount = 200;

    [Fact]
    public void Setup_SameCityCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BreadthFirstFloodFill(), BuildHarness().BreadthFirstFloodFill());

    [Fact]
    public void BreadthFirstFloodFill_SameRoadList_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.BreadthFirstFloodFill());
    }

    [Fact]
    public void DisjointSetUnionFind_SameRoadList_AgreesWithBreadthFirstFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BreadthFirstFloodFill(), harness.DisjointSetUnionFind());
    }

    private static MinimumScoreOfAPathBetweenTwoCitiesBenchmarks BuildHarness()
    {
        var harness = new MinimumScoreOfAPathBetweenTwoCitiesBenchmarks { CityCount = SmallestCityCount };
        harness.Setup();

        return harness;
    }
}
