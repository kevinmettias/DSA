using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximalRectangleBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - summing every row-pair window into a column histogram against
// the one-histogram-sweep-per-row reduction to LC 84 - so a harness whose arms disagree is timing
// two different problems. Both arms return the largest all-'1' rectangle's area. Setup draws the
// cells from one fixed seed, so the same Size must rebuild the same matrix and the same area.
public sealed partial class MaximalRectangleBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().RowPairScan(), BuildHarness().RowPairScan());
        Assert.Equal(BuildHarness().RowHistogramStack(), BuildHarness().RowHistogramStack());
    }

    [Fact]
    public void RowPairScan_SeededBinaryMatrix_AgreesWithRowHistogramStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RowHistogramStack(), harness.RowPairScan());
    }

    [Fact]
    public void RowHistogramStack_SeededBinaryMatrix_AgreesWithRowPairScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RowPairScan(), harness.RowHistogramStack());
    }

    private static MaximalRectangleBenchmarks BuildHarness()
    {
        var harness = new MaximalRectangleBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
