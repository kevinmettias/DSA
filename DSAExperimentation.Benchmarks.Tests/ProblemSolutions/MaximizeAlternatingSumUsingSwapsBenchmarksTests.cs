using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeAlternatingSumUsingSwapsBenchmarks (ARCHITECTURE 17.9): both arms
// are competing strategies for one question - the largest alternating sum reachable by permuting
// values inside each swap-connected component - so a harness whose arms disagree is timing two
// different problems. Setup draws its values and swap pairs from a fixed seed, so the same element
// count must rebuild the same workload; otherwise two published numbers were never comparable.
public sealed partial class MaximizeAlternatingSumUsingSwapsBenchmarksTests
{
    private const int SmallestElementCount = 1_000;

    [Fact]
    public void Setup_SameElementCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ComponentBfs(), BuildHarness().ComponentBfs());

    [Fact]
    public void ComponentBfs_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ComponentBfs(), harness.DisjointSet());
    }

    [Fact]
    public void DisjointSet_AgreesWithComponentBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSet(), harness.ComponentBfs());
    }

    private static MaximizeAlternatingSumUsingSwapsBenchmarks BuildHarness()
    {
        var harness = new MaximizeAlternatingSumUsingSwapsBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();

        return harness;
    }
}
