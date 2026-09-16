using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TimeNeededToBuyTicketsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the queue replay against the O(n) closed-form sum -
// so a harness whose arms disagree is timing two different problems. Both arms return the seconds
// the target person waits as an int, so they are compared directly. Setup draws the ticket counts
// from a fixed seed and derives the target person from Length, so the same Length must rebuild the
// same line.
public sealed partial class TimeNeededToBuyTicketsBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameLine() =>
        Assert.Equal(BuildHarness().QueueSimulation(), BuildHarness().QueueSimulation());

    [Fact]
    public void QueueSimulation_SmallestLength_AgreesWithClosedFormSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ClosedFormSum(), harness.QueueSimulation());
    }

    [Fact]
    public void ClosedFormSum_SmallestLength_AgreesWithQueueSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueueSimulation(), harness.ClosedFormSum());
    }

    private static TimeNeededToBuyTicketsBenchmarks BuildHarness()
    {
        var harness = new TimeNeededToBuyTicketsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
