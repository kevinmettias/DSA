using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DetonateTheMaximumBombsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the hand-rolled bool[] recursion against the same
// successor closure run through this repo's DepthFirstSearch.Traverse - so a harness whose arms
// disagree is timing two different problems. Every bomb is generated with a radius of at least one,
// so a triggered bomb always detonates itself and the answer lies in [1, BombCount]; the bomb grid
// is drawn from one fixed seed, so the same BombCount must rebuild the same workload.
public sealed partial class DetonateTheMaximumBombsBenchmarksTests
{
    private const int SmallestBombCount = 50;
    private const int MinimumDetonationsForARadiusBearingBomb = 1;

    [Fact]
    public void Setup_RadiusBearingBombs_BoundTheAnswerAndTheSameBombCountRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var detonated = harness.ManualRecursiveVisit();

        Assert.InRange(detonated, MinimumDetonationsForARadiusBearingBomb, SmallestBombCount);
        Assert.Equal(detonated, BuildHarness().ManualRecursiveVisit());
    }

    [Fact]
    public void ManualRecursiveVisit_ChainedBlastRadius_AgreesWithRepoDepthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepoDepthFirstSearch(), harness.ManualRecursiveVisit());
    }

    [Fact]
    public void RepoDepthFirstSearch_ChainedBlastRadius_AgreesWithManualRecursiveVisit()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualRecursiveVisit(), harness.RepoDepthFirstSearch());
    }

    private static DetonateTheMaximumBombsBenchmarks BuildHarness()
    {
        var harness = new DetonateTheMaximumBombsBenchmarks { BombCount = SmallestBombCount };
        harness.Setup();

        return harness;
    }
}
