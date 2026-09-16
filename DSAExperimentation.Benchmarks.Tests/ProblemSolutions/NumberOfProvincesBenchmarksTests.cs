using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfProvincesBenchmarks (ARCHITECTURE 17.9): both arms count the connected
// components of the same adjacency matrix - a DFS flood fill against the DisjointSet union-find - so a
// harness whose arms disagree is timing two different questions. The province count is the problem's
// whole answer rather than a proxy. Setup builds the matrix through the shared seeded workload, so the
// same CityCount must rebuild the same matrix.
public sealed partial class NumberOfProvincesBenchmarksTests
{
    private const int SmallestCityCount = 50;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DepthFirstFloodFill(), BuildHarness().DepthFirstFloodFill());

    [Fact]
    public void DepthFirstFloodFill_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.DepthFirstFloodFill());
    }

    [Fact]
    public void DisjointSetUnionFind_AgreesWithDepthFirstFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstFloodFill(), harness.DisjointSetUnionFind());
    }

    private static NumberOfProvincesBenchmarks BuildHarness()
    {
        var harness = new NumberOfProvincesBenchmarks { CityCount = SmallestCityCount };
        harness.Setup();

        return harness;
    }
}
