using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumBenchmarks (ARCHITECTURE 17.9): its two arms are PathSumSolution's,
// competing searches for the same yes/no question - the plain root-to-leaf recursion against
// enumerating every root-to-leaf path and testing each - so a harness whose arms disagree is
// timing two different problems. Setup asks PathSumWorkloads for a tree with exactly one matching
// root-to-leaf path, which that fixture's own tests pin, so the decisive verdict is true and both
// arms read that same tree and target.
public sealed partial class PathSumBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().HasPathSumByRecursion(),
            BuildHarness().HasPathSumByRecursion());

    [Fact]
    public void HasPathSumByRecursion_PlantedPath_AgreesWithHasPathSumByPathEnumeration()
    {
        var harness = BuildHarness();

        Assert.True(harness.HasPathSumByPathEnumeration());
        Assert.Equal(harness.HasPathSumByPathEnumeration(), harness.HasPathSumByRecursion());
    }

    [Fact]
    public void HasPathSumByPathEnumeration_PlantedPath_AgreesWithHasPathSumByRecursion()
    {
        var harness = BuildHarness();

        Assert.True(harness.HasPathSumByRecursion());
        Assert.Equal(harness.HasPathSumByRecursion(), harness.HasPathSumByPathEnumeration());
    }

    private static PathSumBenchmarks BuildHarness()
    {
        var harness = new PathSumBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
