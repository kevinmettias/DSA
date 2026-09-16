using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromicPathQueriesInATreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same per-query flags - the ancestor walk against the LCA bitmask -
// so a harness whose arms disagree is timing two different problems. Setup draws the parent array,
// the labels and the queries from one seeded Random in call order, so the same NodeCount must rebuild
// the same three streams and the same built tree; neither arm mutates the tree it reads, so one
// harness is safe to call twice. The queries are random node pairs over a four-letter alphabet, so
// the flags are genuinely mixed rather than all one value.
public sealed partial class PalindromicPathQueriesInATreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().AncestorWalk()),
            AnswerText.Of(BuildHarness().AncestorWalk()));

    [Fact]
    public void AncestorWalk_SmallestNodeCount_AgreesWithLcaBitmask()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LcaBitmask()),
            AnswerText.Of(harness.AncestorWalk()));
    }

    [Fact]
    public void LcaBitmask_SmallestNodeCount_AgreesWithAncestorWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.AncestorWalk()),
            AnswerText.Of(harness.LcaBitmask()));
    }

    private static PalindromicPathQueriesInATreeBenchmarks BuildHarness()
    {
        var harness = new PalindromicPathQueriesInATreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
