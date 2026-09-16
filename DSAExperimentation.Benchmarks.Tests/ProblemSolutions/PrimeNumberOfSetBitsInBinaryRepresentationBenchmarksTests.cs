using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PrimeNumberOfSetBitsInBinaryRepresentationBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for one question - how many values in the range have a prime
// popcount - so a harness whose arms disagree is timing two different problems. RangeWidth is the
// only [Params] axis and Setup turns it into the [left, right] range both arms are handed, so the
// same RangeWidth must rebuild the same bounds.
public sealed partial class PrimeNumberOfSetBitsInBinaryRepresentationBenchmarksTests
{
    private const int SmallestRangeWidth = 1_000;

    [Fact]
    public void Setup_SameRangeWidth_RebuildsTheSameBounds() =>
        Assert.Equal(
            BuildHarness().TrialDivisionPerValue(),
            BuildHarness().TrialDivisionPerValue());

    [Fact]
    public void TrialDivisionPerValue_AgreesWithPrecomputedSetLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrecomputedSetLookup(), harness.TrialDivisionPerValue());
    }

    [Fact]
    public void PrecomputedSetLookup_AgreesWithTrialDivisionPerValue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrialDivisionPerValue(), harness.PrecomputedSetLookup());
    }

    private static PrimeNumberOfSetBitsInBinaryRepresentationBenchmarks BuildHarness()
    {
        var harness = new PrimeNumberOfSetBitsInBinaryRepresentationBenchmarks
        {
            RangeWidth = SmallestRangeWidth,
        };
        harness.Setup();

        return harness;
    }
}
