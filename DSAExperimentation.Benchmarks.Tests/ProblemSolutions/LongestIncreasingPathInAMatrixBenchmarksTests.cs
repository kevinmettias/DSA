using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestIncreasingPathInAMatrixBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - re-exploring every shared sub-path from
// scratch against the memoized recurrence - so a harness whose arms disagree is timing two
// different problems. Both arms return the path length, a scalar compared directly. Setup fills
// the matrix row-major with 0..Size^2-1, so every cell's only increasing neighbors are right and
// down: the longest increasing path steps down the last row and right along it, a run of
// 2 * Size - 1 cells, which is the decisive value both arms must reach and the same Size must
// rebuild.
public sealed partial class LongestIncreasingPathInAMatrixBenchmarksTests
{
    private const int SmallestSize = 6;

    // A path from the top-left cell to the bottom-right one visits one cell per row and one cell
    // per column, counting the shared corner cell once.
    private const int ExpectedLongestIncreasingPathLength = (2 * SmallestSize) - 1;

    [Fact]
    public void Setup_SmallestSize_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestIncreasingPathLength, BuildHarness().MemoizedRecurrence());
        Assert.Equal(BuildHarness().NaiveRecursion(), BuildHarness().NaiveRecursion());
    }

    [Fact]
    public void NaiveRecursion_SmallestSize_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestIncreasingPathLength, harness.NaiveRecursion());
        Assert.Equal(harness.MemoizedRecurrence(), harness.NaiveRecursion());
    }

    [Fact]
    public void MemoizedRecurrence_SmallestSize_AgreesWithNaiveRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestIncreasingPathLength, harness.MemoizedRecurrence());
        Assert.Equal(harness.NaiveRecursion(), harness.MemoizedRecurrence());
    }

    private static LongestIncreasingPathInAMatrixBenchmarks BuildHarness()
    {
        var harness = new LongestIncreasingPathInAMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
