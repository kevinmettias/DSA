using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BricksFallingWhenHitBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - replaying forward and recomputing roof connectivity by BFS
// after every hit against the reverse-time disjoint-set trick - so a harness whose arms disagree is
// timing two different problems, and the disagreement shows up per hit, not as a summary.
// AnswerText.Of, not OfUnorderedSet: the answers come back one per hit in hit order, and a set
// rendering would score a result against the wrong hit. Setup builds the wall and its hit list from
// one fixed seed, so the same Size must rebuild the same pair.
public sealed partial class BricksFallingWhenHitBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ReplayForwardWithBfs()),
            AnswerText.Of(BuildHarness().ReplayForwardWithBfs()));

    [Fact]
    public void ReplayForwardWithBfs_SeededWallAndHits_AgreesWithReverseTimeDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ReverseTimeDisjointSet()), AnswerText.Of(harness.ReplayForwardWithBfs()));
    }

    [Fact]
    public void ReverseTimeDisjointSet_SeededWallAndHits_AgreesWithReplayForwardWithBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ReplayForwardWithBfs()), AnswerText.Of(harness.ReverseTimeDisjointSet()));
    }

    private static BricksFallingWhenHitBenchmarks BuildHarness()
    {
        var harness = new BricksFallingWhenHitBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
