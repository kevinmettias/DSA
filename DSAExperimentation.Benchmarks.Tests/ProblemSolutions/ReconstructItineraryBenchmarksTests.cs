using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReconstructItineraryBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the itinerary that uses every ticket exactly once -
// so a harness whose arms disagree is timing two different problems. LeetCode pins the answer to
// the lexicographically smallest valid itinerary, and both arms are written to prefer the smallest
// destination at every branch, so the surviving route is a single pinned sequence and
// AnswerText.Of is the right rendering. Setup builds the ticket graph from one fixed seed, so the
// same TicketCount must rebuild the same graph.
public sealed partial class ReconstructItineraryBenchmarksTests
{
    private const int SmallestTicketCount = 200;

    [Fact]
    public void Setup_SameTicketCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScanSelection()),
            AnswerText.Of(BuildHarness().LinearScanSelection()));

    [Fact]
    public void LinearScanSelection_SeededTicketGraph_AgreesWithHeapSelection()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LinearScanSelection()),
            AnswerText.Of(harness.HeapSelection()));
    }

    [Fact]
    public void HeapSelection_SeededTicketGraph_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.HeapSelection()),
            AnswerText.Of(harness.LinearScanSelection()));
    }

    private static ReconstructItineraryBenchmarks BuildHarness()
    {
        var harness = new ReconstructItineraryBenchmarks { TicketCount = SmallestTicketCount };
        harness.Setup();

        return harness;
    }
}
