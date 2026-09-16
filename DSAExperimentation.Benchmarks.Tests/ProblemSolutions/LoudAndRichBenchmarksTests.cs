using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LoudAndRichBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a fresh per-person walk over the richer-than edges against one
// Kahn topological pass that threads each person's best answer down the same edges - so a harness
// whose arms disagree is timing two different problems. Answers come back one per person in id
// order, which is part of the answer: the quietest person for person i belongs at index i, so
// AnswerText.Of and not OfUnorderedSet is the rendering that keeps each answer scored against its
// own person. Setup builds the richer-than DAG (edges always pointing to a higher id, hence
// acyclic) and draws the quiet values from one fixed seed, so the same PersonCount must rebuild
// both.
public sealed partial class LoudAndRichBenchmarksTests
{
    private const int SmallestPersonCount = 50;

    [Fact]
    public void Setup_SamePersonCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            AnswerText.Of(BuildHarness().NaivePerPersonWalk()),
            AnswerText.Of(BuildHarness().NaivePerPersonWalk()));
        Assert.Equal(
            AnswerText.Of(BuildHarness().TopologicalDpPass()),
            AnswerText.Of(BuildHarness().TopologicalDpPass()));
    }

    [Fact]
    public void NaivePerPersonWalk_CappedFanOutDag_AgreesWithTopologicalDpPass()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TopologicalDpPass()),
            AnswerText.Of(harness.NaivePerPersonWalk()));
    }

    [Fact]
    public void TopologicalDpPass_CappedFanOutDag_AgreesWithNaivePerPersonWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.NaivePerPersonWalk()),
            AnswerText.Of(harness.TopologicalDpPass()));
    }

    private static LoudAndRichBenchmarks BuildHarness()
    {
        var harness = new LoudAndRichBenchmarks { PersonCount = SmallestPersonCount };
        harness.Setup();

        return harness;
    }
}
