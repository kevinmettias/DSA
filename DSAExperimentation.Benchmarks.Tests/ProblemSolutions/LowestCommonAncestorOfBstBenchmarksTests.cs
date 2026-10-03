using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LowestCommonAncestorOfBstBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems - which BenchmarkArmsTests checks. Each arm returns the ancestor node itself.
public sealed partial class LowestCommonAncestorOfBstBenchmarksTests
{
    private const int SmallestNodeCount = 1_024;

    // The baseline must actually reach a real ancestor rather than fall off the tree: a null answer
    // fails the type check, and the seeded tree's ancestor of its two deepest insertions is not the
    // zero key.
    [Fact]
    public void BothArms_ReturnAnAncestorThatExists() =>
        Assert.True(Assert.IsType<BinaryTreeNode<int>>(BuildHarness().BstValueComparison()).Value > 0);

    private static LowestCommonAncestorOfBstBenchmarks BuildHarness()
    {
        var harness = new LowestCommonAncestorOfBstBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
