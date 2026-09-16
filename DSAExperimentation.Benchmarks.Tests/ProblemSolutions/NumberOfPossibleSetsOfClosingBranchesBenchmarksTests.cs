using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfPossibleSetsOfClosingBranchesBenchmarks (ARCHITECTURE 17.9): both arms
// count the branch sets that may be closed under the maxDistance rule - the hand-rolled
// Floyd-Warshall over a restricted matrix against this repo's own AllPairsShortestPaths over a built
// BranchNetwork - so a harness whose arms disagree is timing two different questions. The count is the
// problem's whole answer rather than a proxy.
//
// Both arms read the two structures Setup hoists into fields, and neither writes to what it reads:
// the matrix arm copies each restriction into a fresh matrix before refining it, and the graph arm
// only reads node edges. One harness can therefore be called twice in either order. Setup builds the
// road list, so the same ExtraEdgesPerNode must rebuild the same network.
public sealed partial class NumberOfPossibleSetsOfClosingBranchesBenchmarksTests
{
    private const int SmallestExtraEdgesPerNode = 1;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceFloydWarshall(), BuildHarness().BruteForceFloydWarshall());

    [Fact]
    public void BruteForceFloydWarshall_AgreesWithAllPairsShortestPaths()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AllPairsShortestPaths(), harness.BruteForceFloydWarshall());
    }

    [Fact]
    public void AllPairsShortestPaths_AgreesWithBruteForceFloydWarshall()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceFloydWarshall(), harness.AllPairsShortestPaths());
    }

    private static NumberOfPossibleSetsOfClosingBranchesBenchmarks BuildHarness()
    {
        var harness = new NumberOfPossibleSetsOfClosingBranchesBenchmarks
        {
            ExtraEdgesPerNode = SmallestExtraEdgesPerNode,
        };
        harness.Setup();

        return harness;
    }
}
