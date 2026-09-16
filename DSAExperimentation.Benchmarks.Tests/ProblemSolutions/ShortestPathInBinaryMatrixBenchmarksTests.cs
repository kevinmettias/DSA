using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestPathInBinaryMatrixBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the identical 8-directional BFS to the far
// corner, drained from the BCL's Queue<T> in one arm and from this repo's own Queue<TElement>
// in the other - so a harness whose arms disagree is walking two different grids. Setup draws
// the grid from a fixed seed, so the same Size must rebuild the same blocked cells; otherwise
// two published numbers were never comparable.
//
// Both corners are forced clear and the grid is sparse, so a grid whose arms agreed only on
// the problem's "no clear path" sentinel would be agreement about nothing: the seeded grid at
// the smallest Size carries a real clear path, which is what the expected length below pins.
public sealed partial class ShortestPathInBinaryMatrixBenchmarksTests
{
    private const int SmallestSize = 10;

    // The number of cells on the shortest clear path through Setup's own seeded grid: the
    // oracle is the king-move BFS the problem defines, evaluated independently over Setup's
    // documented one-in-ten blocked-cell generator at SmallestSize. Size + (SmallestSize - 1)
    // would be the unobstructed diagonal counted in cells, so this length is a real detour
    // rather than the straight line, and it is never the -1 that means no path exists.
    private const int ExpectedShortestPathLength = 11;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameGrid() =>
        Assert.Equal(BuildHarness().BclQueueBfs(), BuildHarness().BclQueueBfs());

    [Fact]
    public void BclQueueBfs_TenByTenGrid_AgreesWithRepoQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepoQueueBfs(), harness.BclQueueBfs());
        Assert.Equal(ExpectedShortestPathLength, harness.BclQueueBfs());
    }

    [Fact]
    public void RepoQueueBfs_TenByTenGrid_AgreesWithBclQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BclQueueBfs(), harness.RepoQueueBfs());
        Assert.Equal(ExpectedShortestPathLength, harness.RepoQueueBfs());
    }

    private static ShortestPathInBinaryMatrixBenchmarks BuildHarness()
    {
        var harness = new ShortestPathInBinaryMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
