using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumMovesToCaptureTheQueenBenchmarks (ARCHITECTURE 17.9): both arms
// are MinimumMovesToCaptureTheQueenSolution's, the same methods
// MinimumMovesToCaptureTheQueenTests proves correct, and both answer the same question for each
// [Params]-sized batch of random boards.
//
// The agreement asserted here is weak by construction: each arm returns the SUM of its answers
// over the whole batch, not a per-query array, so a disagreement on one query is only caught
// when it does not cancel against another. The assertion is still worth having - a batch total
// that moves means at least one query's answer moved - but it is the batch total that agrees,
// not the individual queries, and fixing that is a harness decision rather than a test one.
public sealed partial class MinimumMovesToCaptureTheQueenBenchmarksTests
{
    // The smallest declared [Params] value: every query is O(1) on the fixed 8x8 board, so a
    // smaller batch reaches the same comparison more cheaply.
    private const int SmallestBatchSize = 1_000;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().DestinationEnumeration(),
            BuildHarness().DestinationEnumeration());

    [Fact]
    public void DestinationEnumeration_AgreesWithLineOfSight()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LineOfSight(), harness.DestinationEnumeration());
    }

    [Fact]
    public void LineOfSight_AgreesWithDestinationEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DestinationEnumeration(), harness.LineOfSight());
    }

    private static MinimumMovesToCaptureTheQueenBenchmarks BuildHarness()
    {
        var harness = new MinimumMovesToCaptureTheQueenBenchmarks { BatchSize = SmallestBatchSize };
        harness.Setup();

        return harness;
    }
}
