using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// PathSumIISolution's, competing searches for the same path list - the backtracking walk that
// builds each path as it descends against collecting every root-to-leaf path and testing each
// afterwards - so a harness whose arms disagree is timing two different problems. Setup asks
// PathSumWorkloads for a tree in which exactly eight root-to-leaf paths sum to the target, which
// that fixture's own tests pin, and the tests here pin that count as well as the agreement, so a
// shared empty list cannot pass as agreement.
public sealed partial class PathSumIIBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    // The benchmark plants this many matching paths.
    private const int ExpectedMatchingPaths = 8;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().RecursiveBacktrack()),
            AnswerGraphText.Of(BuildHarness().RecursiveBacktrack()));

    [Fact]
    public void RecursiveBacktrack_PlantedPaths_AgreesWithAllRootToLeafPaths()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchingPaths, harness.AllRootToLeafPaths().Count);
        Assert.Equal(AnswerGraphText.Of(harness.AllRootToLeafPaths()), AnswerGraphText.Of(harness.RecursiveBacktrack()));
    }

    [Fact]
    public void AllRootToLeafPaths_PlantedPaths_AgreesWithRecursiveBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchingPaths, harness.RecursiveBacktrack().Count);
        Assert.Equal(AnswerGraphText.Of(harness.RecursiveBacktrack()), AnswerGraphText.Of(harness.AllRootToLeafPaths()));
    }

    private static PathSumIIBenchmarks BuildHarness()
    {
        var harness = new PathSumIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
