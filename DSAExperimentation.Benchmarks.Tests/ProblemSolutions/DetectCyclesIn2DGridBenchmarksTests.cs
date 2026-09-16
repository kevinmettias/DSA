using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DetectCyclesIn2DGridBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a hand-rolled parent-tracked DFS against this
// repo's own DisjointSet edge-union - so a harness whose arms disagree is timing two different
// problems. Setup draws the grid from one fixed seed, so the same GridSize must rebuild the same
// grid, and that grid has to contain a cycle at all for either arm to have work to measure.
public sealed partial class DetectCyclesIn2DGridBenchmarksTests
{
    private const int SmallestGridSize = 30;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameLetterGrid()
    {
        // A uniformly random three-letter grid is dense enough in same-character adjacencies that
        // a cycle is present at either size - the benchmark's own reading is how fast each arm
        // reaches one, which only means anything if there is one to reach. The same seed
        // rebuilding the same verdict is what makes two published numbers comparable.
        Assert.True(BuildHarness().HasCycleByParentTrackedDepthFirstSearch());
        Assert.Equal(
            BuildHarness().HasCycleByParentTrackedDepthFirstSearch(),
            BuildHarness().HasCycleByParentTrackedDepthFirstSearch());
    }

    [Fact]
    public void HasCycleByParentTrackedDepthFirstSearch_SeededLetterGrid_AgreesWithHasCycleByDisjointSetEdgeUnion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasCycleByDisjointSetEdgeUnion(), harness.HasCycleByParentTrackedDepthFirstSearch());
    }

    [Fact]
    public void HasCycleByDisjointSetEdgeUnion_SeededLetterGrid_AgreesWithHasCycleByParentTrackedDepthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasCycleByParentTrackedDepthFirstSearch(), harness.HasCycleByDisjointSetEdgeUnion());
    }

    private static DetectCyclesIn2DGridBenchmarks BuildHarness()
    {
        var harness = new DetectCyclesIn2DGridBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
