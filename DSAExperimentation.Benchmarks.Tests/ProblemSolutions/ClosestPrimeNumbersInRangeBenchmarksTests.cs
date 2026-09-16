using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClosestPrimeNumbersInRangeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - trial-dividing every candidate in [2, Right] against
// sieving that same range - so a harness whose arms disagree is timing two different problems. The
// class carries no [GlobalSetup] and both arms read Right directly, so the same Right must produce
// the same answer pair. AnswerText.Of, not OfUnorderedSet: the pair is ordered (num1 < num2) and
// that order is part of the answer, not an accident of how it was built.
public sealed partial class ClosestPrimeNumbersInRangeBenchmarksTests
{
    private const int SmallestRight = 2_000;

    [Fact]
    public void SieveScan_RangeStartingAtTwo_AgreesWithTrialDivisionScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.TrialDivisionScan()), AnswerText.Of(harness.SieveScan()));
    }

    [Fact]
    public void TrialDivisionScan_RangeStartingAtTwo_AgreesWithSieveScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.SieveScan()), AnswerText.Of(harness.TrialDivisionScan()));
    }

    private static ClosestPrimeNumbersInRangeBenchmarks BuildHarness() => new() { Right = SmallestRight };
}
