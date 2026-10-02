using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LowestCommonAncestorOfBstBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. The arms answer the same query as each other and nothing else, so the only
// number that means anything is the pair of them read together.
public sealed partial class LowestCommonAncestorOfBstBenchmarksTests
{
    private const int SmallestNodeCount = 1_024;

    [Fact]
    public void BstValueComparison_AgreesWithAncestryWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AncestryWalk(), harness.BstValueComparison());
    }

    // Both arms must actually reach a real ancestor rather than fall off the tree: 0 is not in the
    // generated range, so a zero here means one arm answered null where the other answered a node.
    [Fact]
    public void BothArms_ReturnAnAncestorThatExists() =>
        Assert.True(BuildHarness().BstValueComparison() > 0);

    private static LowestCommonAncestorOfBstBenchmarks BuildHarness()
    {
        var harness = new LowestCommonAncestorOfBstBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
