using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathWithMinimumEffortBenchmarks (ARCHITECTURE 17.9): its two arms are
// PathWithMinimumEffortSolution's, competing searches for the same minimum-effort path - binary
// search over candidate efforts, each checked by a flood fill, against a single heap-ordered
// Dijkstra - so a harness whose arms disagree is timing two different problems. Setup draws the
// height grid from one fixed seed, so the same Size must rebuild the same grid, and both arms read
// that one grid.
public sealed partial class PathWithMinimumEffortBenchmarksTests
{
    private const int SmallestSize = 15;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BinarySearchFloodFill(), BuildHarness().BinarySearchFloodFill());

    [Fact]
    public void BinarySearchFloodFill_SeededHeightGrid_AgreesWithHeapDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapDijkstra(), harness.BinarySearchFloodFill());
    }

    [Fact]
    public void HeapDijkstra_SeededHeightGrid_AgreesWithBinarySearchFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchFloodFill(), harness.HeapDijkstra());
    }

    private static PathWithMinimumEffortBenchmarks BuildHarness()
    {
        var harness = new PathWithMinimumEffortBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
