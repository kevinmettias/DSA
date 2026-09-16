using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TheKthFactorOfNBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - full-range trial division against BinarySearch.LowerBound
// anchored at floor(sqrt(n)) - so a harness whose arms disagree is timing two different problems.
// Both arms tally their per-number factor into an int, so they are compared directly; K is fixed
// above every n's divisor count, so both arms are forced through the full scan and every number
// contributes to the tally. Setup draws the values from a fixed seed, so the same Length must
// rebuild the same workload.
public sealed partial class TheKthFactorOfNBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().FullRangeScan(), BuildHarness().FullRangeScan());

    [Fact]
    public void FullRangeScan_SmallestLength_AgreesWithBinarySearchAnchored()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchAnchored(), harness.FullRangeScan());
    }

    [Fact]
    public void BinarySearchAnchored_SmallestLength_AgreesWithFullRangeScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullRangeScan(), harness.BinarySearchAnchored());
    }

    private static TheKthFactorOfNBenchmarks BuildHarness()
    {
        var harness = new TheKthFactorOfNBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
