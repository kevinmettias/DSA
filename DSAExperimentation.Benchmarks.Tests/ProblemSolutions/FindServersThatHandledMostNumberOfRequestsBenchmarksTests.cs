using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindServersThatHandledMostNumberOfRequestsBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - a walk around the ring for the next free
// server against this repo's FenwickTree ceiling query paired with a Heap of busy servers - so a
// harness whose arms disagree has dispatched the same arrivals to two different server sets. Both
// arms report the busiest servers in ascending index order, which is the order the problem fixes, so
// they are compared as ordered sequences rather than as a set.
//
// Setup's loads run up to three times the server count, so requests routinely outlive many later
// arrivals and real contention and wraparound occur, and the seeded arrivals and loads are the same
// for a given ServerCount - the same parameters must rebuild the same dispatch.
public sealed partial class FindServersThatHandledMostNumberOfRequestsBenchmarksTests
{
    // The smaller of Setup's [Params(50, 400)] server counts.
    private const int SmallestServerCount = 50;

    [Fact]
    public void Setup_SameServerCount_RebuildsTheSameBusiestServers() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScanRing()),
            AnswerText.Of(BuildHarness().LinearScanRing()));

    [Fact]
    public void LinearScanRing_ContendedArrivalSequence_AgreesWithFenwickCeilingAndHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.FenwickCeilingAndHeap()),
            AnswerText.Of(harness.LinearScanRing()));
    }

    [Fact]
    public void FenwickCeilingAndHeap_ContendedArrivalSequence_AgreesWithLinearScanRing()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LinearScanRing()),
            AnswerText.Of(harness.FenwickCeilingAndHeap()));
    }

    private static FindServersThatHandledMostNumberOfRequestsBenchmarks BuildHarness()
    {
        var harness = new FindServersThatHandledMostNumberOfRequestsBenchmarks { ServerCount = SmallestServerCount };
        harness.Setup();

        return harness;
    }
}
