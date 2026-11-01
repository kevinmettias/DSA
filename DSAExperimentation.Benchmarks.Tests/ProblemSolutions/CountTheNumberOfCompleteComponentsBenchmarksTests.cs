using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfCompleteComponentsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// an expected count derived from the workload's construction rather than from either arm.
//
// The edge array is private, and the workload's defining property - NodeCount / CliqueSize disjoint
// cliques, every one of them already complete - is exactly what the arms count, so the setup is
// asserted through that count being the documented clique count.
public sealed partial class CountTheNumberOfCompleteComponentsBenchmarksTests
{
    private const int SmallestNodeCount = 25;

    private const int CliqueSize = 25;

    private const int ExpectedCompleteComponents = SmallestNodeCount / CliqueSize;

    [Fact]
    public void AdjacencySetPairwiseScan_DisjointCompleteCliques_CountsEveryClique() =>
        Assert.Equal(ExpectedCompleteComponents, BuildHarness().AdjacencySetPairwiseScan());

    private static CountTheNumberOfCompleteComponentsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfCompleteComponentsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
