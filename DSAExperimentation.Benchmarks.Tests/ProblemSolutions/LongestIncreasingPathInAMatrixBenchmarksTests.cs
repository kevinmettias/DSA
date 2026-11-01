using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestIncreasingPathInAMatrixBenchmarks (ARCHITECTURE 17.9): Setup fills
// the matrix row-major with 0..Size^2-1, so every cell's only increasing neighbors are right and
// down: the longest increasing path steps down the last row and right along it, a run of
// 2 * Size - 1 cells - an expected value derived from the workload's construction rather than from
// either arm, which the generic agreement check cannot supply. The baseline returns the path length,
// a scalar compared directly.
public sealed partial class LongestIncreasingPathInAMatrixBenchmarksTests
{
    private const int SmallestSize = 6;

    // A path from the top-left cell to the bottom-right one visits one cell per row and one cell
    // per column, counting the shared corner cell once.
    private const int ExpectedLongestIncreasingPathLength = (2 * SmallestSize) - 1;

    [Fact]
    public void NaiveRecursion_SmallestSize_RunsCornerToCorner() =>
        Assert.Equal(ExpectedLongestIncreasingPathLength, BuildHarness().NaiveRecursion(SmallestSize));

    private static LongestIncreasingPathInAMatrixBenchmarks BuildHarness()
    {
        var harness = new LongestIncreasingPathInAMatrixBenchmarks();
        harness.Setup();

        return harness;
    }
}
