using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IncrementalEvenWeightedCycleQueriesBenchmarks (ARCHITECTURE 17.9): both
// arms are competing strategies for the same question - a BFS per edge against a disjoint set
// that prunes the BFS to edges whose endpoints are still disconnected - so a harness whose arms
// disagree is timing two different problems. Both arms answer with the single count of edges
// that closed an even-weight cycle, an int, compared directly. The edge stream is built from a
// seeded Random, so the same EdgeCount must rebuild the same stream and therefore the same count.
public sealed partial class IncrementalEvenWeightedCycleQueriesBenchmarksTests
{
    private const int SmallestEdgeCount = 500;

    [Fact]
    public void Setup_SameEdgeCount_RebuildsTheSameEdgeStream() =>
        Assert.Equal(BuildHarness().BruteForceBfs(), BuildHarness().BruteForceBfs());

    [Fact]
    public void BruteForceBfs_EvenWeightedCycleCount_AgreesWithDisjointSetPrunedBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetPrunedBfs(), harness.BruteForceBfs());
    }

    [Fact]
    public void DisjointSetPrunedBfs_EvenWeightedCycleCount_AgreesWithBruteForceBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceBfs(), harness.DisjointSetPrunedBfs());
    }

    private static IncrementalEvenWeightedCycleQueriesBenchmarks BuildHarness()
    {
        var harness = new IncrementalEvenWeightedCycleQueriesBenchmarks { EdgeCount = SmallestEdgeCount };
        harness.Setup();

        return harness;
    }
}
