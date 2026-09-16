using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SimplifiedFractionsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - trial division against the Euclidean algorithm
// to decide which of the pairs below a denominator are coprime - so a harness whose arms
// disagree is listing two different sets of fractions. Both arms now build LeetCode's own
// formatted answer, and DenominatorLimit is the only input either takes, so a harness is that
// one property and nothing else: this class has no [GlobalSetup] to call.
//
// Fractions come back ordered by denominator and then by numerator, and the position of "p/q"
// is part of this answer, so AnswerText.Of rather than OfUnorderedSet keeps each fraction
// scored against its own position.
public sealed partial class SimplifiedFractionsBenchmarksTests
{
    private const int SmallestDenominatorLimit = 200;

    [Fact]
    public void TrialDivisionGcd_TwoHundredDenominatorLimit_AgreesWithEuclideanGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.EuclideanGcd()),
            AnswerText.Of(harness.TrialDivisionGcd()));
    }

    [Fact]
    public void EuclideanGcd_TwoHundredDenominatorLimit_AgreesWithTrialDivisionGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TrialDivisionGcd()),
            AnswerText.Of(harness.EuclideanGcd()));
    }

    private static SimplifiedFractionsBenchmarks BuildHarness() =>
        new() { DenominatorLimit = SmallestDenominatorLimit };
}
