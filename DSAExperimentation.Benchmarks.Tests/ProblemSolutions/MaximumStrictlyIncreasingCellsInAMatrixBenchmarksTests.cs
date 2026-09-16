using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumStrictlyIncreasingCellsInAMatrixBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the per-cell row/column rescan against the
// sort-every-cell-once batch DP - so a harness whose arms disagree is timing two different problems.
// Setup draws the matrix from one fixed seed, so the same Size must rebuild the same workload;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumStrictlyIncreasingCellsInAMatrixBenchmarksTests
{
    private const int SmallestSize = 8;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().SortedBatchDp(), BuildHarness().SortedBatchDp());

    [Fact]
    public void MemoizedRowColumnScan_SquareMatrix_AgreesWithSortedBatchDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedBatchDp(), harness.MemoizedRowColumnScan());
    }

    [Fact]
    public void SortedBatchDp_SquareMatrix_AgreesWithMemoizedRowColumnScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRowColumnScan(), harness.SortedBatchDp());
    }

    private static MaximumStrictlyIncreasingCellsInAMatrixBenchmarks BuildHarness()
    {
        var harness = new MaximumStrictlyIncreasingCellsInAMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
