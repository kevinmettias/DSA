using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfCompleteComponentsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a pairwise adjacency-set scan against a
// union-find tally - so a harness whose arms disagree is timing two different problems, not two ways
// of answering one. Setup derives the edge array from NodeCount alone, so the same NodeCount must
// rebuild the same workload; otherwise two published numbers were never comparable in the first
// place.
//
// The edge array is private, and the workload's defining property - NodeCount / CliqueSize disjoint
// cliques, every one of them already complete - is exactly what the arms count, so the setup is
// asserted through that count being the documented clique count.
public sealed partial class CountTheNumberOfCompleteComponentsBenchmarksTests
{
    private const int SmallestNodeCount = 250;

    private const int CliqueSize = 25;

    private const int ExpectedCompleteComponents = SmallestNodeCount / CliqueSize;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameCliques()
    {
        Assert.Equal(ExpectedCompleteComponents, BuildHarness().AdjacencySetPairwiseScan());
        Assert.Equal(
            BuildHarness().AdjacencySetPairwiseScan(),
            BuildHarness().AdjacencySetPairwiseScan());
    }

    [Fact]
    public void AdjacencySetPairwiseScan_DisjointCompleteCliques_AgreesWithDisjointSetTally()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetTally(), harness.AdjacencySetPairwiseScan());
    }

    [Fact]
    public void DisjointSetTally_DisjointCompleteCliques_AgreesWithAdjacencySetPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AdjacencySetPairwiseScan(), harness.DisjointSetTally());
    }

    private static CountTheNumberOfCompleteComponentsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfCompleteComponentsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
