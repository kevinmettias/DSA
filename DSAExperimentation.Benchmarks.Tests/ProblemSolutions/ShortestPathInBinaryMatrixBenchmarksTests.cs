using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestPathInBinaryMatrixBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the baseline's answer to one derived outside the arms. Both
// corners are forced clear and the grid is sparse, so a grid whose arms agreed only on the problem's
// "no clear path" sentinel would be agreement about nothing: the seeded grid at the smallest Size
// carries a real clear path, which is what the expected length below pins.
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
    public void BclQueueBfs_TenByTenGrid_FindsTheSeededShortestPath() =>
        Assert.Equal(ExpectedShortestPathLength, BuildHarness().BclQueueBfs());

    private static ShortestPathInBinaryMatrixBenchmarks BuildHarness()
    {
        var harness = new ShortestPathInBinaryMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
