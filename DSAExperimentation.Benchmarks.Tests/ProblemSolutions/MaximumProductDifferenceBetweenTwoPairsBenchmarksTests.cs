using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumProductDifferenceBetweenTwoPairsBenchmarks (ARCHITECTURE 17.9): both
// arms are MaximumProductDifferenceBetweenTwoPairsSolution's competing strategies for one question
// - the quadratic pair scan against one merge sort that reads off the two extremes - so a harness
// whose arms disagree is timing two different problems. Both answer with a single difference,
// compared directly.
public sealed partial class MaximumProductDifferenceBetweenTwoPairsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValueArray()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The value array is private, so the rebuild is pinned through the difference it produces:
        // the same Length must draw the same seeded positive values and pick the same two pairs.
        Assert.Equal(first.BruteForcePairScan(), second.BruteForcePairScan());
        Assert.Equal(first.MergeSortExtremes(), second.MergeSortExtremes());
    }

    [Fact]
    public void BruteForcePairScan_SeededValueArray_AgreesWithMergeSortExtremes()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortExtremes(), harness.BruteForcePairScan());
    }

    [Fact]
    public void MergeSortExtremes_SeededValueArray_AgreesWithBruteForcePairScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePairScan(), harness.MergeSortExtremes());
    }

    private static MaximumProductDifferenceBetweenTwoPairsBenchmarks BuildHarness()
    {
        var harness = new MaximumProductDifferenceBetweenTwoPairsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
