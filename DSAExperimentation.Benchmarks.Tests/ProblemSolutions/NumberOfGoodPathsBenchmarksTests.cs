using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfGoodPathsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the per-pair path walk against this repo's own DisjointSet sweep
// over the edges in increasing order of their higher-valued endpoint - so a harness whose arms
// disagree is counting the good paths of two different trees. Setup draws the seeded node values and
// builds the path graph 0-1-2-...(n-1), so the same NodeCount must rebuild the same values and edges.
//
// Both arms return an int, so they are compared directly. Neither arm mutates the values array or the
// edge list, so one harness serves both arms in either order.
public sealed partial class NumberOfGoodPathsBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PairwisePathWalk(), BuildHarness().PairwisePathWalk());

    [Fact]
    public void DisjointSetSweep_AgreesWithPairwisePathWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwisePathWalk(), harness.DisjointSetSweep());
    }

    [Fact]
    public void PairwisePathWalk_AgreesWithDisjointSetSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetSweep(), harness.PairwisePathWalk());
    }

    private static NumberOfGoodPathsBenchmarks BuildHarness()
    {
        var harness = new NumberOfGoodPathsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
