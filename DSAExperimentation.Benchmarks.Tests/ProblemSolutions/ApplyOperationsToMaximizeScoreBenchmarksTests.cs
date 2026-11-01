using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ApplyOperationsToMaximizeScoreBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to each other, and this asserts the score is positive - Setup draws values of at least two with a
// non-empty budget, so an arm that spent nothing cannot agree its way past the harness.
public sealed partial class ApplyOperationsToMaximizeScoreBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void LinearBoundaryScan_TwoHundredValueArray_ReturnsAPositiveScore() =>
        Assert.True(BuildHarness().LinearBoundaryScan() > 0);

    private static ApplyOperationsToMaximizeScoreBenchmarks BuildHarness()
    {
        var harness = new ApplyOperationsToMaximizeScoreBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
