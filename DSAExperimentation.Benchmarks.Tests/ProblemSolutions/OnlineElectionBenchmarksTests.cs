using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OnlineElectionBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one leader-per-query array - the per-query rescan against the precomputed binary
// search - so a harness whose arms disagree is timing two different problems. Setup draws the votes
// from one seeded Random and derives every query from its own timestamp, so the same Length must
// rebuild the same three arrays. The answers are per-query and ordered by the problem's own query
// order, so the default order-sensitive rendering is the right comparison.
public sealed partial class OnlineElectionBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PrecomputedBinarySearch()),
            AnswerText.Of(BuildHarness().PrecomputedBinarySearch()));

    [Fact]
    public void PerQueryRescan_SmallestLength_AgreesWithPrecomputedBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PrecomputedBinarySearch()),
            AnswerText.Of(harness.PerQueryRescan()));
    }

    [Fact]
    public void PrecomputedBinarySearch_SmallestLength_AgreesWithPerQueryRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PerQueryRescan()),
            AnswerText.Of(harness.PrecomputedBinarySearch()));
    }

    private static OnlineElectionBenchmarks BuildHarness()
    {
        var harness = new OnlineElectionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
