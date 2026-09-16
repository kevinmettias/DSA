using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfIdealArraysBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - trial division re-paid per value against a
// smallest-prime-factor sieve amortized across the whole range - so a harness whose arms disagree is
// timing two different problems, not two ways of answering one. There is no [GlobalSetup] here: the
// array length is fixed at 4 and MaxValue is the only argument either arm takes, so there is nothing
// to prepare ahead of the measured call and nothing to rebuild between two runs of the same params.
public sealed partial class CountTheNumberOfIdealArraysBenchmarksTests
{
    private const int SmallestMaxValue = 200;

    [Fact]
    public void TrialDivisionPerValue_FixedLengthFour_AgreesWithSmallestPrimeFactorSieve()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SmallestPrimeFactorSieve(), harness.TrialDivisionPerValue());
    }

    [Fact]
    public void SmallestPrimeFactorSieve_FixedLengthFour_AgreesWithTrialDivisionPerValue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrialDivisionPerValue(), harness.SmallestPrimeFactorSieve());
    }

    private static CountTheNumberOfIdealArraysBenchmarks BuildHarness() =>
        new() { MaxValue = SmallestMaxValue };
}
