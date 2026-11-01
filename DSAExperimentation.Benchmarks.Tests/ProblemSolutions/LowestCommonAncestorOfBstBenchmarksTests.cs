using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LowestCommonAncestorOfBstBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// holds the arms to each other's answer, but two arms that both fell off the tree would agree on
// null, so the baseline's answer is checked against the workload itself. Each arm returns the
// ancestor node itself.
public sealed partial class LowestCommonAncestorOfBstBenchmarksTests
{
    private const int SmallestNodeCount = 1_024;

    // The baseline must actually reach a real ancestor rather than fall off the tree: a null answer
    // fails the comparison, and the seeded tree's ancestor of its two deepest insertions is not the
    // zero key.
    [Fact]
    public void BstValueComparison_SeededTree_ReturnsAnAncestorThatExists() =>
        Assert.True(BuildHarness().BstValueComparison()?.Value > 0);

    private static LowestCommonAncestorOfBstBenchmarks BuildHarness()
    {
        var harness = new LowestCommonAncestorOfBstBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
