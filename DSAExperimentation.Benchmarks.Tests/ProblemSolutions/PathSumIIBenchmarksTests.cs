using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumIIBenchmarks (ARCHITECTURE 17.9): the baseline's path count against the
// one the workload's shape fixes, which the generic check cannot supply - it holds the root-to-leaf
// enumeration to the baseline, and a shared empty list would pass as agreement. Setup asks
// PathSumWorkloads for a tree in which exactly eight root-to-leaf paths sum to the target, which that
// fixture's own tests pin.
public sealed partial class PathSumIIBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    // The benchmark plants this many matching paths.
    private const int ExpectedMatchingPaths = 8;

    [Fact]
    public void RecursiveBacktrack_PlantedPaths_FindsEveryPlantedPath() =>
        Assert.Equal(ExpectedMatchingPaths, BuildHarness().RecursiveBacktrack().Count);

    private static PathSumIIBenchmarks BuildHarness()
    {
        var harness = new PathSumIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
