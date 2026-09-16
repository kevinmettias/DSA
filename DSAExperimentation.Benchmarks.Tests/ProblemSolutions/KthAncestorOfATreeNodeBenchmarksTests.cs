using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthAncestorOfATreeNodeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the kth ancestor of every one of a fixed query batch -
// so a harness whose arms disagree is timing two different problems. Each arm answers with the
// SUM of every query's ancestor value, over a seeded batch of a million queries, so the
// comparison is on the aggregate the arms actually computed rather than on a sampled query.
//
// The precompute deliberately stays inside PrecomputedAncestorChains rather than in
// [GlobalSetup], which is the trade being measured; both arms seed and reset nothing of their
// own, so one harness instance is safe to call twice in either order.
public sealed partial class KthAncestorOfATreeNodeBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameParentArrayAndQueries() =>
        Assert.Equal(
            BuildHarness().WalkParentArrayPerQuery(),
            BuildHarness().WalkParentArrayPerQuery());

    [Fact]
    public void WalkParentArrayPerQuery_SeededHeapShapedTree_AgreesWithPrecomputedAncestorChains()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrecomputedAncestorChains(), harness.WalkParentArrayPerQuery());
    }

    [Fact]
    public void PrecomputedAncestorChains_SeededHeapShapedTree_AgreesWithWalkParentArrayPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.WalkParentArrayPerQuery(), harness.PrecomputedAncestorChains());
    }

    private static KthAncestorOfATreeNodeBenchmarks BuildHarness()
    {
        var harness = new KthAncestorOfATreeNodeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
