using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// PathSumIISolution's, competing searches for the same path list - the backtracking walk that
// builds each path as it descends against collecting every root-to-leaf path and testing each
// afterwards - so a harness whose arms disagree is timing two different problems. The harness has
// no tuned parameter at all: Setup builds LeetCode 113's own example tree, two of whose four
// root-to-leaf paths the fixture names as summing to the target, and the tests pin that count as
// well as the agreement, so a shared empty list cannot pass as agreement.
public sealed partial class PathSumIIBenchmarksTests
{
    private const int ExpectedMatchingPaths = 2;

    [Fact]
    public void Setup_ExampleTree_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RecursiveBacktrack()),
            AnswerText.Of(BuildHarness().RecursiveBacktrack()));

    [Fact]
    public void RecursiveBacktrack_ExampleTree_AgreesWithAllRootToLeafPaths()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchingPaths, harness.AllRootToLeafPaths().Count);
        Assert.Equal(AnswerText.Of(harness.AllRootToLeafPaths()), AnswerText.Of(harness.RecursiveBacktrack()));
    }

    [Fact]
    public void AllRootToLeafPaths_ExampleTree_AgreesWithRecursiveBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchingPaths, harness.RecursiveBacktrack().Count);
        Assert.Equal(AnswerText.Of(harness.RecursiveBacktrack()), AnswerText.Of(harness.AllRootToLeafPaths()));
    }

    private static PathSumIIBenchmarks BuildHarness()
    {
        var harness = new PathSumIIBenchmarks();
        harness.Setup();

        return harness;
    }
}
