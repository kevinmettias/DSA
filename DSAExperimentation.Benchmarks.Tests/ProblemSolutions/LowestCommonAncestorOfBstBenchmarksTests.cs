using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LowestCommonAncestorOfBstBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems - which BenchmarkArmsTests checks. Each arm returns the ancestor node itself.
public sealed partial class LowestCommonAncestorOfBstBenchmarksTests
{
    private const int SmallestNodeCount = 1_024;

    // Both arms must actually reach a real ancestor rather than fall off the tree: a null answer
    // fails the comparison, and the seeded tree's ancestor of its two deepest insertions is not the
    // zero key.
    [Fact]
    public void BothArms_ReturnAnAncestorThatExists()
    {
        Assert.True(BuildHarness().BstValueComparison()?.Value > 0);
        Assert.True(BuildHarness().AncestryWalk()?.Value > 0);
    }

    private static LowestCommonAncestorOfBstBenchmarks BuildHarness()
    {
        var harness = new LowestCommonAncestorOfBstBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
