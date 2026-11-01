using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumBenchmarks (ARCHITECTURE 17.9): the baseline's verdict against the one
// the workload's shape fixes, which the generic check cannot supply - it holds the path enumeration to
// the baseline. Setup asks PathSumWorkloads for a tree with exactly one matching root-to-leaf path,
// which that fixture's own tests pin, so the decisive verdict is true.
public sealed partial class PathSumBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void HasPathSumByRecursion_PlantedPath_FindsThePath() =>
        Assert.True(BuildHarness().HasPathSumByRecursion());

    private static PathSumBenchmarks BuildHarness()
    {
        var harness = new PathSumBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
