using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DetectCyclesIn2DGridBenchmarks (ARCHITECTURE 17.9): a property of the
// workload's shape the generic check cannot see - the seeded grid has to contain a cycle at all for
// either arm to have work to measure.
public sealed partial class DetectCyclesIn2DGridBenchmarksTests
{
    private const int SmallestGridSize = 30;

    // A uniformly random three-letter grid is dense enough in same-character adjacencies that
    // a cycle is present at either size - the benchmark's own reading is how fast each arm
    // reaches one, which only means anything if there is one to reach.
    [Fact]
    public void HasCycleByParentTrackedDepthFirstSearch_SeededLetterGrid_FindsACycle() =>
        Assert.True(BuildHarness().HasCycleByParentTrackedDepthFirstSearch());

    private static DetectCyclesIn2DGridBenchmarks BuildHarness()
    {
        var harness = new DetectCyclesIn2DGridBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
