using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumBenchmarks (ARCHITECTURE 17.9): its two arms are PathSumSolution's,
// competing searches for the same yes/no question - the plain root-to-leaf recursion against
// enumerating every root-to-leaf path and testing each - so a harness whose arms disagree is
// timing two different problems. The harness has no tuned parameter at all: Setup builds LeetCode
// 112's own example tree, whose accepting path the fixture names, and both arms read that same
// tree and target.
public sealed partial class PathSumBenchmarksTests
{
    [Fact]
    public void Setup_ExampleTree_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().HasPathSumByRecursion(),
            BuildHarness().HasPathSumByRecursion());

    [Fact]
    public void HasPathSumByRecursion_ExampleTree_AgreesWithHasPathSumByPathEnumeration()
    {
        var harness = BuildHarness();

        Assert.True(harness.HasPathSumByPathEnumeration());
        Assert.Equal(harness.HasPathSumByPathEnumeration(), harness.HasPathSumByRecursion());
    }

    [Fact]
    public void HasPathSumByPathEnumeration_ExampleTree_AgreesWithHasPathSumByRecursion()
    {
        var harness = BuildHarness();

        Assert.True(harness.HasPathSumByRecursion());
        Assert.Equal(harness.HasPathSumByRecursion(), harness.HasPathSumByPathEnumeration());
    }

    private static PathSumBenchmarks BuildHarness()
    {
        var harness = new PathSumBenchmarks();
        harness.Setup();

        return harness;
    }
}
