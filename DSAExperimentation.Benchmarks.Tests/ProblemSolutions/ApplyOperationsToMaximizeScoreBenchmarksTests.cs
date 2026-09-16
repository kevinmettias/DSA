using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ApplyOperationsToMaximizeScoreBenchmarks (ARCHITECTURE 17.9): its two arms are
// ApplyOperationsToMaximizeScoreSolution's competing strategies for the same question - an outward scan for each
// index's two boundaries against one monotonic stack pass per side - so a harness whose arms disagree has scored
// two different arrays. Both arms return the maximized modular score as a long, one scalar, so they are compared
// directly; the score is positive because Setup draws values of at least two with a non-empty budget, which is
// asserted too, so an arm that spent nothing cannot agree its way past the harness. Setup draws the array from one
// seed, so the same Length must rebuild the same values.
public sealed partial class ApplyOperationsToMaximizeScoreBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearBoundaryScan(), BuildHarness().LinearBoundaryScan());

    [Fact]
    public void LinearBoundaryScan_TwoHundredValueArray_AgreesWithStackBoundaryScan()
    {
        var harness = BuildHarness();

        Assert.True(harness.LinearBoundaryScan() > 0);
        Assert.Equal(harness.StackBoundaryScan(), harness.LinearBoundaryScan());
    }

    [Fact]
    public void StackBoundaryScan_TwoHundredValueArray_AgreesWithLinearBoundaryScan()
    {
        var harness = BuildHarness();

        Assert.True(harness.StackBoundaryScan() > 0);
        Assert.Equal(harness.LinearBoundaryScan(), harness.StackBoundaryScan());
    }

    private static ApplyOperationsToMaximizeScoreBenchmarks BuildHarness()
    {
        var harness = new ApplyOperationsToMaximizeScoreBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
