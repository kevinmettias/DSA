using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CalculateAmountPaidInTaxesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - walking LeetCode's int[][] brackets directly against
// the same brackets behind IRandomAccessSequence<T>'s Get - so a harness whose arms disagree is
// timing two different tables, and this is the class whose whole point is that swapping the
// representation costs nothing. Both arms return a double, so they are compared under a named
// relative tolerance rather than by exact equality; the fractions of a cent a tax table can produce
// are not what the comparison is about. Setup builds the brackets and the income that sits one unit
// below the top bracket, so the same BracketCount must rebuild both.
public sealed partial class CalculateAmountPaidInTaxesBenchmarksTests
{
    private const int SmallestBracketCount = 200;

    // Both arms round the same way, so agreement is expected far below this; the tolerance exists so
    // a last-bit difference in the two accumulations cannot fail the harness for the wrong reason.
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void Setup_SameBracketCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BracketArrayWalk(),
            BuildHarness().BracketArrayWalk(),
            RelativeTolerance);

    [Fact]
    public void BracketArrayWalk_TwoHundredBrackets_AgreesWithRandomAccessSequenceWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RandomAccessSequenceWalk(), harness.BracketArrayWalk(), RelativeTolerance);
    }

    [Fact]
    public void RandomAccessSequenceWalk_TwoHundredBrackets_AgreesWithBracketArrayWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BracketArrayWalk(), harness.RandomAccessSequenceWalk(), RelativeTolerance);
    }

    private static CalculateAmountPaidInTaxesBenchmarks BuildHarness()
    {
        var harness = new CalculateAmountPaidInTaxesBenchmarks { BracketCount = SmallestBracketCount };
        harness.Setup();

        return harness;
    }
}
