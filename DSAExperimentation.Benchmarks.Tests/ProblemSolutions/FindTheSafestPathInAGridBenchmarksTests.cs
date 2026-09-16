using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheSafestPathInAGridBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - re-walking the safeness grid by BFS once per
// binary-search step against a single widest-path pass over a heap - so a harness whose arms
// disagree is timing two different problems. Setup draws the grid from one fixed seed and clears
// both endpoints, so the same Size must rebuild the same grid.
public sealed partial class FindTheSafestPathInAGridBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BclBfs(), BuildHarness().BclBfs());

    [Fact]
    public void BclBfs_SmallestSize_AgreesWithHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Heap(), harness.BclBfs());
    }

    [Fact]
    public void Heap_SmallestSize_AgreesWithBclBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BclBfs(), harness.Heap());
    }

    private static FindTheSafestPathInAGridBenchmarks BuildHarness()
    {
        var harness = new FindTheSafestPathInAGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
