using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfBeautifulIntegersInTheRangeBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - walking every integer in [Low, High] against
// the digit-DP memo whose cost depends only on High's digit count - so a harness whose arms disagree
// is counting two different ranges. Setup derives the exclusive upper bound from RangeSize, so the
// same RangeSize must rebuild the same [Low, High).
//
// Both arms return a long, so they are compared directly. Neither arm keeps state between calls (the
// digit DP allocates its own memo per call), so one harness serves both arms in either order.
public sealed partial class NumberOfBeautifulIntegersInTheRangeBenchmarksTests
{
    private const int SmallestRangeSize = 100_000;

    [Fact]
    public void Setup_SameRangeSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithDigitDpMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DigitDpMemo(), harness.BruteForce());
    }

    [Fact]
    public void DigitDpMemo_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.DigitDpMemo());
    }

    private static NumberOfBeautifulIntegersInTheRangeBenchmarks BuildHarness()
    {
        var harness = new NumberOfBeautifulIntegersInTheRangeBenchmarks { RangeSize = SmallestRangeSize };
        harness.Setup();

        return harness;
    }
}
