using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfDigitOneBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - materializing and scanning every integer 1..UpperBound against
// the place-value tally that visits only UpperBound's own decimal digits - so a harness whose arms
// disagree is tallying two different ranges. The class carries no [GlobalSetup]: UpperBound is the
// whole workload, passed straight through to both arms, so the same UpperBound must answer both.
//
// Both arms return a long, so they are compared directly.
public sealed partial class NumberOfDigitOneBenchmarksTests
{
    private const int SmallestUpperBound = 20_000;

    [Fact]
    public void BruteForceScan_AgreesWithDigitPositionTally()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DigitPositionTally(), harness.BruteForceScan());
    }

    [Fact]
    public void DigitPositionTally_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceScan(), harness.DigitPositionTally());
    }

    private static NumberOfDigitOneBenchmarks BuildHarness() => new() { UpperBound = SmallestUpperBound };
}
