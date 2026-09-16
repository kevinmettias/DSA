using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReachableNodesWithRestrictionsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - how many nodes the walk from node 0 reaches
// without stepping on a restricted one - so a harness whose arms disagree is timing two different
// problems. Both return that count as a scalar, and each is handed the prepared restriction Set
// [GlobalSetup] builds, so one harness is safe to call twice in either order. Setup builds the tree
// and its restriction set from one fixed seed, so the same NodeCount must rebuild both.
public sealed partial class ReachableNodesWithRestrictionsBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DepthFirstFloodFill(), BuildHarness().DepthFirstFloodFill());

    [Fact]
    public void DepthFirstFloodFill_SeededRestrictedTree_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstFloodFill(), harness.DisjointSetUnionFind());
    }

    [Fact]
    public void DisjointSetUnionFind_SeededRestrictedTree_AgreesWithTheFloodFillArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.DepthFirstFloodFill());
    }

    private static ReachableNodesWithRestrictionsBenchmarks BuildHarness()
    {
        var harness = new ReachableNodesWithRestrictionsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
