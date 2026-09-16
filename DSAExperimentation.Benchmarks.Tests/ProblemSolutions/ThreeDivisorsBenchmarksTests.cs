using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ThreeDivisorsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(num) full-range trial division against
// BinarySearch.LowerBound anchored at floor(sqrt(num)) - so a harness whose arms disagree is timing
// two different problems. Both arms tally how many of the workload's numbers have exactly three
// divisors into an int, so they are compared directly. Setup draws the numbers from a fixed seed,
// so the same Length must rebuild the same workload.
public sealed partial class ThreeDivisorsBenchmarksTests
{
    private const int SmallestLength = 200;

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

    private static ThreeDivisorsBenchmarks BuildHarness()
    {
        var harness = new ThreeDivisorsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
