using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinCostToConnectAllPointsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the dense O(n^2) Prim sweep against Kruskal over the full
// Manhattan-distance edge set - so a harness whose arms disagree is timing two different problems. Both
// arms return the spanning tree's total cost as an int, so they are compared directly; a minimum
// spanning tree's total weight is unique even when several trees attain it, so the two strategies must
// land on the same number. Neither arm mutates the point cloud, so one harness is safe to read twice in
// either order, and Setup draws the points from one fixed seed, so the same PointCount must rebuild the
// same cloud and with it the same total.
public sealed partial class MinCostToConnectAllPointsBenchmarksTests
{
    private const int SmallestPointCount = 50;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DensePrim(), BuildHarness().DensePrim());

    [Fact]
    public void DensePrim_SeededPointCloud_AgreesWithKruskalMst()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KruskalMst(), harness.DensePrim());
    }

    [Fact]
    public void KruskalMst_SeededPointCloud_AgreesWithDensePrim()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DensePrim(), harness.KruskalMst());
    }

    private static MinCostToConnectAllPointsBenchmarks BuildHarness()
    {
        var harness = new MinCostToConnectAllPointsBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
