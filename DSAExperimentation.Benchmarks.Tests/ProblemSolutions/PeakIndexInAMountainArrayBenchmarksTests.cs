using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PeakIndexInAMountainArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// PeakIndexInAMountainArraySolution's, competing searches for the same peak index - the linear
// scan against the halving predicate search for the first downhill step - so a harness whose arms disagree is timing two
// different problems. Setup builds the mountain from Length alone with its peak at the midpoint,
// so the same Length must rebuild the same mountain, and the tests pin that midpoint index as
// well as the agreement, since agreeing on the wrong index would otherwise pass.
public sealed partial class PeakIndexInAMountainArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup places the peak at the array's midpoint, the same halving the shared constant names.
    private const int ExpectedPeakIndex = SmallestLength / AlgorithmConstants.HalvingFactor;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_MidpointPeakMountain_AgreesWithPredicateSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPeakIndex, harness.PredicateSearch());
        Assert.Equal(harness.PredicateSearch(), harness.LinearScan());
    }

    [Fact]
    public void PredicateSearch_MidpointPeakMountain_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPeakIndex, harness.LinearScan());
        Assert.Equal(harness.LinearScan(), harness.PredicateSearch());
    }

    private static PeakIndexInAMountainArrayBenchmarks BuildHarness()
    {
        var harness = new PeakIndexInAMountainArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
