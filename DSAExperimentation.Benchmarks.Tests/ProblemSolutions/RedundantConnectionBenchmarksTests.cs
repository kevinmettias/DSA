using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RedundantConnectionBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different problems.
// Setup's edge list is seeded, so the same node count must rebuild the same workload - otherwise two
// published numbers were never comparable in the first place.
public sealed partial class RedundantConnectionBenchmarksTests
{
    private const int SmallestNodeCount = 64;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PathSearch()),
            AnswerText.Of(BuildHarness().PathSearch()));

    [Fact]
    public void DisjointSet_AgreesWithPathSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PathSearch()),
            AnswerText.Of(harness.DisjointSet()));
    }

    private static RedundantConnectionBenchmarks BuildHarness()
    {
        var harness = new RedundantConnectionBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
